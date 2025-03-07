using System.Collections.Generic;
using UnityEngine;

public enum GameDir
{
    Hor,
    Ver
}

public enum BombType
{
    none,
    hor,
    ver,
    super,
    large,
}

public struct BombItemInfo
{
    public Vector2Int pos;
    public BombType bombType;
}

public struct GemsCollectionInfo
{
    public HashSet<GemsItem> gems; //用于储存当前的消除位置的所有物体
    public BombType bombType;   //用于制定此形状可生成什么炸弹类型
}

/// <summary>
/// 炸弹生成管理类
/// </summary>
public class BombManager
{
    public BombItemInfo? SwitchBomb(int count,GameDir dir,GemsItem[] gemsItem)
    {
        if (count == 4)
        {
            if(dir == GameDir.Hor)
            {
                //如果是横向消除，则生成竖向炸弹
                Debug.Log("竖向消除四个炸弹");
                gemsItem[1].IsBomb = BombType.ver;
            } 
            else
            {
                Debug.Log("横向消除四个炸弹");
                gemsItem[1].IsBomb = BombType.hor;
            }
            return new BombItemInfo { pos = gemsItem[1].Idx, bombType = dir == GameDir.Hor?BombType.ver:BombType.hor};
        }
        else if (count == 5)
        {
            //生成超级炸弹
            Debug.Log("生成超级炸弹");
            gemsItem[count / 2].IsBomb = BombType.super;
            return new BombItemInfo { pos = gemsItem[count/2].Idx, bombType = dir == GameDir.Hor ? BombType.ver : BombType.hor };
        }
        return null;
    }

    public void HandlerBomb(BombItemInfo bi, GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection, List<BombItemInfo> bombItems)
    {
        if (bi.bombType == BombType.hor)
        {
            this.HandleHorMerge(bi.pos.x, gemsItems, mergeCollection);
        }
        else if (bi.bombType == BombType.ver)
        {
            this.HandlerVerMerge(bi.pos.y, gemsItems, mergeCollection);
        }
        else if(bi.bombType == BombType.super)
        {

        }
        //将转换成炸弹的GemItem类型重置
        gemsItems[bi.pos.x, bi.pos.y].IsBomb = BombType.none;
        bombItems.RemoveAt(0);
    }

    //处理横向消除
    void HandleHorMerge(int row,GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {
        //如果是横向消除则消除一行
        for (int i = 0; i < GameCfg.col; i++)
        {
            mergeCollection.Add(gemsItems[row, i]);
        }
    }
    //处理纵向消除
    void HandlerVerMerge(int col,GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {
        //如果是纵向消除则消除一列
        for (int i = 0; i < GameCfg.row; i++)
        {
            mergeCollection.Add(gemsItems[i, col]);
        }
    }
    //处理超级炸弹
    void HandlerSuperMerge(GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {

    }
}
