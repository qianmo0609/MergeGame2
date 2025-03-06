using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    //所有panel的父物体
    Transform m_UIRoot;
    //存储所有UIPanel
    Dictionary<string, UIBase> uiCollection;

    public Transform UIRoot { get => m_UIRoot; }

    public override void OnInit()
    {
        base.OnInit();
        uiCollection = new Dictionary<string, UIBase>();
        //加载UIRoot
        this.m_UIRoot = GameObject.Instantiate(ResManager.Instance.uiRootPrefab).transform;
    }

    public T GetWindow<T>() where T : UIBase
    {
        string key = typeof(T).Name;
        UIBase win;
        if (uiCollection.TryGetValue(key,out win))
        {
            uiCollection.Remove(key);
            return win as T;
        }
        else
        {
            if (ResManager.Instance.uiWinsPrefab.TryGetValue(key,out win))
            {
                UIBase ui = GameObject.Instantiate(win,this.m_UIRoot);
                uiCollection[key] = ui;
                return ui as T;
            }
            else
            {
                Debug.LogError("查询的窗口不存在！");
                return null;
            }
        }
    }

    public void PutWindow<T>(UIBase windows) where T : UIBase
    {
        string key = typeof(T).Name;
        UIBase win;
        if (!uiCollection.TryGetValue(key, out win))
        {
            win.transform.parent = null;
            uiCollection.Add(key, windows);
        }
        else
        {
            Debug.LogError("查询的窗口不存在！");
        }
    }

    public void OnDestroy()
    {
        uiCollection?.Clear();
    }
}
