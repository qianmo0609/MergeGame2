using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public void CreateEffectTextItem(int num,Vector3 ViewPos, Transform parent)
    {
        EffectTextItem et = ResManager.Instance.CreateGameObj<EffectTextItem>(GameObjEunm.effectTextItem);
        //������ֵ�ͳ�ʼλ��
        et.OnInitEffect(num, ViewPos, parent);
    }

    public void CreateEffectItem(int id,Vector3 position)
    {
        EffectItem ei = ResManager.Instance.CreateGameObj<EffectItem>(GameObjEunm.effectItem,id);
        ei.OnInitInfo(ei.gameObject, id, position);
    }

    public void CreateEffectbomb(int id,Vector3 position)
    {
        BombEffctItem ei = ResManager.Instance.CreateGameObj<BombEffctItem>(GameObjEunm.bombEffct);
        ei.OnInitInfo(ei.gameObject, id, position);
    }
}
