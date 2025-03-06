using UnityEngine;

public class CreateFactory : Singleton<CreateFactory>
{
    public override void OnInit()
    {
        base.OnInit();
    }

    public T CreateGameObj<T>(GameObjEunm type,int id = 0) where T: class
    {
        switch (type)
        {
            case GameObjEunm.gemItem:
                GemsItem gemItem = PoolManager.Instance.gemsPool.getObjFromPool();
                if (gemItem == null)
                    gemItem = GameObject.Instantiate(ResManager.Instance.slotPrefab).AddComponent<GemsItem>();
                return gemItem as T;
            case GameObjEunm.effectTextItem:
                EffectTextItem et = PoolManager.Instance.EffectTextPool.getObjFromPool();
                if (et == null)
                    et = GameObject.Instantiate(ResManager.Instance.effectTextItems);
                return et as T;
            case GameObjEunm.effectItem:
                EffectItem ei = PoolManager.Instance.EffectItemDic[id].getObjFromPool();
                if (ei == null)
                    ei = GameObject.Instantiate(ResManager.Instance.effcts[id - 1]).AddComponent<EffectItem>();
                return ei as T;
            case GameObjEunm.effectFlyItem:
                EffectFlyItem ef = PoolManager.Instance.EffFlyItemPool.getObjFromPool();
                if (ef == null)
                    ef = GameObject.Instantiate(ResManager.Instance.slotPrefab).AddComponent<EffectFlyItem>();
                return ef as T;
            case GameObjEunm.loopListItem:
                LoopListItem ll = PoolManager.Instance.LoopListItemPool.getObjFromPool();
                if (ll == null)
                    ll = GameObject.Instantiate(ResManager.Instance.slotPrefab).AddComponent<LoopListItem>();
                return ll as T;
            case GameObjEunm.scoreListItem:
                ScoreListItem sl = PoolManager.Instance.ScoreListItemPool.getObjFromPool();
                if (sl == null)
                    sl= GameObject.Instantiate<ScoreListItem>(ResManager.Instance.scoreListItem);
                return sl as T;
            case GameObjEunm.bomb:
                Bomb b = PoolManager.Instance.BombItemPool.getObjFromPool();
                if (b == null)
                    b = GameObject.Instantiate(ResManager.Instance.slotPrefab).AddComponent<Bomb>();
                return b as T;
            case GameObjEunm.bombEffct:
                BombEffctItem be = PoolManager.Instance.BombEffctPool.getObjFromPool();
                if (be == null)
                    be = GameObject.Instantiate(ResManager.Instance.effectBomb).AddComponent<BombEffctItem>();
                return be as T;
            default:
                return null;
        }
    }

 
}
