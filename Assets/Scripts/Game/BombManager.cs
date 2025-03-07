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
    public HashSet<GemsItem> gems; //���ڴ��浱ǰ������λ�õ���������
    public BombType bombType;   //�����ƶ�����״������ʲôը������
}

/// <summary>
/// ը�����ɹ�����
/// </summary>
public class BombManager
{
    public BombItemInfo? SwitchBomb(int count,GameDir dir,GemsItem[] gemsItem)
    {
        if (count == 4)
        {
            if(dir == GameDir.Hor)
            {
                //����Ǻ�������������������ը��
                Debug.Log("���������ĸ�ը��");
                gemsItem[1].IsBomb = BombType.ver;
            } 
            else
            {
                Debug.Log("���������ĸ�ը��");
                gemsItem[1].IsBomb = BombType.hor;
            }
            return new BombItemInfo { pos = gemsItem[1].Idx, bombType = dir == GameDir.Hor?BombType.ver:BombType.hor};
        }
        else if (count == 5)
        {
            //���ɳ���ը��
            Debug.Log("���ɳ���ը��");
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
        //��ת����ը����GemItem��������
        gemsItems[bi.pos.x, bi.pos.y].IsBomb = BombType.none;
        bombItems.RemoveAt(0);
    }

    //�����������
    void HandleHorMerge(int row,GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {
        //����Ǻ�������������һ��
        for (int i = 0; i < GameCfg.col; i++)
        {
            mergeCollection.Add(gemsItems[row, i]);
        }
    }
    //������������
    void HandlerVerMerge(int col,GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {
        //�������������������һ��
        for (int i = 0; i < GameCfg.row; i++)
        {
            mergeCollection.Add(gemsItems[i, col]);
        }
    }
    //������ը��
    void HandlerSuperMerge(GemsItem[,] gemsItems, HashSet<GemsItem> mergeCollection)
    {

    }
}
