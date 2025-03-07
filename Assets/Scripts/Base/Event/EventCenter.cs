using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : Singleton<EventCenter>
{
    Dictionary<int, Action> evnetDic;

    public EventCenter()
    {
        evnetDic = new Dictionary<int, Action>();
    }

    public void RegisterEvent(int id,Action action)
    {
        Action callBc;
        if(!evnetDic.TryGetValue(id,out callBc))
        {
            evnetDic.Add(id,action);
        }
    }

    public void ExcuteEvent(int id)
    {
        Action callBc;
        if (evnetDic.TryGetValue(id, out callBc))
        {
            callBc();
        }
    }

    public void UnregisterEvent(int id)
    {
        if (this.evnetDic == null) return;
        Action callBc;
        if (evnetDic.TryGetValue(id, out callBc))
        {
            evnetDic.Remove(id);
        }
    }

    public void Disable()
    {
        this.evnetDic.Clear();
        this.evnetDic = null;
    }
}
