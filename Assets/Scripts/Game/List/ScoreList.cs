using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreList
{
    Transform ListObj;
    Vector3 currentPos;
    List<ScoreListItem> scoreListCollection;
    int maxDisplayNum;
    int maxNum;

    int currentButtomIdx;

    public ScoreList(Transform obj)
    {
        this.ListObj = obj;
        this.OnReset();
#if UNITY_EDITOR
        this.TestListDisplay();
#endif
    }

    public void OnReset()
    {
        maxDisplayNum = GameCfg.scoreListItemMaxNum - 1;
        maxNum = GameCfg.scoreListItemMaxNum;
        scoreListCollection = new List<ScoreListItem>(maxNum);
        currentPos = GameCfg.scoreListStartPoss[GameCfg.level - 1];
        currentButtomIdx = 0;
    }

    public void AddItem(MergeInfo mergeInfo)
    {
        //生成一个飞行物体，飞行完成后，生成一个Gem
        EffectFlyItem ef = CreateFactory.Instance.CreateGameObj<EffectFlyItem>(GameObjEunm.effectFlyItem);
        this.MoveItem();
        ef.OnInitInfo(mergeInfo,this.ListObj.TransformPoint(this.GetNextItemPos()), ResManager.Instance.gemsSprites[mergeInfo.type], this.GetCb());
    }

#if UNITY_EDITOR
    //测试列表物体显示
    void TestListDisplay()
    {
        //new GameObject().AddComponent<Test>().StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        while (scoreListCollection.Count < maxNum)
        {
            //this.DisplayGem();
            this.AddItem(new MergeInfo{type = 1,score = 100,num = 1,row = 1,col = 1 });
            yield return new WaitForSeconds(1);
        }
        ClearCollection();
    }
#endif

    public void DisplayGem(MergeInfo mergeInfo)
    {
        //增加显示的Item
        ScoreListItem sl = CreateFactory.Instance.CreateGameObj<ScoreListItem>(GameObjEunm.scoreListItem);
        sl.OnSetInfo(ResManager.Instance.gemsSprites[mergeInfo.type], mergeInfo.num);
        sl.transform.parent = ListObj;
        sl.transform.localPosition = currentPos;
        scoreListCollection.Add(sl);
        if (scoreListCollection.Count < maxNum)
        {
            currentPos += Vector3.up * GameCfg.scoreListItemInterval;
        }
    }

    void MoveItem()
    {
        if (scoreListCollection.Count > maxDisplayNum)
        {
            float y;
            #region 使用Sequence方式
            //Sequence sequence = DOTween.Sequence();
            ////如果列表中已经大于6个，则列表Item下移， 最底部的Item移动到上部显示
            //for (int i = 0; i < scoreListCollection.Count; i++)
            //{
            //    y = GameCfg.scoreListStartPoss[GameCfg.level - 1].y + (i - 1) * GameCfg.scoreListItemInterval;
            //    sequence.Join(scoreListCollection[i].transform.DOLocalMoveY(y, 1.2f));
            //}
            //for (int i = 0; i < scoreListCollection.Count; i++)
            //{
            //    y = currentPos.y - GameCfg.scoreListItemInterval * (GameCfg.scoreListItemMaxNum - this.MappintIdx(i) + 1);
            //    sequence.Join(scoreListCollection[i].transform.DOLocalMoveY(y, .3f));
            //}
            //sequence.Play();//.OnComplete(this.MoveButtomItem);
            #endregion
            for (int i = 0; i < scoreListCollection.Count; i++)
            {
                y = currentPos.y - GameCfg.scoreListItemInterval * (GameCfg.scoreListItemMaxNum - this.MappintIdx(i));
                scoreListCollection[i].transform.DOLocalMoveY(y, .2f);
            }
        }
    }

    Action<MergeInfo> GetCb()
    {
         return scoreListCollection.Count < maxNum?this.DisplayGem:MoveButtomItem;
    }

    public void MoveButtomItem(MergeInfo mergeInfo)
    {
        ScoreListItem sl = scoreListCollection[currentButtomIdx];
        sl.transform.localPosition = currentPos;
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

    public Vector3 GetNextItemPos()
    {
        return currentPos + new Vector3(GameCfg.flyTOPosOffsetX, GameCfg.scoreListItemInterval, 0);
    }  

    void ClearCollection()
    {
        for (int i = scoreListCollection.Count - 1; i >=0 ; i--)
        {
            ScoreListItem sl = scoreListCollection[i];
            sl.SetItemState(false,GameCfg.spriteRange);
            sl.IsFull = true;
            //Vector3 tarPos = sl.transform.position + new Vector3(Utils.RandomFloatVale(-1.0f, 1f), Utils.RandomFloatVale(0, 1.0f), 0);
            //sl.transform.DOMove(tarPos, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
            //{
            //    //宝石下落
            //    sl.transform.DOMoveY(-10, Utils.RandomFloatVale(0.1f, 0.8f)).SetEase(Ease.InExpo).OnComplete(() => { sl.OnRecycleSelf(); });
            //});
        }
        scoreListCollection.Clear();
    }

    public void OnRestInfo()
    {
        this.ClearCollection();
        this.OnReset();
    }
}
