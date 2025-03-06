using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffctItem : EffectItem
{
    protected override void TOPool()
    {
        PoolManager.Instance.BombEffctPool.putObjToPool(this);
    }
}
