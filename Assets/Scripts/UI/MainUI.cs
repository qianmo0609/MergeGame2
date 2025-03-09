using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] UIButton btnStart;
    [SerializeField] UIButton btnHandUp;

    private void Start()
    {
        btnStart.onClick.Add(new EventDelegate(OnStartEvent));
        btnHandUp.onClick.Add(new EventDelegate(OnHandUpEvent));
    }

    private void OnStartEvent()
    {
        if (!GameCfg.isHandUp)
        {
            EventCenter.Instance.ExcuteEvent(EventNum.startEvent);
        }
        else
        {
            btnStart.normalSprite = ConstValue.btnStartNormalSpriteName;
            btnHandUp.normalSprite = ConstValue.btnHandUpNormalSpriteName;
            GameCfg.isHandUp = false;
        }
    }
    private void OnHandUpEvent()
    {
        if (!GameCfg.isHandUp)
        {
            GameCfg.isHandUp = true;
            EventCenter.Instance.ExcuteEvent(EventNum.startEvent);
            btnStart.normalSprite = ConstValue.btnStartHandUpSpriteName;
            btnHandUp.normalSprite = ConstValue.btnHandUpHandUpSpriteName;
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
