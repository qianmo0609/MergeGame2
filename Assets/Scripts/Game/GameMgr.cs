using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMgr : MonoSingleton<GameMgr>
{
    GameMap gameMap = null;
    GameObject grid = null;
    BombManager bombManager = null; // ը��������
    Coroutine creategemCoroutine = null;
    Coroutine gemRandomFullCoroutine = null;
    Coroutine gemMergeCoroutione = null;

    GemsItem[,] gemsItemsCollect; //��Ϸ�еı�ʯ����
    int[,] mapFlag; //���ڱ������λ�õ���������
    List<HashSet<Vector2Int>> gemsItems; //���ڴ洢����������

    List<BombItemInfo> bombItems; //�洢��ǰ��ը����Ϣ
    List<MergeInfo> gemMergeInfos; //�洢����������Ϣ

    List<List<MergeInfo>> bombMergeInfo; //�洢ը������������Ϣ

    private bool isFirst; //�Ƿ��ǵ�һ��ִ��
    ScoreList scoreList;

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
        this.CreateWS();
        gameMap = new GameMap();
        bombManager = new BombManager();
        gameMap.OnInitLayout(grid);
        scoreList = new ScoreList(gameMap.Bg.transform.Find("List"));
        UIManager.Instance.GetWindow<MainUI>().Show();
        gemsItemsCollect = new GemsItem[GameCfg.row, GameCfg.col];
        gemMergeInfos = new List<MergeInfo>();
        bombMergeInfo = new List<List<MergeInfo>>();
        bombItems = new List<BombItemInfo>();
        mapFlag = new int[GameCfg.row, GameCfg.col];
        gemsItems = new List<HashSet<Vector2Int>>();
        this.isFirst = true;
        EventCenter.Instance.RegisterEvent(EventNum.StartEvent, this.StartRandomFull);
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
                GemsItem gemItem = CreateOneGemItem(Utils.GetStartPos(j, i), i <= 1 ? DirEnum.left : DirEnum.right, new Vector2Int(j, i));
                gemsItemsCollect[j, i] = gemItem;
            }
        }
        yield return ws05;
        //�����걦ʯ��ʼ���
        DetectMergeGems();

        if (this.creategemCoroutine != null)
        {
            StopCoroutine(this.creategemCoroutine);
            this.creategemCoroutine = null;
        }
    }
    
    /// <summary>
    /// ��ӡ���������ʾ
    /// </summary>
    void DisplayArray()
    {
        string s = "";
        for (int j = 0; j < GameCfg.row; j++)
        {
            for (int i = 0; i < GameCfg.col; i++)
            {
                s += mapFlag[j, i];
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    GemsItem CreateOneGemItem(Vector3 pos, DirEnum dir, Vector2Int idx,bool isCreateBomb = false)
    {
        GemsItem gemItem = CreateFactory.Instance.CreateGameObj<GemsItem>(GameObjEunm.gemItem);
        gemItem.transform.SetParent(grid.transform);
        gemItem.transform.position = pos;
        if (ResManager.Instance.gemsSprites.Length > 0)
        {
            //��Ҫ����������
            int spriteIdx = Utils.getGemsIdx(Utils.RandomIntVale(0, 10001));
            gemItem.OnInitInfo(ResManager.Instance.gemsSprites[spriteIdx], spriteIdx, dir, idx,BombType.none);
        }
        gemItem.TweenTOPosition(isDelay:true);
        return gemItem;
    }

    /// <summary>
    /// ��ʯ��ʼ�������������
    /// </summary>
    void StartRandomFull()
    {
        if (GameCfg.gameState != GameState.idle) { Debug.Log(ConstValue.tips); Debug.Log($"��ǰ����Ϸ״̬�ǣ�{GameCfg.gameState}"); return; }
        if (this.isFirst)
        {
            this.StartCreateGems();
            this.isFirst = false;
        }
        else
        {
            gemRandomFullCoroutine = StartCoroutine(RandomFull());
            EventCenter.Instance.ExcuteEvent(EventNum.ClearScoreListEvent);
            EventCenter.Instance.ExcuteEvent(EventNum.HideComboLabel);
            GameCfg.comboNum = 0;
        }
        GameCfg.isEnableBtnStart = false;
        EventCenter.Instance.ExcuteEvent(EventNum.EnableOrDisableBtnStartEvent);
    }

#if UNITY_EDITOR
    void TestBomb()
    {
        //�����
        //Debug.Log(bombManager.IsLine5(new Vector2Int[] { new Vector2Int(3,1), new Vector2Int(3,2), new Vector2Int(3,3) , new Vector2Int(3,4) , new Vector2Int(3,5) }));
        //�����
        //Debug.Log(bombManager.IsLine5(new Vector2Int[] { new Vector2Int(1,1), new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1), new Vector2Int(5, 1) }));

        //bool isT = false;
        //Tshap;
        ///*
        //      0           
        //    000
        //      0
        // */
        //isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3),new Vector2Int(2,3), new Vector2Int(4, 3) });
        //Debug.Log(isT);
        ///*
        //    0
        //    000
        //    0
        // */
        //isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(2, 1), new Vector2Int(4, 1) });
        //Debug.Log(isT);
        ///*
        // 0 0 0
        //   0
        //   0
        // */
        //isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(4, 2), new Vector2Int(5, 2) });
        //Debug.Log(isT);
        ///*
        //  0
        //  0
        //0 0 0
        // */
        //isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(1, 2), new Vector2Int(2, 2) });
        //Debug.Log(isT);

        
   //     /*
   //      0           
   //   0000
   //      0
   //*/
   //     isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 0),new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(2, 3), new Vector2Int(4, 3) });
   //     Debug.Log(isT);
   //     /*
   //         0
   //         0000
   //         0
   //      */
   //     isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 0),new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(2, 0), new Vector2Int(4, 0) });
   //     Debug.Log(isT);
   //     /*
   //      0 0 0
   //        0
   //        0
   //        0
   //      */
   //     isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(4, 2), new Vector2Int(5, 2), new Vector2Int(5, 3) });
   //     Debug.Log(isT);
   //     /*
   //       0
   //       0
   //       0
   //     0 0 0
   //      */
   //     isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(0, 2),new Vector2Int(1, 2), new Vector2Int(2, 2) });
   //     Debug.Log(isT);

        //Lshape

    //    /*
    //     0           
    //     0
    //     0 0 0
    //*/
    //    isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(1, 1), new Vector2Int(2, 1) });
    //    Debug.Log(isT);
    //    /*
    //     0 0 0
    //     0
    //     0
    //     */
    //    isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(4, 1), new Vector2Int(5, 1) });
    //    Debug.Log(isT);
    //    /*
    //     0 0 0
    //         0
    //         0
    //     */
    //    isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(4, 3), new Vector2Int(5, 3) });
    //    Debug.Log(isT);
    //    /*
    //        0
    //        0
    //    0 0 0
    //     */
    //    isT = bombManager.IsTShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(1, 3), new Vector2Int(2, 3) });
    //    Debug.Log(isT);

        //Cross Shape
        /*
         0           
       0 0 0
         0 
       */
        //isT = bombManager.IsCrossShape(new Vector2Int[] { new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(1, 2), new Vector2Int(4, 2) });
        //Debug.Log(isT);
    }
#endif

    IEnumerator RandomFull(bool isReCreateFGems = true)
    {
        GemsItem g;
        for (int i = 0; i < gemsItemsCollect.GetLength(0); i++)
        {
            for (int j = 0; j < gemsItemsCollect.GetLength(1); j++)
            {
                if (Utils.IsCorner(new Vector2Int(i, j))) continue;
                g = gemsItemsCollect[i,j];
                g.IsFull = true;
            }
        }
        //�������֮��Ҫ����ⲿ�ֱ�ʯ
        System.Array.Clear(gemsItemsCollect, 0, gemsItemsCollect.Length);
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
        //�����⵽�п�������ı�ʯ��ִ���������
        if (DetectGemsMethod())
        {
            //�������
            this.SearchPoint();
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
            GameCfg.isEnableBtnStart = true;
            EventCenter.Instance.ExcuteEvent(EventNum.EnableOrDisableBtnStartEvent);
        }
    }

    /// <summary>
    /// ���ʱ����������ı�ʯ����
    /// </summary>
    /// <returns></returns>
    public bool DetectGemsMethod()
    {
        bool isMatch = false;
        #region ������ⷨ
        #endregion
        isMatch = this.CheckThree();
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

    /// <summary>
    /// ���3x3
    /// </summary>+
    /// <returns></returns>
    bool CheckThree()
    {
        bool isMatch = false;
        GemsItem g1, g2, g3;
        //������
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col - 2; j++)
            {
                //��Ϊ�洢�Ǵ����½ǿ�ʼ�洢�ģ����Դ�ͷ������oK��
                g1 = gemsItemsCollect[i, j];
                g2 = gemsItemsCollect[i, j + 1];
                g3 = gemsItemsCollect[i, j + 2];
                if (g1 != null && g2 != null && g3 != null)
                {
                    if ((g1.GemType & g2.GemType & g3.GemType) != 0)
                    {
                        mapFlag[g1.Idx.x, g1.Idx.y] = g1.Type + 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = g1.Type + 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = g1.Type + 1;
                        isMatch = true;
                    }
                }
            }
        }

        //������
        for (int i = 0; i < GameCfg.row - 2; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                g1 = gemsItemsCollect[i, j];
                g2 = gemsItemsCollect[i + 1, j];
                g3 = gemsItemsCollect[i + 2, j];
                if (g1 != null && g2 != null && g3 != null)
                {
                    if ((g1.GemType & g2.GemType & g3.GemType) != 0)
                    {
                        mapFlag[g1.Idx.x, g1.Idx.y] = g1.Type + 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = g1.Type + 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = g1.Type + 1;
                        isMatch = true;
                    }
                }
            }
        }
        //this.DisplayArray();
        return isMatch;
    }

    /// <summary>
    /// �����������ĵ�ͼ��ɸѡ������Ҫ��ĵ�
    /// </summary>
    void SearchPoint()
    {
        //1.�����ҵ���ͼ��һ����Ϊ0�ĵ㣬����Ϊ��ǰ��
        //2.ʹ��DFS/BFS �ҵ���������ͨ�����е㣬������ʱ��ÿ�õ�һ����ͽ�������
        //3.�ӵ�ǰ�㿪ʼ���������ҵ�һ�����ʵ㣬ֱ���������
        GemsItem g = null;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (mapFlag[i,j] >= 1)
                {
                    //�ʹӵ�ǰ�㿪ʼ����
                    g = gemsItemsCollect[i, j];
                    HashSet<Vector2Int> gems = this.FindMatches(i, j, g.Type + 1);
                    //Debug.Log(gems.Count);
                    this.AddMergeInfo(g.Type,i,j,gems.Count);
                    gemsItems.Add(gems);
                }
            }
        }
    }

    HashSet<Vector2Int> FindMatches(int x,int y,int tarType)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // ��ʼ������
        queue.Enqueue(new Vector2Int(x,y));
        mapFlag[x, y] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            matches.Add(current);

            foreach (Vector2Int dir in Utils.directions)
            {
                Vector2Int next = current + dir;

                // �߽���
                if (next.x < 0 || next.x >= GameCfg.row) continue;
                if (next.y < 0 || next.y >= GameCfg.col) continue;

                // ���ͼ�� && ���ʱ�� 
                if (mapFlag[next.x, next.y] == tarType)
                {
                    mapFlag[next.x, next.y] = 0;
                    queue.Enqueue(new Vector2Int(next.x, next.y));
                }
            }
        }
        return matches;
    }

    void AddMergeInfo(int type, int row, int col,int num)
    {
        MergeInfo mergeInfo = new MergeInfo { type = type, row = row, col = col, num = num };
        gemMergeInfos.Add(mergeInfo);
    }

    /// <summary>
    /// �����ʯ
    /// </summary>
    IEnumerator MergeGems(bool isBombMerge = false)
    {
        HashSet<Vector2Int> gems;
        bool isPlayEffectTxt = false;
        for (int i = 0; i < gemsItems.Count; i++)
        {
            gems = gemsItems[i];
            //����������״�ж�����ը��
            if (!isBombMerge) {
                //����ը������������Ҫִ��ը�����ж�
                bombManager.SwitchBomb(gems.Count,gems.ToArray(),gemsItemsCollect, bombItems);
            }
            isPlayEffectTxt = true; 
            //����������ϵ�gem�Ͳ�����Ч
            foreach (var item in gems)
            {
                if (isPlayEffectTxt)
                {
                    //����һ������ɵ��Աߣ�����һ��������Ч����
                    //��Ч����
                    EffectManager.Instance.CreateEffectTextItem(100 * gems.Count, Utils.GetNGUIPos(item.x), UIManager.Instance.UIRoot);
                    //��ը���ϲ�ʱִ��
                    if (!isBombMerge)
                        this.CreateFlyGemItem(gemMergeInfos[i]);
                    isPlayEffectTxt = false;
                }
                //Debug.Log(gemsItemsCollect[item.x, item.y]);
                gemsItemsCollect[item.x,item.y]?.PlayMergeEffect();
            }

            if (isBombMerge)
            {
                //ը���ϲ�����ʱ��Ҫ��һ�ַ�ʽ,ÿ��ը���������������Ͳ�һ������Ҫ������������һһ��ʾ
                for (int x = 0; x < bombMergeInfo[i].Count; x++)
                {
                    this.CreateFlyGemItem(bombMergeInfo[i][x]);
                    yield return new WaitForSeconds(.8f);
                }
            }
            yield return new WaitForSeconds(.8f);
        }
        GemsItem g;
        for (int i = 0; i < gemsItems.Count; i++)
        {
            gems = gemsItems[i];
            //�����ը���ϲ���Ҫ�������ٺϲ�
            if (isBombMerge) {
                var sortedEnumerable = gems.OrderBy(v => v.x).ThenBy(v => v.y);
                //����������
                foreach (var item in sortedEnumerable)
                {
                    g = gemsItemsCollect[item.x, item.y];
                    if (g != null && g.IsBomb == BombType.none)
                    {
                        yield return new WaitForSeconds(.01f);
                        MergeGemAndMove(g.Idx.x, g.Idx.y);
                        g.RecycleSelf();
                    }
                }
            }
            else
            {
                foreach (var item in gems)
                {
                    g = gemsItemsCollect[item.x, item.y];
                    if (g != null && g.IsBomb == BombType.none)
                    {
                        yield return new WaitForSeconds(.01f);
                        MergeGemAndMove(g.Idx.x, g.Idx.y);
                        g.RecycleSelf();
                    }
                }
            }
        }

        //��պϲ���Ϣ
        System.Array.Clear(mapFlag, 0, mapFlag.Length);
        gemsItems.Clear();

        //�������ĸ����͵ķ�����Ϣ
        gemMergeInfos.Clear();
        bombMergeInfo.Clear();

        yield return ws08;
        if (gemMergeCoroutione != null)
        {
            StopCoroutine(gemMergeCoroutione);
            gemMergeCoroutione = null;
        }
        //�������ը����,�ȴ���ը��
        if (bombItems.Count > 0)
        {
            for (int i = 0; i < bombItems.Count; i++)
            {
                //�����Ӧը������
                bombManager.HandlerBomb(bombItems[i], gemsItemsCollect, gemsItems,bombItems,bombMergeInfo);
            }
            //���ը����Ϣ
            bombItems.Clear();
            //ը����������Ҫ�Ƴ���������Ӻ�ִ�кϲ�����
            gemMergeCoroutione = StartCoroutine(MergeGems(true));
        }
        else
        {
            //û������ը���������֮��ͽ��м��
            DetectMergeGems();
        }
    }

    //����һ����������ɵ��Ա�
    void CreateFlyGemItem(MergeInfo mergeInfo)
    {
        //���Ӵ�һ����������ɵ�ָ��λ��
        scoreList.AddItem(mergeInfo);
        GameCfg.comboNum++;
        EventCenter.Instance.ExcuteEvent(EventNum.ShowComboLabel);
    }

    void MergeGemAndMove(int x, int y) 
    {
        int xIdx = x;
        //��g�ǵڶ��е�Ԫ�أ�Ҫ�ӵ����С������н�Ԫ������
        for (int i = x; i > 0; i--)
        {
            if (Utils.IsCorner(new Vector2Int(i, y))) continue;
            //�õ���һ�е�GemItem
            GemsItem g1 = gemsItemsCollect[i-1,y];
            if (g1 == null)
            {
                this.ReplenishGem(i, y, Utils.GetStartPos(0, y), false);
                continue;
            }
            //���õ���GemItem��ֵ����һ��
            gemsItemsCollect[i,y] = g1;
            g1.Idx = new Vector2Int(xIdx, y);
            //���ԭλ������
            gemsItemsCollect[i - 1, y] = null;
            g1.TweenTOPosition();
            xIdx--;
        }
        bool isCreateBomb = false;
        //�����µ�GemsItem������λ��
        this.ReplenishGem(0, y, Utils.GetStartPos(0, y), isCreateBomb);
    }

    /// <summary>
    /// �����µ�Gem
    /// </summary>
    void ReplenishGem(int x, int y, Vector3 curPos, bool isCreateBomb = false)
    {
        if (Utils.IsCorner(new Vector2Int(x, y))) return;
        GemsItem gNew = CreateOneGemItem(curPos, y < 3 ? DirEnum.left : DirEnum.right, new Vector2Int(x, y),isCreateBomb);
        gemsItemsCollect[x,y] = gNew;
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
