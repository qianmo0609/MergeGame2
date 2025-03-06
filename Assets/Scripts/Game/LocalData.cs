using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ScoreData
{
    public int num;
    public int type;
}

public class LocalData : Singleton<LocalData>
{
    List<ScoreData> scoreDatas;

    public List<ScoreData> ScoreDatas { get => scoreDatas;}

    public override void OnInit()
    {
        scoreDatas = new List<ScoreData>();
        base.OnInit();
    }

    public void AddScoreData(ScoreData scoreData)
    {
        scoreDatas.Add(scoreData);
    }

    public List<ScoreData> GetScoreData()
    {
        return scoreDatas;
    }

    public void OnDisable()
    {
        scoreDatas.Clear();
        scoreDatas = null;
    }
}
