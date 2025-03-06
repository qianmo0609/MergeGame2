using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool<T> where T :class
{
    Stack<T> gameObjs;

    int maxCapacity = 1000;

    public Pool(int capacity = 1000)
    {
        gameObjs = new Stack<T>(capacity);
    }

    public T getObjFromPool()
    {
        if(gameObjs.Count <= 0)
        {
#if UNITY_EDITOR
            //Debug.Log("��ǰ����û�ж���");
#endif
            return null;
        }
        return gameObjs.Pop();
    }

    public void putObjToPool(T obj)
    {
        if(gameObjs.Count <= this.maxCapacity)
        {
            gameObjs.Push(obj);
        }
        else
        {
#if UNITY_EDITOR
            //Debug.Log("������������޷�����");
#endif
        }
    }

    public void OnDestroy()
    {
        gameObjs.Clear();
    }
}
