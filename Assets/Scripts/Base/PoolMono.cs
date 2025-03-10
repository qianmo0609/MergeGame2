using System.Collections.Generic;
using UnityEngine;

public class PoolMono<T> where T : MonoBehaviour
{
    Stack<T> gameObjs;

    int maxCapacity = 1000;

    public PoolMono(int capacity = 1000)
    {
        gameObjs = new Stack<T>(capacity);
    }

    public T getObjFromPool(GameObjEunm type, int id = 0) 
    {
        if(gameObjs.Count <= 0)
        {
#if UNITY_EDITOR
            //Debug.Log("当前池中没有对象");
#endif      
            return ResManager.Instance.InstantiateMonoObj<T>(type,id);
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
