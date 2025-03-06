using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public void CreateEffectTextItem(int num,Vector3 ViewPos, Transform parent)
    {
        EffectTextItem et = CreateFactory.Instance.CreateGameObj<EffectTextItem>(GameObjEunm.effectTextItem);
        //传递数值和初始位置
        et.OnInitEffect(num, ViewPos, parent);
    }

    public void CreateEffectItem(int id,Vector3 position)
    {
        EffectItem ei = CreateFactory.Instance.CreateGameObj<EffectItem>(GameObjEunm.effectItem,id);
        ei.OnInitInfo(ei.gameObject, id, position);
    }

    public void CreateEffectbomb(int id,Vector3 position)
    {
        BombEffctItem ei = CreateFactory.Instance.CreateGameObj<BombEffctItem>(GameObjEunm.bombEffct);
        ei.OnInitInfo(ei.gameObject, id, position);
    }
}
