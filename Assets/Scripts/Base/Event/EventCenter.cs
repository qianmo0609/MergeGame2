using System;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public Delegate Action;
    public bool IsOnce;
    public MonoBehaviour Target;
}

public class EventCenter : Singleton<EventCenter>
{
    private Dictionary<Type, Dictionary<string, List<EventData>>> _eventDict =
            new Dictionary<Type, Dictionary<string, List<EventData>>>();

    #region Settings
    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode = false;
    [SerializeField] private Color _logColor = Color.cyan;
    #endregion

    #region Core Methods
    /// <summary>
    /// 添加事件监听（带对象生命周期绑定）
    /// </summary>
    public void AddListener<T>(string eventKey, Action<T,T,T> callback,
                              MonoBehaviour target = null, bool isOnce = false)
    {
        Type eventType = typeof(T);
        ValidateEventKey(eventKey);

        lock (_eventDict)
        {
            if (!_eventDict.ContainsKey(eventType))
                _eventDict[eventType] = new Dictionary<string, List<EventData>>();

            if (!_eventDict[eventType].ContainsKey(eventKey))
                _eventDict[eventType][eventKey] = new List<EventData>();

            _eventDict[eventType][eventKey].Add(new EventData
            {
                Action = callback,
                IsOnce = isOnce,
                Target = target
            });

            Log($"Added listener for [{eventKey}] ({eventType.Name})");
        }
    }

    /// <summary>
    /// 触发事件（带参数校验）
    /// </summary>
    public void TriggerEvent<T>(string eventKey, T eventData)
    {
        Type eventType = typeof(T);
        ValidateEventKey(eventKey);

        List<EventData> validListeners = new List<EventData>();

        lock (_eventDict)
        {
            if (!_eventDict.ContainsKey(eventType)) return;
            if (!_eventDict[eventType].ContainsKey(eventKey)) return;

            foreach (var data in _eventDict[eventType][eventKey])
            {
                // 检查目标对象是否已销毁
                if (data.Target != null && data.Target == null) continue;

                validListeners.Add(data);
            }

            // 清理一次性监听
            _eventDict[eventType][eventKey].RemoveAll(data => data.IsOnce);
        }

        // 实际触发调用（在锁外执行）
        foreach (var data in validListeners)
        {
            try
            {
                (data.Action as Action<T>)?.Invoke(eventData);
                Log($"Triggered [{eventKey}] with {eventData}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Event Error: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 移除特定监听（精确匹配）
    /// </summary>
    public void RemoveListener<T>(string eventKey, Action<T> callback)
    {
        Type eventType = typeof(T);

        lock (_eventDict)
        {
            if (!_eventDict.ContainsKey(eventType)) return;
            if (!_eventDict[eventType].ContainsKey(eventKey)) return;

            _eventDict[eventType][eventKey].RemoveAll(data =>
                data.Action.Equals(callback));
        }
    }

    /// <summary>
    /// 清空所有事件（场景切换时自动调用）
    /// </summary>
    public void ClearAllEvents()
    {
        lock (_eventDict)
        {
            _eventDict.Clear();
            Log("All events cleared");
        }
    }
    #endregion

    #region Utilities
    private void ValidateEventKey(string eventKey)
    {
        if (string.IsNullOrEmpty(eventKey))
            throw new ArgumentException("Event key cannot be null or empty");
    }

    private void Log(string message)
    {
        if (_debugMode)
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(_logColor)}>[EventCenter]</color> {message}");
    }

    private void OnDestroy()
    {
        ClearAllEvents();
    }
    #endregion

    #region Extension Methods
    // 无参数版本
    public void AddListener(string eventKey, Action callback,
                          MonoBehaviour target = null, bool isOnce = false)
    {
        AddListener<object>(eventKey, (a,b,c)=> callback?.Invoke(), target, isOnce);
    }

    public void TriggerEvent(string eventKey)
    {
        TriggerEvent<object>(eventKey, null);
    }

    public void RemoveListener(string eventKey, Action callback)
    {
        RemoveListener<object>(eventKey, _ => callback?.Invoke());
    }
    #endregion

    public void Disable()
    {

    }
}
