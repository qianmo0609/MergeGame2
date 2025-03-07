using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] CurveManager curveManager; 
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
        Instantiate(curveManager);
    }

    private void OnDestroy()
    {
        LocalData.Instance.OnDisable();
        PoolManager.Instance.OnDestroy();
        EventCenter.Instance.Disable();
        UIManager.Instance.OnDestroy();
    }
}
