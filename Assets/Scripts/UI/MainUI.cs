using DG.Tweening;
using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] UIButton btnStart;
    [SerializeField] UIButton btnHandUp;
    [SerializeField] UILabel comboLable;
    [SerializeField] UISprite comboSprite;

    private void Start()
    {
        btnStart.onClick.Add(new EventDelegate(OnStartEvent));
        btnHandUp.onClick.Add(new EventDelegate(OnHandUpEvent));
        EventCenter.Instance.RegisterEvent(EventNum.ShowComboLabel,this.OnShowComboLabel);
        EventCenter.Instance.RegisterEvent(EventNum.HideComboLabel,this.OnHideComboLabel);
        EventCenter.Instance.RegisterEvent(EventNum.EnableOrDisableBtnStartEvent, this.EnableOrDisableStartBtn);
    }

    private void OnStartEvent()
    {
        if (!GameCfg.isHandUp)
        {
            EventCenter.Instance.ExcuteEvent(EventNum.StartEvent);
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
            EventCenter.Instance.ExcuteEvent(EventNum.StartEvent);
            btnStart.normalSprite = ConstValue.btnStartHandUpSpriteName;
            btnHandUp.normalSprite = ConstValue.btnHandUpHandUpSpriteName;
        }
    }

    private void OnShowComboLabel()
    {
        comboLable.transform.DOComplete();
        comboLable.gameObject.SetActive(true);
        comboSprite.gameObject.SetActive(true);
        comboLable.transform.DOPunchScale(Vector3.one * 2f,.4f,vibrato:0, elasticity:0);
        comboLable.text = GameCfg.comboNum.ToString();
    }

    private void OnHideComboLabel()
    {
        comboLable.gameObject.SetActive(false);
        comboSprite.gameObject.SetActive(false);
    }

    void EnableOrDisableStartBtn()
    {
        if (GameCfg.isHandUp) return;
        this.btnStart.enabled = GameCfg.isEnableBtnStart;
        this.btnStart.defaultColor = GameCfg.isEnableBtnStart ? Color.white : Color.gray;
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
