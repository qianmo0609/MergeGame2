using DG.Tweening;
using System;
using UnityEngine;

public class EffectFlyItem : MonoBehaviour,IFlyComponent
{
    Action<MergeInfo> cb;

    Vector3 p0, p1, p2;

    bool isCanMove = false;

    // �����ƶ�����ʱ��
    public float duration = .5f;
    // ��¼��ʼ�ƶ���ʱ��
    private float startTime;

    MergeInfo mergeInfo;

    public bool IsCanMove { get => isCanMove; set => isCanMove = value; }

    public virtual void OnInitInfo(MergeInfo mergeInfo, Vector3 tartPos,Sprite sprite,Action<MergeInfo> cb,bool isMoveAtOnce = true)
    {
        Vector3 pos = Utils.GetNextPos(mergeInfo.row, mergeInfo.col);
        SpriteRenderer sp = this.GetComponent<SpriteRenderer>();
        sp.sprite = sprite;
        sp.sortingOrder = 3;
        this.transform.position = pos;
        this.transform.localScale = 0.6856104f * Vector3.one;
        this.mergeInfo = mergeInfo;
        this.cb = cb;
        p0 = pos;
        p1 = new Vector3(pos.x + tartPos.x / 2, pos.y + GameCfg.flyBezierOffsetY, 0);
        p2 = tartPos;
        this.isCanMove = isMoveAtOnce;
        // ��¼��ʼ�ƶ���ʱ��
        startTime = Time.time;
        //this.MovePath(pos,tartPos);
    }

    public virtual void Update()
    {
        if (isCanMove)
        {
            // ���㵱ǰʱ��ռ��ʱ��ı���
            float t = (Time.time - startTime) / duration;
            // ȷ��t��0��1�ķ�Χ��
            t = Mathf.Clamp01(t);
            this.transform.position = Utils.CalculateQuadraticBezierPoint(t,p0,p1,p2);
            if (Vector3.Distance(this.transform.position, p2) < 0.01f)
            {
                isCanMove = false;
                this.FlyComplete();
            }
        }
    }

    public void MovePath(Vector3 pos,Vector3 tartPos)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.transform.DOMove(new Vector3(pos.x+tartPos.x/2,pos.y + 2,0),0.3f));
        sequence.Append(this.transform.DOMove(tartPos, 0.3f).SetEase(Ease.OutQuad));
        sequence.Play().OnComplete(this.FlyComplete);
    }

    public void FlyComplete()
    {
        //������ɺ�ִ�лص�����
        this.cb?.Invoke(this.mergeInfo);
        this.RecycleSelf();
    }

    public virtual void RecycleSelf()
    {
        this.transform.position = new Vector3(10000, 10000, 0);
        PoolManager.Instance.EffFlyItemPool.putObjToPool(this);
    }
}
