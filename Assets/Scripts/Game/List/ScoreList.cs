using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ScoreList
{
    Transform ListObj;
    List<ScoreListItem> scoreListCollection;
    int maxNum;

    int currentButtomIdx;

    public ScoreList(Transform obj)
    {
        this.ListObj = obj;
        this.OnReset();
    }

    public void OnReset()
    {
        maxNum = GameCfg.scoreListItemMaxNum;
        scoreListCollection = new List<ScoreListItem>(maxNum);
        currentButtomIdx = 0;
        EventCenter.Instance.RegisterEvent(EventNum.ClearScoreListEvent,this.ClearCollection);
    }

    public void AddItem(MergeInfo mergeInfo)
    {
        //生成一个飞行物体，飞行完成后，生成一个Gem
        EffectFlyItem ef = ResManager.Instance.CreateGameObj<EffectFlyItem>(GameObjEunm.effectFlyItem);
        this.MoveItem();
        ef.OnInitInfo(mergeInfo,this.ListObj.TransformPoint(Vector3.zero), ResManager.Instance.gemsSprites[mergeInfo.type], this.FlyCB);
    }

    void FlyCB(MergeInfo mergeInfo)
    {
        if (scoreListCollection.Count < maxNum)
        {
            this.DisplayGem(mergeInfo);
        }
        else
        {
            this.MoveButtomItem(mergeInfo);
        }
    }

    /// <summary>
    /// 增加Item
    /// </summary>
    /// <param name="mergeInfo"></param>
    public void DisplayGem(MergeInfo mergeInfo)
    {
        //增加显示的Item
        ScoreListItem sl = ResManager.Instance.CreateGameObj<ScoreListItem>(GameObjEunm.scoreListItem);
        sl.OnSetInfo(ResManager.Instance.gemsSprites[mergeInfo.type], mergeInfo.num);
        sl.transform.parent = ListObj;
        sl.transform.localPosition = Vector3.zero;
        scoreListCollection.Add(sl);
    }

    /// <summary>
    /// 移动Item
    /// </summary>
    void MoveItem()
    {
        float y = 0;
        for (int i = 0; i < scoreListCollection.Count; i++)
        {
            y = 0 - GameCfg.scoreListItemInterval * (scoreListCollection.Count - this.MappintIdx(i));
            scoreListCollection[i].transform.DOLocalMoveY(y, .2f);
            if(scoreListCollection[i].transform.localPosition.y <= -1.5f)
            {
                scoreListCollection[i].gameObject.SetActive(false);
            }
        }
    }

    public void MoveButtomItem(MergeInfo mergeInfo)
    {
        ScoreListItem sl = scoreListCollection[currentButtomIdx];
        sl.gameObject.SetActive(true);
        sl.transform.localPosition = Vector3.zero;
        sl.OnSetInfo(ResManager.Instance.gemsSprites[mergeInfo.type], mergeInfo.num);
        this.currentButtomIdx++;
        this.currentButtomIdx %= GameCfg.scoreListItemMaxNum;
    }

    public int MappintIdx(int i)
    {
        int idx = i - this.currentButtomIdx;
        if (idx < 0)
        {
            idx += this.maxNum;
            
        }
        return idx;
    }

    void ClearCollection()
    {
        for (int i = scoreListCollection.Count - 1; i >=0 ; i--)
        {
            scoreListCollection[i].OnRecycleSelf();
        }
        scoreListCollection.Clear();
        this.currentButtomIdx = 0;
    }

    public void OnRestInfo()
    {
        this.ClearCollection();
        this.OnReset();
    }
}
