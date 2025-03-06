using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
   void Start()
    {
        Application.targetFrameRate = 60;
        EventCenter.Instance.OnInit();
        PoolManager.Instance.OnInit();
        ResManager.Instance.OnInit();
        LocalData.Instance.OnInit();
        CreateFactory.Instance.OnInit();
        UIManager.Instance.OnInit();
        GameMgr.Instance.OnInit();
    }

    private void OnDestroy()
    {
        GameMgr.Instance.OnDestroy();
        LocalData.Instance.OnDisable();
        PoolManager.Instance.OnDestroy();
        EventCenter.Instance.Disable();
        UIManager.Instance.OnDestroy();
    }
}
