using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : UIBase
{
    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.Instance.PutWindow<MainUI>(this);
    }
}
