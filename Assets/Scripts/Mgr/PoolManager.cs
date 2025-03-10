using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public PoolMono<GemsItem> gemsPool;
    public Dictionary<int, PoolMono<EffectItem>> EffectItemDic;
    public PoolMono<EffectTextItem> EffectTextPool;
    public PoolMono<EffectFlyItem> EffFlyItemPool;
    public PoolMono<LoopListItem> LoopListItemPool;
    public PoolMono<ScoreListItem> ScoreListItemPool;
    public PoolMono<Bomb> BombItemPool;
    public PoolMono<BombEffctItem> BombEffctPool;

    public override void OnInit()
    {
        base.OnInit();
        gemsPool = new PoolMono<GemsItem>();
        EffectItemDic = new Dictionary<int, PoolMono<EffectItem>>(){ 
           [1] = new PoolMono<EffectItem>(),
           [2] = new PoolMono<EffectItem>(),
           [3] = new PoolMono<EffectItem>(),
           [4] = new PoolMono<EffectItem>(),
           [5] = new PoolMono<EffectItem>(),
        };
        EffectTextPool = new PoolMono<EffectTextItem>();
        EffFlyItemPool = new PoolMono<EffectFlyItem>();
        LoopListItemPool = new PoolMono<LoopListItem>();
        ScoreListItemPool = new PoolMono<ScoreListItem>();
        BombItemPool = new PoolMono<Bomb>(2);
        BombEffctPool = new PoolMono<BombEffctItem>(2);
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
    }
}
