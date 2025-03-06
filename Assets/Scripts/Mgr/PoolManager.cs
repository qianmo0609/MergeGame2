using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public Pool<GemsItem> gemsPool;
    public Dictionary<int, Pool<EffectItem>> EffectItemDic;
    public Pool<EffectTextItem> EffectTextPool;
    public Pool<EffectFlyItem> EffFlyItemPool;
    public Pool<LoopListItem> LoopListItemPool;
    public Pool<ScoreListItem> ScoreListItemPool;
    public Pool<Bomb> BombItemPool;
    public Pool<BombEffctItem> BombEffctPool;
    public Pool<GameObject> BottomWall;

    public override void OnInit()
    {
        base.OnInit();
        gemsPool = new Pool<GemsItem>();
        EffectItemDic = new Dictionary<int, Pool<EffectItem>>(){ 
           [1] = new Pool<EffectItem>(),
           [2] = new Pool<EffectItem>(),
           [3] = new Pool<EffectItem>(),
           [4] = new Pool<EffectItem>(),
           [5] = new Pool<EffectItem>(),
        };
        EffectTextPool = new Pool<EffectTextItem>();
        EffFlyItemPool = new Pool<EffectFlyItem>();
        LoopListItemPool = new Pool<LoopListItem>();
        ScoreListItemPool = new Pool<ScoreListItem>();
        BombItemPool = new Pool<Bomb>(2);
        BombEffctPool = new Pool<BombEffctItem>(2);
        BottomWall = new Pool<GameObject>(20);
    }

    public void OnDestroy()
    {
        gemsPool.OnDestroy();
        EffectTextPool.OnDestroy();
        EffectItemDic.Clear();
        EffFlyItemPool.OnDestroy();
        LoopListItemPool.OnDestroy();
        ScoreListItemPool.OnDestroy();
        BombItemPool.OnDestroy();
        BombEffctPool.OnDestroy();
        BottomWall.OnDestroy();
    }
}
