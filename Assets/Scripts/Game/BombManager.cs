using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BombType
{
    none = 2,
    hor = 4,
    ver = 8,
    super = 16,
    large = 32,
}

public struct BombItemInfo
{
    public GemsItem gem;
    public BombType bombType;
    public int gemType;
}

/// <summary>
/// 炸弹生成管理类
/// </summary>
public class BombManager
{
    Dictionary<int, MergeInfo> mergeInfosDic; //用于加快查询物体的分数信息

    public BombManager()
    {
        mergeInfosDic = new Dictionary<int, MergeInfo>();  
    }

    public void SwitchBomb(int count,Vector2Int[] gemsItem, GemsItem[,] gemsItemCollection, List<BombItemInfo> bombItems)
    {
        if (count == 4)
        {
            bool isVer = false;
            Vector2Int pos = gemsItem[1];
            //4个检出第一个和第二个的x和y轴判断是横向还是竖向
            if (gemsItem[0].x == gemsItem[1].x)
            {
                //如果行相等，则表明是横向的四个
                //如果是横向消除，则生成竖向炸弹
                //Debug.Log("竖向消除四个炸弹");
                gemsItemCollection[pos.x,pos.y].IsBomb = BombType.ver;
                isVer = true;
            }
            else
            {
                //Debug.Log("横向消除四个炸弹");
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.hor;
                isVer = false;
            }
            bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x,pos.y], bombType = isVer ?BombType.ver:BombType.hor});
        }
        else if (count >= 5)
        {
            Vector2Int pos;
            //如果大于等于五个，可能是直线型、T字型、L字型、十字型、还有其他形状
            if (this.IsLine5(gemsItem))
            {
                //生成超级炸弹
                //Debug.Log("生成超级炸弹");
                pos = gemsItem[count / 2];
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.super;
                bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x, pos.y], bombType = BombType.super });
            }
            else if(this.IsTShape(gemsItem) || this.IsLShape(gemsItem) || this.IsCrossShape(gemsItem))
            {
                //生成大炸弹
                //Debug.Log("生成大炸弹");
                pos = gemsItem[count / 2];
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.large;
                bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x, pos.y], bombType = BombType.large });
            }
            //其他形状则不需要处理
        }
        //其他数量则也不需要处理
    }

    public bool IsLine5(Vector2Int[] gemsItem)
    {
        //只要包含5个联排生成一个大炸弹
        int sameRow = 1;
        int sameCol = 1;
        int firstRow = gemsItem[0].x;
        int firstCol = gemsItem[0].y;

        for (int i = 0; i < gemsItem.Length; i++)
        {
            if (gemsItem[i].x != firstRow)
            {
                sameRow = 1;
                firstRow = gemsItem[i].x;
            }
            else
            {
                sameRow++;
                if(sameRow >= 5)
                {
                    return true;
                }
            }
            if (gemsItem[i].y != firstCol)
            {
                sameCol = 1;
                firstCol = gemsItem[i].y;
            }
            else
            {
                sameCol++;
                if(sameCol >= 5)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 检测T字型和十字型
    /// </summary>
    /// <param name="gemsItem"></param>
    /// <returns></returns>
    bool IsTOrCrossShape(Vector2Int[] gemsItem)
    {
        if (gemsItem.Length > 6) return false;
        foreach (var center in gemsItem)
        {
            int upCount = 0, downCount = 0, leftCount = 0, rightCount = 0;
            foreach (var point in gemsItem)
            {
                if (point.y == center.y)
                {
                    if (point.x < center.x) upCount++;
                    if (point.x > center.x) downCount++;
                }
                if (point.x == center.x)
                {
                    if (point.y < center.y) leftCount++;
                    if (point.y > center.y) rightCount++;
                }
            }

            //前两种T型判断
            if ((upCount + downCount >= 2) && ((leftCount >= 2 && rightCount >= 0 && rightCount < 2) || (leftCount >= 0 && leftCount < 2 && rightCount >= 2)))
            {
                return true;
            }

            //后两种T型判断
            if ((leftCount + rightCount >= 2) && ((upCount >= 2 && downCount >= 0 && downCount < 2) || (upCount >= 0 && upCount < 2 && downCount >= 2)))
            {
                return true;
            }


            //十字型判断
            if ((upCount >= 1 && downCount >= 1) && (leftCount >= 1 && rightCount >= 1))
            {
                return true;
            }
        }
        return false;
    }


    public bool IsTShape(Vector2Int[] gemsItem)
    {
        /*
                ￥   ￥      ￥￥￥    ￥          0     0      0 0 0     0  
            ￥￥￥   ￥￥￥    ￥      ￥       0000     0000     0       0
                ￥   ￥        ￥    ￥￥￥        0     0        0       0
                                                                 0     0 0 0  ....
         
         */
        //检测后的点存储的顺序是前三个是行上的点后两个是列上的点
        //只要包含T型就生成大炸弹

        //遍历每个点作为中心点，统计该中心点上下左右方向的连通点数量。若垂直方向（上下）至少有 2 个连通点，
        //且水平方向（左右）至少有一个方向有 2 个连通点，则判定为 T 型。
        if (gemsItem.Length > 6) return false;
        foreach (var center in gemsItem)
        {
            int upCount = 0, downCount = 0, leftCount = 0, rightCount = 0;
            foreach (var point in gemsItem)
            {
                if (point.y == center.y)
                {
                    if (point.x < center.x) upCount++;
                    if (point.x > center.x) downCount++;
                }
                if (point.x == center.x)
                {
                    if (point.y < center.y) leftCount++;
                    if (point.y > center.y) rightCount++;
                }
            }

            //前两种T型判断
            if ((upCount + downCount >= 2) && ((leftCount >= 2 && rightCount >= 0 && rightCount < 2) || (leftCount >= 0 && leftCount < 2 && rightCount >= 2)))
            {
                return true;
            }

            //后两种T型判断
            if ((leftCount + rightCount >= 2) && ((upCount >= 2 && downCount >= 0 && downCount < 2) || (upCount >= 0 && upCount < 2 && downCount >= 2)))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsLShape(Vector2Int[] gemsItem)
    {
        /*
            ￥          ￥       
            ￥          ￥
            ￥￥￥  ￥￥￥   ....
       */
        //行上列上可能不止三个点
        //如果行列上有有五个点，会被判定为两个超级炸弹
        //只要包含L型就生成大炸弹
        if (gemsItem.Length > 6) return false;
        //通过双重循环遍历点对，对于处于同一行或同一列的点对，分别统计水平和垂直方向的连通点数量。
        //若水平和垂直方向都至少有 3 个连通点，则判定为 L 型
        for (int i = 0; i < gemsItem.Length; i++)
        {
            for (int j = i + 1; j < gemsItem.Length; j++)
            {
                var p1 = gemsItem[i];
                var p2 = gemsItem[j];

                if (p1.x == p2.x)
                {
                    int horizontalCount = 0;
                    int verticalCount = 0;
                    foreach (var point in gemsItem)
                    {
                        if (point.x == p1.x) horizontalCount++;
                        if (point.y == p1.y || point.y == p2.y) verticalCount++;
                    }
                    if (horizontalCount >= 3 && verticalCount >= 3)
                    {
                        return true;
                    }
                }
                else if (p1.y == p2.y)
                {
                    int horizontalCount = 0;
                    int verticalCount = 0;
                    foreach (var point in gemsItem)
                    {
                        if (point.y == p1.y) verticalCount++;
                        if (point.x == p1.x || point.x == p2.x) horizontalCount++;
                    }
                    if (horizontalCount >= 3 && verticalCount >= 3)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断是否为十字型
    /// </summary>
    /// <param name="gemsItem"></param>
    /// <returns></returns>
    public bool IsCrossShape(Vector2Int[] gemsItem)
    {
        /*
              ￥
            ￥￥￥
              ￥
        */
        //行上列上可能不止三个点，可能是四个点、六、七、八、九
        //如果行列上有有五个点，会被判定为两个超级炸弹
        //只要包含十字型就生成大炸弹

        //遍历每个点作为中心点，统计该中心点上下左右方向的连通点数量。
        //若上下和左右方向分别至少有 2 个连通点（加上中心点就构成两组 3 个元素），则判定为十字型
        if (gemsItem.Length > 5) return false;
        foreach (var center in gemsItem)
        {
            int upCount = 0, downCount = 0, leftCount = 0, rightCount = 0;
            foreach (var point in gemsItem)
            {
                if (point.y == center.y)
                {
                    if (point.x < center.x) upCount++;
                    if (point.x > center.x) downCount++;
                }
                if (point.x == center.x)
                {
                    if (point.y < center.y) leftCount++;
                    if (point.y > center.y) rightCount++;
                }
            }
            if ((upCount >= 1 && downCount >= 1) && (leftCount >= 1 && rightCount >= 1))
            {
                return true;
            }
        }
        return false;
    }

    public void HandlerBomb(BombItemInfo bi, GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems,List<BombItemInfo> bombItems,List<List<MergeInfo>> bombMergeInfos)
    {
        if (bi.bombType == BombType.hor)
        {
            //横向炸弹消除
            this.HandleHorMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        else if (bi.bombType == BombType.ver)
        {
            //竖向炸弹消除
            this.HandlerVerMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        else if (bi.bombType == BombType.super)
        {
            //超级炸弹消除
            this.HandlerSuperMerge(bi.gem.Idx, gemsItemsCollect, gemsItems,bombItems, bombMergeInfos);
        }
        else if(bi.bombType == BombType.large)
        {
            //大炸弹消除
            this.HandlerLargeMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        //将转换成炸弹的GemItem类型重置
        bi.gem.IsBomb = BombType.none;
    }

    /// <summary>
    /// 处理横向消除炸弹
    /// </summary>
    /// <param name="row"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandleHorMerge(Vector2Int pos,GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        GemsItem g = null;
        //如果是横向消除则消除一行
        HashSet<Vector2Int> gems = new HashSet<Vector2Int>(GameCfg.col);
        List<MergeInfo> mergeInfos = new List<MergeInfo>();

        for (int i = 0; i < GameCfg.col; i++)
        {
            g = gemsItemsCollect[pos.x, i];
            if(g!= null && (g.IsBomb & (BombType.none | BombType.hor)) != 0)
            {
                gems.Add(g.Idx);
                this.AddMergeInfoToDic(pos,g);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// 处理纵向消除炸弹
    /// </summary>
    /// <param name="col"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandlerVerMerge(Vector2Int pos,GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        GemsItem g = null;
        //如果是纵向消除则消除一列
        HashSet<Vector2Int> gems = new HashSet<Vector2Int>(GameCfg.row);
        for (int i = 0; i < GameCfg.row; i++)
        {
            g = gemsItemsCollect[i, pos.y];
            if (g != null && (g.IsBomb & (BombType.none|BombType.ver)) != 0)
            {
                gems.Add(g.Idx);
                this.AddMergeInfoToDic(pos,g);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// 处理超级炸弹
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    /// <param name="bombItems"></param>
    void HandlerSuperMerge(Vector2Int pos, GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<BombItemInfo> bombItems, List<List<MergeInfo>> bombMergeInfos)
    {
        Vector2Int current;
        BombType bt = BombType.none;
        HashSet<Vector2Int> gems = new HashSet<Vector2Int>();
        GemsItem g = null;
        //检查四个方向有没有炸弹
        for (int i = 0;i < 4; i++)
        {
            current = pos + Utils.directions[i];
            // 边界检查
            if (current.x < 0 || current.x >= GameCfg.row) continue;
            if (current.y < 0 || current.y >= GameCfg.col) continue;
            g = gemsItemsCollect[current.x, current.y];
            if (g == null) continue;
            bt = g.IsBomb;
        }
        if(bt == BombType.super)
        {
            //如果有超级炸弹
            //将所有格子中没被销毁的物体加入到gemsItems中
            this.FindNotHaveDestroyItem(pos,gemsItemsCollect, gems);
        }
        else if(bt != BombType.none)
        {
            //如果有其他炸弹，则
            //随机一个类型
            //将此类型的所有物体加入到gemsItems中
            this.FindTarTypeItem(pos,Utils.randomAGemType(0, 5), gemsItemsCollect, gems);
        }
        else
        {
            //如果周围四个格子没有其他炸弹
            //随机炸掉哪一格的宝石,随机取此格子的一个方向,寻找一个没有被移除的物体作为要移除的物体

            int idx = -1;
            for (int i = 0; i < 4; i++)
            {
                idx = Random.Range(0, 4);
                current = pos + Utils.directions[Random.Range(0, 4)];
                if((current.x < 0 || current.x >= GameCfg.row) && (current.y < 0 || current.y >= GameCfg.col))
                {
                    g = gemsItemsCollect[current.x, current.y];
                    if (g != null && !g.IsRemove)
                    {
                        break;
                    }
                }
            }
            if(idx != -1)
            {
                //取这个格子中物体的GemType
                //获取这个类型的所有物体，加入到gesItems中
                this.FindTarTypeItem(pos, g.GemType, gemsItemsCollect, gems);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// 处理大炸弹
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandlerLargeMerge(Vector2Int pos, GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        Vector2Int current;
        HashSet<Vector2Int> gems = new HashSet<Vector2Int>();
        GemsItem g;
        //将周围四个格子的物体加入到gemsItems中
        for (int i = 0; i < 4; i++)
        {
            current = pos + Utils.directions[i];
            // 边界检查
            if (current.x < 0 || current.x >= GameCfg.row) continue;
            if (current.y < 0 || current.y >= GameCfg.col) continue;

            g = gemsItemsCollect[current.x, current.y];
            if(g!= null && g.IsBomb == BombType.none)
            {
                //如果当前物体不是炸弹才添加到消除物体
                gems.Add(g.Idx);
                this.AddMergeInfoToDic(pos,g);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// 查找棋盘上所有指定类型的元素，添加到移除列表中
    /// </summary>
    /// <param name="gemType">指定的类型</param>
    /// <param name="gemsItemsCollect">存储物体的数组</param>
    /// /// <param name="gemsItems">要消除物体的集合</param>
    void FindTarTypeItem(Vector2Int pos,int gemType, GemsItem[,] gemsItemsCollect, HashSet<Vector2Int> gems)
    {
        GemsItem g = null;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                g = gemsItemsCollect[i, j];
                //如果是与目标类型相同的Item则添加到移除列表
                if (g != null && (g.GemType & gemType) != 0)
                {
                    gems.Add(g.Idx);
                    this.AddMergeInfoToDic(pos,g);
                }
            }
        }
    }

    /// <summary>
    /// 查找目前没有被移除的所有物体，添加到移除列表中
    /// </summary>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void FindNotHaveDestroyItem(Vector2Int pos,GemsItem[,] gemsItemsCollect, HashSet<Vector2Int> gems)
    {
        GemsItem g = null;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for(int j = 0; j < GameCfg.col; j++)
            {
                g = gemsItemsCollect[i, j];
                //如果没有被移除，则添加到移除列表中
                if (g!= null && !g.IsRemove)
                {
                    gems.Add(g.Idx);
                    this.AddMergeInfoToDic(pos,g);
                }
            }
        }
    }

    void AddMergeInfoToDic(Vector2Int pos,GemsItem g)
    {
        MergeInfo mergeInfo;
        if (!mergeInfosDic.ContainsKey(g.Type))
        {
            mergeInfo = new MergeInfo { type = g.Type, num = 1, row = pos.x, col = pos.y };
            mergeInfosDic.Add(g.Type, mergeInfo);
        }
        else
        {
            mergeInfo = mergeInfosDic[g.Type];
            mergeInfo.num++;
            mergeInfosDic[g.Type] = mergeInfo;
        }
    }

    void DicDataToList(List<List<MergeInfo>> bombMergeInfos)
    {
        MergeInfo mergeInfo;
        List<MergeInfo> data = new List<MergeInfo>();
        for (int i = 0; i < 5; i++)
        {
            if (mergeInfosDic.TryGetValue(i,out mergeInfo))
            {
                data.Add(mergeInfo);
            }
        }
        bombMergeInfos.Add(data);
        mergeInfosDic.Clear();
    }
}
