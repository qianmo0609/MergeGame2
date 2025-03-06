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
            //Debug.Log("当前池中没有对象");
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
            //Debug.Log("对象池已满，无法放入");
#endif
        }
    }

    public void OnDestroy()
    {
        gameObjs.Clear();
    }
}
