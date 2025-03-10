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
/// ը�����ɹ�����
/// </summary>
public class BombManager
{
    Dictionary<int, MergeInfo> mergeInfosDic; //���ڼӿ��ѯ����ķ�����Ϣ

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
            //4�������һ���͵ڶ�����x��y���ж��Ǻ���������
            if (gemsItem[0].x == gemsItem[1].x)
            {
                //�������ȣ�������Ǻ�����ĸ�
                //����Ǻ�������������������ը��
                //Debug.Log("���������ĸ�ը��");
                gemsItemCollection[pos.x,pos.y].IsBomb = BombType.ver;
                isVer = true;
            }
            else
            {
                //Debug.Log("���������ĸ�ը��");
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.hor;
                isVer = false;
            }
            bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x,pos.y], bombType = isVer ?BombType.ver:BombType.hor});
        }
        else if (count >= 5)
        {
            Vector2Int pos;
            //������ڵ��������������ֱ���͡�T���͡�L���͡�ʮ���͡�����������״
            if (this.IsLine5(gemsItem))
            {
                //���ɳ���ը��
                //Debug.Log("���ɳ���ը��");
                pos = gemsItem[count / 2];
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.super;
                bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x, pos.y], bombType = BombType.super });
            }
            else if(this.IsTShape(gemsItem) || this.IsLShape(gemsItem) || this.IsCrossShape(gemsItem))
            {
                //���ɴ�ը��
                //Debug.Log("���ɴ�ը��");
                pos = gemsItem[count / 2];
                gemsItemCollection[pos.x, pos.y].IsBomb = BombType.large;
                bombItems.Add(new BombItemInfo { gem = gemsItemCollection[pos.x, pos.y], bombType = BombType.large });
            }
            //������״����Ҫ����
        }
        //����������Ҳ����Ҫ����
    }

    public bool IsLine5(Vector2Int[] gemsItem)
    {
        //ֻҪ����5����������һ����ը��
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
    /// ���T���ͺ�ʮ����
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

            //ǰ����T���ж�
            if ((upCount + downCount >= 2) && ((leftCount >= 2 && rightCount >= 0 && rightCount < 2) || (leftCount >= 0 && leftCount < 2 && rightCount >= 2)))
            {
                return true;
            }

            //������T���ж�
            if ((leftCount + rightCount >= 2) && ((upCount >= 2 && downCount >= 0 && downCount < 2) || (upCount >= 0 && upCount < 2 && downCount >= 2)))
            {
                return true;
            }


            //ʮ�����ж�
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
                ��   ��      ������    ��          0     0      0 0 0     0  
            ������   ������    ��      ��       0000     0000     0       0
                ��   ��        ��    ������        0     0        0       0
                                                                 0     0 0 0  ....
         
         */
        //����ĵ�洢��˳����ǰ���������ϵĵ�����������ϵĵ�
        //ֻҪ����T�;����ɴ�ը��

        //����ÿ������Ϊ���ĵ㣬ͳ�Ƹ����ĵ��������ҷ������ͨ������������ֱ�������£������� 2 ����ͨ�㣬
        //��ˮƽ�������ң�������һ�������� 2 ����ͨ�㣬���ж�Ϊ T �͡�
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

            //ǰ����T���ж�
            if ((upCount + downCount >= 2) && ((leftCount >= 2 && rightCount >= 0 && rightCount < 2) || (leftCount >= 0 && leftCount < 2 && rightCount >= 2)))
            {
                return true;
            }

            //������T���ж�
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
            ��          ��       
            ��          ��
            ������  ������   ....
       */
        //�������Ͽ��ܲ�ֹ������
        //�����������������㣬�ᱻ�ж�Ϊ��������ը��
        //ֻҪ����L�;����ɴ�ը��
        if (gemsItem.Length > 6) return false;
        //ͨ��˫��ѭ��������ԣ����ڴ���ͬһ�л�ͬһ�еĵ�ԣ��ֱ�ͳ��ˮƽ�ʹ�ֱ�������ͨ��������
        //��ˮƽ�ʹ�ֱ���������� 3 ����ͨ�㣬���ж�Ϊ L ��
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
    /// �ж��Ƿ�Ϊʮ����
    /// </summary>
    /// <param name="gemsItem"></param>
    /// <returns></returns>
    public bool IsCrossShape(Vector2Int[] gemsItem)
    {
        /*
              ��
            ������
              ��
        */
        //�������Ͽ��ܲ�ֹ�����㣬�������ĸ��㡢�����ߡ��ˡ���
        //�����������������㣬�ᱻ�ж�Ϊ��������ը��
        //ֻҪ����ʮ���;����ɴ�ը��

        //����ÿ������Ϊ���ĵ㣬ͳ�Ƹ����ĵ��������ҷ������ͨ��������
        //�����º����ҷ���ֱ������� 2 ����ͨ�㣨�������ĵ�͹������� 3 ��Ԫ�أ������ж�Ϊʮ����
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
            //����ը������
            this.HandleHorMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        else if (bi.bombType == BombType.ver)
        {
            //����ը������
            this.HandlerVerMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        else if (bi.bombType == BombType.super)
        {
            //����ը������
            this.HandlerSuperMerge(bi.gem.Idx, gemsItemsCollect, gemsItems,bombItems, bombMergeInfos);
        }
        else if(bi.bombType == BombType.large)
        {
            //��ը������
            this.HandlerLargeMerge(bi.gem.Idx, gemsItemsCollect, gemsItems, bombMergeInfos);
        }
        //��ת����ը����GemItem��������
        bi.gem.IsBomb = BombType.none;
    }

    /// <summary>
    /// �����������ը��
    /// </summary>
    /// <param name="row"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandleHorMerge(Vector2Int pos,GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        GemsItem g = null;
        //����Ǻ�������������һ��
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
    /// ������������ը��
    /// </summary>
    /// <param name="col"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandlerVerMerge(Vector2Int pos,GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        GemsItem g = null;
        //�������������������һ��
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
    /// ������ը��
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
        //����ĸ�������û��ը��
        for (int i = 0;i < 4; i++)
        {
            current = pos + Utils.directions[i];
            // �߽���
            if (current.x < 0 || current.x >= GameCfg.row) continue;
            if (current.y < 0 || current.y >= GameCfg.col) continue;
            g = gemsItemsCollect[current.x, current.y];
            if (g == null) continue;
            bt = g.IsBomb;
        }
        if(bt == BombType.super)
        {
            //����г���ը��
            //�����и�����û�����ٵ�������뵽gemsItems��
            this.FindNotHaveDestroyItem(pos,gemsItemsCollect, gems);
        }
        else if(bt != BombType.none)
        {
            //���������ը������
            //���һ������
            //�������͵�����������뵽gemsItems��
            this.FindTarTypeItem(pos,Utils.randomAGemType(0, 5), gemsItemsCollect, gems);
        }
        else
        {
            //�����Χ�ĸ�����û������ը��
            //���ը����һ��ı�ʯ,���ȡ�˸��ӵ�һ������,Ѱ��һ��û�б��Ƴ���������ΪҪ�Ƴ�������

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
                //ȡ��������������GemType
                //��ȡ������͵��������壬���뵽gesItems��
                this.FindTarTypeItem(pos, g.GemType, gemsItemsCollect, gems);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// �����ը��
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gemsItemsCollect"></param>
    /// <param name="gemsItems"></param>
    void HandlerLargeMerge(Vector2Int pos, GemsItem[,] gemsItemsCollect, List<HashSet<Vector2Int>> gemsItems, List<List<MergeInfo>> bombMergeInfos)
    {
        Vector2Int current;
        HashSet<Vector2Int> gems = new HashSet<Vector2Int>();
        GemsItem g;
        //����Χ�ĸ����ӵ�������뵽gemsItems��
        for (int i = 0; i < 4; i++)
        {
            current = pos + Utils.directions[i];
            // �߽���
            if (current.x < 0 || current.x >= GameCfg.row) continue;
            if (current.y < 0 || current.y >= GameCfg.col) continue;

            g = gemsItemsCollect[current.x, current.y];
            if(g!= null && g.IsBomb == BombType.none)
            {
                //�����ǰ���岻��ը������ӵ���������
                gems.Add(g.Idx);
                this.AddMergeInfoToDic(pos,g);
            }
        }
        gems.Add(pos);
        gemsItems.Add(gems);
        this.DicDataToList(bombMergeInfos);
    }

    /// <summary>
    /// ��������������ָ�����͵�Ԫ�أ���ӵ��Ƴ��б���
    /// </summary>
    /// <param name="gemType">ָ��������</param>
    /// <param name="gemsItemsCollect">�洢���������</param>
    /// /// <param name="gemsItems">Ҫ��������ļ���</param>
    void FindTarTypeItem(Vector2Int pos,int gemType, GemsItem[,] gemsItemsCollect, HashSet<Vector2Int> gems)
    {
        GemsItem g = null;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                g = gemsItemsCollect[i, j];
                //�������Ŀ��������ͬ��Item����ӵ��Ƴ��б�
                if (g != null && (g.GemType & gemType) != 0)
                {
                    gems.Add(g.Idx);
                    this.AddMergeInfoToDic(pos,g);
                }
            }
        }
    }

    /// <summary>
    /// ����Ŀǰû�б��Ƴ����������壬��ӵ��Ƴ��б���
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
                //���û�б��Ƴ�������ӵ��Ƴ��б���
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
