using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public UIButton btnClose;

    private void Awake()
    {
        btnClose?.onClick.Add(new EventDelegate(this.Hide));
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide() 
    {
        this.gameObject.SetActive(false);
        UIManager.Instance.PutWindow(this);
    }

    public void OnDestroy()
    {
        btnClose?.onClick.Clear();
    }
}
