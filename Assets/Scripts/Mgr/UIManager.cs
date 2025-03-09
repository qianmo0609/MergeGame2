using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    //����panel�ĸ�����
    Transform m_UIRoot;
    //�洢����UIPanel
    Dictionary<string, UIBase> uiCollection;

    public Transform UIRoot { get => m_UIRoot; }

    public override void OnInit()
    {
        base.OnInit();
        uiCollection = new Dictionary<string, UIBase>();
        //����UIRoot
        this.m_UIRoot = GameObject.Instantiate(ResManager.Instance.uiRootPrefab).transform;
    }

    public T GetWindow<T>() where T : UIBase
    {
        string key = typeof(T).Name;
        UIBase win;
        if (uiCollection.TryGetValue(key,out win))
        {
            win.gameObject.SetActive(true);
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
                Debug.LogError("��ѯ�Ĵ��ڲ����ڣ�");
                return null;
            }
        }
    }

    public void PutWindow(UIBase windows)
    {
        string key = windows.GetType().Name;
        UIBase win;
        if (uiCollection.TryGetValue(key, out win))
        {
            win.transform.parent = null;
            win.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("��ѯ�Ĵ��ڲ����ڣ�");
        }
    }

    public void OnDestroy()
    {
        uiCollection?.Clear();
    }
}
