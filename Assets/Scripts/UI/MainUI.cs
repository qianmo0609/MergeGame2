using DG.Tweening;
using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] UIButton btnStart;
    [SerializeField] UIButton btnHandUp;
    [SerializeField] UILabel comboLable;

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

    private void OnShowComboLabel()
    {
        comboLable.gameObject.SetActive(true);
        comboLable.transform.DOPunchScale(Vector3.one * 1.2f,.4f);
        //comboLable.text = ;
    }

    private void OnHideComboLabel()
    {
        comboLable.gameObject.SetActive(false);
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
