using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCtl 
{
    Transform grid;
    public GemsItem[,] gemsItemsCollect; //游戏中的宝石数据
    public int[,] mapFlag; //用于标记消除位置的物体类型

    public GemCtl(Transform grid)
    {
        gemsItemsCollect = new GemsItem[GameCfg.row, GameCfg.col];
        mapFlag = new int[GameCfg.row, GameCfg.col];
        this.grid = grid;
    }

    public GemsItem CreateOneGemItem(Vector3 pos, DirEnum dir, Vector2Int idx, bool isCreateBomb = false)
    {
        GemsItem gemItem = ResManager.Instance.CreateGameObj<GemsItem>(GameObjEunm.gemItem);
        gemItem.transform.SetParent(this.grid.transform);
        gemItem.transform.position = pos;
        if (ResManager.Instance.gemsSprites.Length > 0)
        {
            //需要按概率生成
            int spriteIdx = Utils.getGemsIdx(Utils.RandomIntVale(0, 10001));
            gemItem.OnInitInfo(ResManager.Instance.gemsSprites[spriteIdx], spriteIdx, dir, idx, BombType.none);
        }
        gemItem.TweenTOPosition(isDelay: true);
        return gemItem;
    }

    public void GemClear()
    {
        GemsItem g;
        for (int i = 0; i < gemsItemsCollect.GetLength(0); i++)
        {
            for (int j = 0; j < gemsItemsCollect.GetLength(1); j++)
            {
                if (Utils.IsCorner(new Vector2Int(i, j))) continue;
                g = gemsItemsCollect[i, j];
                g.IsFull = true;
            }
        }
        //掉落完成之后要清除这部分宝石
        System.Array.Clear(gemsItemsCollect, 0, gemsItemsCollect.Length);
    }

    /// <summary>
    /// 检查3x3
    /// </summary>+
    /// <returns></returns>
    public bool CheckThree()
    {
        bool isMatch = false;
        GemsItem g1, g2, g3;
        //横向检测
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col - 2; j++)
            {
                //因为存储是从左下角开始存储的，所以从头遍历是oK的
                g1 = gemsItemsCollect[i, j];
                g2 = gemsItemsCollect[i, j + 1];
                g3 = gemsItemsCollect[i, j + 2];
                if (g1 != null && g2 != null && g3 != null)
                {
                    if ((g1.GemType & g2.GemType & g3.GemType) != 0)
                    {
                        mapFlag[g1.Idx.x, g1.Idx.y] = g1.Type + 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = g1.Type + 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = g1.Type + 1;
                        isMatch = true;
                    }
                }
            }
        }

        //纵向检测
        for (int i = 0; i < GameCfg.row - 2; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                g1 = gemsItemsCollect[i, j];
                g2 = gemsItemsCollect[i + 1, j];
                g3 = gemsItemsCollect[i + 2, j];
                if (g1 != null && g2 != null && g3 != null)
                {
                    if ((g1.GemType & g2.GemType & g3.GemType) != 0)
                    {
                        mapFlag[g1.Idx.x, g1.Idx.y] = g1.Type + 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = g1.Type + 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = g1.Type + 1;
                        isMatch = true;
                    }
                }
            }
        }
        //this.DisplayArray();
        return isMatch;
    }

    public GemsItem GetGemItem(int x,int y)
    {
        return gemsItemsCollect[x, y];
    }

    public HashSet<Vector2Int> FindMatches(int x, int y, int tarType)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 初始化队列
        queue.Enqueue(new Vector2Int(x, y));
        mapFlag[x, y] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            matches.Add(current);

            foreach (Vector2Int dir in Utils.directions)
            {
                Vector2Int next = current + dir;

                // 边界检查
                if (next.x < 0 || next.x >= GameCfg.row) continue;
                if (next.y < 0 || next.y >= GameCfg.col) continue;

                // 类型检查 && 访问标记 
                if (mapFlag[next.x, next.y] == tarType)
                {
                    mapFlag[next.x, next.y] = 0;
                    queue.Enqueue(new Vector2Int(next.x, next.y));
                }
            }
        }
        return matches;
    }

    public void ClearFlag()
    {
        System.Array.Clear(mapFlag, 0, mapFlag.Length);
    }

    public void MergeMove(int x, int y,Action<int, int, Vector3, bool> ReplenishGem)
    {
        int xIdx = x;
        //如g是第二行的元素，要从第三行、第四行将元素下移
        for (int i = x; i > 0; i--)
        {
            if (Utils.IsCorner(new Vector2Int(i, y))) continue;
            //得到上一行的GemItem
            GemsItem g1 = gemsItemsCollect[i - 1, y];
            if (g1 == null)
            {
                ReplenishGem(i, y, Utils.GetStartPos(0, y), false);
                continue;
            }
            //将得到的GemItem赋值给下一行
            gemsItemsCollect[i, y] = g1;
            g1.Idx = new Vector2Int(xIdx, y);
            //清空原位置数据
            gemsItemsCollect[i - 1, y] = null;
            g1.TweenTOPosition();
            xIdx--;
        }
    }

    /// <summary>
    /// 打印标记数组显示
    /// </summary>
    void DisplayArray()
    {
        string s = "";
        for (int j = 0; j < GameCfg.row; j++)
        {
            for (int i = 0; i < GameCfg.col; i++)
            {
                s += mapFlag[j, i];
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}
