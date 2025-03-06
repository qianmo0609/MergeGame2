using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
    Animator ani;
    int id;

    public void OnInitInfo(GameObject effctObj,int id,Vector3 position)
    {
        ani = effctObj.GetComponent<Animator>();
        effctObj.transform.position = position;
        this.gameObject.SetActive(true);
        StartCoroutine(RecycleEffect());
        this.id = id;
    }

    IEnumerator RecycleEffect()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.position = new Vector3(10000, 10000, 0);
        this.gameObject.SetActive(false);
        this.TOPool();
    }

    protected virtual void TOPool()
    {
        PoolManager.Instance.EffectItemDic[id].putObjToPool(this);
    }
}
