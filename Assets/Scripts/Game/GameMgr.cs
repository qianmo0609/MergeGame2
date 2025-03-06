using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoSingleton<GameMgr>
{
    GameMap gameMap = null;
    GameObject grid = null;
    Coroutine creategemCoroutine = null;
    Coroutine gemRandomFullCoroutine = null;
    Coroutine gemMergeCoroutione = null;
    Coroutine restartCoroutione = null;

    GemsItem[,] gemsItemsCollect;
    //Stack<GemsItem> mergeItemCollect; //������Ҫ�����ı�ʯ

    HashSet<GemsItem> mergeItemCollect; //������Ҫ�����ı�ʯ;

    ScoreList scoreList;

    Dictionary<int,MergeInfo> gemMergeInfos;

    int bombNumEatchRound = 1;
    Stack<GemsItem> bombCollecion;

    private bool[,] visited;// ���������

    #region yiled return ����
    WaitForSeconds ws005;
    WaitForSeconds ws01;
    WaitForSeconds ws02;
    WaitForSeconds ws03;
    WaitForSeconds ws05;
    WaitForSeconds ws08;
    WaitForSeconds ws10;
    WaitForSeconds ws20;
    #endregion

    public override void OnInit()
    {
        this.CreateGrid();
        gameMap = new GameMap();
        gameMap.OnInitLayout(grid);
        UIManager.Instance.GetWindow<MainUI>().Show();
        this.CreateWS();
        gemsItemsCollect = new GemsItem[GameCfg.row, GameCfg.col];
        StartCreateGems();
        mergeItemCollect = new HashSet<GemsItem>();
        //gemMergeInfos = new Dictionary<int, MergeInfo>();
        //bombCollecion = new Stack<GemsItem>(1);
        //visited = new bool[GameCfg.row, GameCfg.col];
        //scoreList = new ScoreList(gameMap.Bg.transform.Find("ListObj"));
    }

    void CreateWS()
    {
        ws005 = new WaitForSeconds(.015f);
        ws01 = new WaitForSeconds(.1f);
        ws02 = new WaitForSeconds(.2f);
        ws03 = new WaitForSeconds(.3f);
        ws05 = new WaitForSeconds(.5f);
        ws08 = new WaitForSeconds(.8f);
        ws10 = new WaitForSeconds(1f);
        ws20 = new WaitForSeconds(2f);
    }

    void CreateGrid()
    {
        if (this.grid == null)
            grid = new GameObject("Grid");
    }

    void StartCreateGems()
    {
        creategemCoroutine = StartCoroutine(this.CreateGem());
    }

    IEnumerator CreateGem()
    {
        GameCfg.gameState= GameState.isMatching;
        /*
           $$$$   
           $$$$
           $$$$
         ->$$$$
           �����̵����½ǿ�ʼ����,
            0123
           0$$$$
           1$$$$
           2$$$$
           3$$$$
           �к��д����Ͻǿ�ʼ�����һ�е�һ��
         */
        for (int j = GameCfg.row - 1; j >= 0; j--)
        {
            for (int i = 0; i < GameCfg.col; i++)
            {
                if (Utils.IsCorner(new Vector2Int(j, i))) continue;
                GemsItem gemItem = CreateOneGemItem(Utils.GetStartPos(j, i), i <= 1 ? DirEnum.left : DirEnum.right, new Vector2Int(j, i),isDelay:true);
                gemsItemsCollect[j, i] = gemItem;
            }
        }
        yield return ws02;
        //�����걦ʯ��ʼ���
        DetectMergeGems();

        if (this.creategemCoroutine != null)
        {
            StopCoroutine(this.creategemCoroutine);
            this.creategemCoroutine = null;
        }
    }

    GemsItem CreateOneGemItem(Vector3 pos, DirEnum dir, Vector2Int idx,bool isCreateBomb = false,bool isDelay = false)
    {
        GemsItem gemItem = CreateFactory.Instance.CreateGameObj<GemsItem>(GameObjEunm.gemItem);
        gemItem.transform.SetParent(grid.transform);
        gemItem.transform.position = pos;
        if (ResManager.Instance.gemsSprites.Length > 0)
        {
            //��Ҫ����������
            int spriteIdx = Utils.getGemsIdx(Utils.RandomIntVale(0, 10001));
            gemItem.OnInitInfo(isCreateBomb?ResManager.Instance.bombSprite:ResManager.Instance.gemsSprites[spriteIdx], spriteIdx, dir, idx,isCreateBomb);
        }
        gemItem.TweenTOPosition(isDelay:isDelay);
        return gemItem;
    }

    /// <summary>
    /// ��ʯ��ʼ�������������
    /// </summary>
    void StartRandomFull()
    {
        if (GameCfg.gameState != GameState.idle) { Debug.Log(ConstValue.tips); return; }
        gemRandomFullCoroutine = StartCoroutine(RandomFull());
        //��������ڹһ�״̬�£�ÿ�ε������Ҫ���õ���ʼ��ť
        GameCfg.isEnableBtnStart = false;
    }

    IEnumerator RandomFull(bool isReCreateFGems = true)
    {
        GemsItem g;
        for (int i = 0; i < gemsItemsCollect.GetLength(0); i++)
        {
            for (int j = 0; j < gemsItemsCollect.GetLength(1); j++)
            {
                g = gemsItemsCollect[i,j];
                g.IsFull = true;
            }
        }
        //��շ�����ʾ
        //scoreList.OnRestInfo();
        //�������֮��Ҫ����ⲿ�ֱ�ʯ
        System.Array.Clear(gemsItemsCollect, 0, gemsItemsCollect.Length);
        //����ÿ��ը����
        this.bombNumEatchRound = 1;
        //������ɺ������ɱ�ʯ
        yield return ws08;
        if (isReCreateFGems) { StartCreateGems(); }
        
        if (gemRandomFullCoroutine != null)
        {
            StopCoroutine(gemRandomFullCoroutine);
            gemRandomFullCoroutine = null;
        }
    }

    /// <summary>
    /// ��ⱦʯ,�ڱ�ʯ�������֮��ʼ��ⱦʯʱ���п�������
    /// </summary>
    public void DetectMergeGems()
    {
        //�����Ϸ�����ˣ��Ͳ���Ҫ�����
        if (GameCfg.gameState == GameState.gameOver) return;
        //�����⵽�п�������ı�ʯ��ִ���������
        if (DetectGemsMethod())
        {
            GameCfg.comboNum++;
            gemMergeCoroutione = StartCoroutine(MergeGems());
        }
        else
        {
            //û�м�⵽��������ı�ʯ��ϲ�״̬����
            GameCfg.gameState = GameState.idle;
            //����ǹһ�״̬,��û�п�����������ʱ������
            if (GameCfg.isHandUp)
            {
                //���û�м�⵽���Ժϲ��ı�ʯ����ȫ����ʯ���䣬�����������б�ʯ
                StartRandomFull();
            }
            else
            {
                //������ǹһ�״̬������Ҫ����StartBtn
                GameCfg.isEnableBtnStart = true;
            }
        }
    }

    /// <summary>
    /// ���ʱ����������ı�ʯ����
    /// </summary>
    /// <returns></returns>
    public bool DetectGemsMethod()
    {
        bool isMatch = false;
        int matchNum = 0;
        int gemType = -1;

        #region ������ⷨ
        //������
        for (int i = 0; i < GameCfg.row; i++)
        {
            matchNum = 0;
            gemType = -1;
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (Utils.IsCorner(new Vector2Int(i,j))) continue;
                //��Ϊ�洢�Ǵ����½ǿ�ʼ�洢�ģ����Դ�ͷ������oK��
                //g1 = gemsItemsCollect[i, j];
                //g2 = gemsItemsCollect[i, j + 1];
                //g3 = gemsItemsCollect[i, j + 2];
                //if ((g1.GemType & g2.GemType & g3.GemType) != 0)//& gemsItemsCollect[4 * i + j + 3].GemType) != 0)
                //{
                //    //��ô˵�����������ʯ��������һ����
                //    mergeItemCollect.Push(g1);
                //    mergeItemCollect.Push(g2);
                //    mergeItemCollect.Push(g3);

                //    //this.AddMergeInfo(g1.GemType, 100, g1.Type, g1.Idx.x, g1.Idx.y, 3);
                //    isMatch = true;
                //}
                if(gemType == -1)
                {
                    gemType = gemsItemsCollect[i, j].GemType;
                }
                if((gemsItemsCollect[i, j].GemType & gemType) != 0)
                {
                    matchNum++;
                }
                else
                {
                    if (matchNum >= 3)
                    {
                        mergeItemCollect.Add(gemsItemsCollect[i, j - 2]);
                        mergeItemCollect.Add(gemsItemsCollect[i, j - 1]);
                        mergeItemCollect.Add(gemsItemsCollect[i, j]);
                        isMatch = true;
                    }
                    matchNum = 0;
                    gemType = gemsItemsCollect[i, j].GemType;
                }
            }
        }
        int num = 0;
        //������
        for (int i = 0; i < GameCfg.row; i++)
        {
            matchNum = 0;
            gemType = -1;
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (Utils.IsCorner(new Vector2Int(i, j))) continue;
                //num = 0;
                ////��Ϊ�洢�Ǵ����½ǿ�ʼ�洢�ģ����Դ�ͷ������oK��
                //g1 = gemsItemsCollect[i, j];
                //g2 = gemsItemsCollect[i + 1, j];
                //g3 = gemsItemsCollect[i + 2, j];
                //if ((g1.GemType & g2.GemType & g3.GemType) != 0)//& gemsItemsCollect[i + j * 4 + 3].GemType) != 0)
                //{
                //    //��ô˵�����������ʯ��������һ����
                //    if (!mergeItemCollect.Contains(g1))
                //    {
                //        mergeItemCollect.Push(g1);
                //        isMatch = true;
                //        num++;
                //    }
                //    if (!mergeItemCollect.Contains(g2))
                //    {
                //        mergeItemCollect.Push(g2);
                //        isMatch = true;
                //        num++;
                //    }
                //    if (!mergeItemCollect.Contains(g3))
                //    {
                //        mergeItemCollect.Push(g3);
                //        isMatch = true;
                //        num++;
                //    }
                //    //this.AddMergeInfo(g1.GemType, 100, g1.Type, g1.Idx.x, g1.Idx.y, num);
                //}

                if (gemType == -1)
                {
                    gemType = gemsItemsCollect[i, j].GemType;
                }
                if ((gemsItemsCollect[i, j].GemType & gemType) != 0)
                {
                    matchNum++;
                }
                else
                {
                    if (matchNum >= 3)
                    {
                        mergeItemCollect.Add(gemsItemsCollect[i - 2, j]);
                        mergeItemCollect.Add(gemsItemsCollect[i - 1, j]);
                        mergeItemCollect.Add(gemsItemsCollect[i, j]);
                        isMatch = true;
                    }
                    gemType = gemsItemsCollect[i, j].GemType;
                    matchNum = 0;
                }
            }
        }
        #endregion

        #region ���������㷨
        /*
         * �����3��������������ô�Ի������ڵĴ�СΪ5*5�ģ��ڶ��μ��Ϳ�������ǰ����Ԫ�أ�
         * ֱ���������ĸ�Ԫ�ؿ�ʼ���,��Ϊ������û��Ҫ�����
          |$$$$$|$$$$              $$$|$$$$$|$
          |$$$$$|$$$$     --->     $$$|$$$$$|$  
          |$$$$$|$$$$              $$$|$$$$$|$
          |$$$$$|$$$$              $$$|$$$$$|$
          |$$$$$|$$$$              $$$|$$$$$|$
           $$$$$$$$$               $$$$$$$$$
         */
        #endregion
        return isMatch;
    }

    void AddMergeInfo(int gemType, int score, int type, int row, int col,int num)
    {
        MergeInfo mergeInfo;
        if(gemMergeInfos.TryGetValue(gemType,out mergeInfo))
        {
            mergeInfo.score += score;
            mergeInfo.row = row;
            mergeInfo.col = col;
            mergeInfo.num += num;
        }
        else 
        {
            mergeInfo = new MergeInfo {type = type,score = score,row = row,col = col,num = num};
            gemMergeInfos.Add(gemType, mergeInfo);
        }
    }

    /// <summary>
    /// �����ʯ
    /// </summary>
    IEnumerator MergeGems()
    {
        //����һ������ɵ��Աߣ�����һ��������Ч����
        //Dictionary<int, MergeInfo>.ValueCollection merges = gemMergeInfos.Values;
        //foreach (var item in merges)
        //{
        //    EffectManager.Instance.CreateEffectTextItem(item.score, Utils.GetNGUIPos(item.row), UIManager.Instance.UIRoot);
        //    this.CreateFlyGemItem(item);
        //    yield return new WaitForSeconds(.1f);
        //}
        //����������ϵ�gem�Ͳ�����Ч
        foreach (var item in mergeItemCollect)
        {
            item.PlayMergeEffect();
        }
        //yield return new WaitForSeconds(.5f);
        //����������
        foreach (var item in mergeItemCollect)
        {
            MergeGemAndMove(item.Idx.x, item.Idx.y);
            item.RecycleSelf();
        }

        //��պϲ���Ϣ
        mergeItemCollect.Clear();

        //�������ĸ����͵ķ�����Ϣ
        //gemMergeInfos.Clear();

        //�������ը����,�ȴ���ը��
        //if (bombCollecion.Count > 0)
        //{
        //    GemsItem g = bombCollecion.Pop();
        //    Vector2Int idx = g.Idx;
        //    yield return ws03;
        //    g.gameObject.SetActive(false);
        //    this.CreateBomb(Utils.GetCurrentPos(idx.x, idx.y), idx.x, idx.y);
        //}
        yield return ws08;

        if (gemMergeCoroutione != null)
        {
            StopCoroutine(gemMergeCoroutione);
            gemMergeCoroutione = null;
        }
        //�������֮���ٽ��м��
        DetectMergeGems();
    }

    //����һ����������ɵ��Ա�
    void CreateFlyGemItem(MergeInfo mergeInfo)
    {
        //���ӱ������ݼ�¼
        LocalData.Instance.AddScoreData(new ScoreData { type = mergeInfo.type, num = mergeInfo.num});
        //���Ӵ�һ����������ɵ�ָ��λ��
        scoreList.AddItem(mergeInfo);
    }

    void MergeGemAndMove(int x, int y) 
    {
        int xIdx = x;
        //��g�ǵڶ��е�Ԫ�أ�Ҫ�ӵ����С������н�Ԫ������
        //Sequence sequence = DOTween.Sequence();
        for (int i = x; i > 0; i--)
        {
            if (Utils.IsCorner(new Vector2Int(i-1, y)) || Utils.IsCorner(new Vector2Int(i, y))) continue;
            //�õ���һ�е�GemItem
            GemsItem g1 = gemsItemsCollect[i-1,y];
            //���ԭλ������
            gemsItemsCollect[i-1,y] = null;
            //���õ���GemItem��ֵ����һ��
            gemsItemsCollect[i,y] = g1;
            g1.Idx = new Vector2Int(xIdx, y);
            //��GemItem��������һ��
            //sequence.Join(g1.TweenTOPosition());
            g1.TweenTOPosition();
            xIdx--;
        }
        //sequence.Play();
        bool isCreateBomb = false;
        //�ȼ���ը�����ʣ��Ƿ���Ҫ����ը��
        //if (this.CalacBombPercentage())
        //{
        //    //���С��30����˵���������ɵ���ը��
        //    //this.CreateBomb(Utils.GetCurrentPos(0, y), 0, y);
        //    isCreateBomb = true;
        //    //�����ɵ�ը������1
        //    this.bombNumEatchRound--;
        //}
        //�����µ�GemsItem������λ��
        this.ReplenishGem(0, y, Utils.GetStartPos(0, y), isCreateBomb);
    }

    void CreateBomb(Vector3 curPos,int x, int y)
    {
        Bomb b = CreateFactory.Instance.CreateGameObj<Bomb>(GameObjEunm.bomb);
        //b.OnInitInfo(new MergeInfo { row = x,col = y}, gameMap.GetCurrentWallPos(), ResManager.Instance.bombSprite, this.BombCb,true);
    }

    /// <summary>
    /// ը��ִ�����Ļص�����
    /// </summary>
    void BombCb(MergeInfo mergeInfo)
    {
        this.MergeGemAndMove(mergeInfo.row, mergeInfo.col);
        //if (gameMap.DestroyWall())
        //{
        //    GameCfg.gameState = GameState.gameOver;
        //    restartCoroutione = StartCoroutine(this.OnRestartGame());
        //}
    }

    /// <summary>
    /// ����ը������
    /// </summary>
    bool CalacBombPercentage()
    {
        //��Ҫ��������µ�Gem����ը��������ǲ���ը��������Ҫ������ը���������µ�Gem���²�����ը��ĿǰĬ�����ڵ�0�У�����������λ������Ҫ�޸Ĵ�ʱ����ֵ
        if (this.bombNumEatchRound > 0 &&  Utils.RandomFloatVale(0.0f, GameCfg.bombPercentageDenominator) < GameCfg.bombPercentageNumerator)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �����µ�Gem
    /// </summary>
    void ReplenishGem(int x, int y, Vector3 curPos, bool isCreateBomb = false)
    {
        if (Utils.IsCorner(new Vector2Int(x, y))) return;
        GemsItem gNew = CreateOneGemItem(curPos, y < 3 ? DirEnum.left : DirEnum.right, new Vector2Int(0, y),isCreateBomb);
        gemsItemsCollect[0,y] = gNew;
        //�����ը�������
        if (isCreateBomb)
        {
            bombCollecion.Push(gNew);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        gameMap = null;
        grid = null;
        creategemCoroutine = null;
        gemRandomFullCoroutine = null;
        gemMergeCoroutione = null;
        StopAllCoroutines();
    }
}

public struct MergeInfo
{
    public int type;
    public int score;
    public int num;
    public int row;
    public int col;
}
