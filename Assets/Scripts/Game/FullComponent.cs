using System;
using UnityEngine;

public class FullComponent
{
    float vv = 3; //重力方向速度
    float vh = 0; //水平方向速度

    Transform ctlObj = null;

    AnimationCurve curve;
    float t = 0;
    float multy;

    public FullComponent(Transform ctlObj)
    {
        this.ctlObj = ctlObj;
        curve = CurveManager.Instance.gravityAnimationCurve;
    }

    public void UpdateInfo()
    {
        vh = Utils.RandomFloatVale(-5, 5);
        vv = 7;
        multy = UnityEngine.Random.Range(3.0f,8.0f);
    }

    public void Update(Action cb)
    {
        if (this.ctlObj == null) return;
        t += Time.deltaTime * 2f;
        vv -= curve.Evaluate(t) * this.multy;
        this.ctlObj.transform.position += new Vector3(vh, vv, 0) * Time.deltaTime;

        if (this.ctlObj.transform.position.y < -10)
        {
            cb?.Invoke();
            t = 0;
        }
    }
}
