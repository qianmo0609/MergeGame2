using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EffectTextItem : MonoBehaviour
{
    public UILabel label;

    public void OnInitEffect(int num, Vector3 ViewPos,Transform parent)
    {
        label.text = num.ToString();
        this.transform.parent = parent;
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = ViewPos;
        //做移动的动画
        DOTween.To(() => this.label.alpha, (value) => this.label.alpha = value, 1, 0.3f);
        this.transform.DOMoveX((ViewPos.x + 1), 1.5f).SetEase(Ease.OutQuad).OnComplete(MoveEffectText);
        DOTween.To(() => this.label.alpha, (value) => this.label.alpha = value, 0, 0.5f).SetDelay(0.7f);
    }

    void MoveEffectText()
    {
        this.transform.parent = null;
        this.transform.position = new Vector3(10000, 10000, 0);
        //PoolManager.Instance.EffectTextPool.putObjToPool(this);
        ResManager.Instance.PutObjToPool<EffectTextItem>(GameObjEunm.effectTextItem,this);
    }
}
