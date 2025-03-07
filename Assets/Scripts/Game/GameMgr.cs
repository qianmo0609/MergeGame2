using System.Collections;
using System.Collections.Generic;
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
    Dictionary<int,MergeInfo> gemMergeInfos;

    private bool isFirst; //�Ƿ��ǵ�һ��ִ��

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
        UIManager.Instance.GetWindow<MainUI>().Show();
        gemsItemsCollect = new GemsItem[GameCfg.row, GameCfg.col];
        gemMergeInfos = new Dictionary<int, MergeInfo>();
        bombItems = new List<BombItemInfo>();
        mapFlag = new int[GameCfg.row, GameCfg.col];
        gemsItems = new List<HashSet<Vector2Int>>();
        this.isFirst = true;
        EventCenter.Instance.RegisterEvent(EventNum.startEvent, this.StartRandomFull);
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
        //this.DisplayArray();
        yield return ws02;
        //�����걦ʯ��ʼ���
        DetectMergeGems();

        if (this.creategemCoroutine != null)
        {
            StopCoroutine(this.creategemCoroutine);
            this.creategemCoroutine = null;
        }
    }

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
        gemItem.TweenTOPosition();
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
        }
    }

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
        //TODO:��շ�����ʾ

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
            this.DisplayArray();
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
                        mapFlag[g1.Idx.x, g1.Idx.y] = 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = 1;
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
                        mapFlag[g1.Idx.x, g1.Idx.y] = 1;
                        mapFlag[g2.Idx.x, g2.Idx.y] = 1;
                        mapFlag[g3.Idx.x, g3.Idx.y] = 1;
                        isMatch = true;
                    }
                }
            }
        }
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
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (mapFlag[i,j] == 1)
                {
                    //�ʹӵ�ǰ�㿪ʼ����
                    gemsItems.Add(this.FindMatches(i,j));
                }
            }
        }
    }

    // �ķ����������ϡ��¡�����
    Vector2Int[] directions = {
        new Vector2Int(-1,0), //��
        new Vector2Int(1,0), //��
        new Vector2Int(0,-1), //�� 
        new Vector2Int(0,1) //��
    };

    HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    HashSet<Vector2Int> FindMatches(int x,int y)
    {
        matches.Clear();
        queue.Clear();

        // ��ʼ������
        queue.Enqueue(new Vector2Int(x,y));
        mapFlag[x, y] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            matches.Add(current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                // �߽���
                if (next.x < 0 || next.x >= GameCfg.row) continue;
                if (next.y < 0 || next.y >= GameCfg.col) continue;

                // ���ͼ�� && ���ʱ�� 
                if (mapFlag[next.x, next.y] == 1)
                {
                    mapFlag[next.x, next.y] = 0;
                    queue.Enqueue(new Vector2Int(next.x, next.y));
                }
            }
        }
        string s = "";
        foreach (var item in matches)
        {
            s += $"{item.x} + {item.y} ";
        }
        Debug.Log(s);
        return matches;
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

        HashSet<Vector2Int> gems;
        Debug.Log("ccccccccccc");
        for (int i = 0; i < gemsItems.Count; i++)
        {
            //TODO:����������״�ж�����ը��

            gems = gemsItems[i];
            //����������ϵ�gem�Ͳ�����Ч
            foreach (var item in gems)
            {
                gemsItemsCollect[item.x,item.y]?.PlayMergeEffect();
            }
            yield return new WaitForSeconds(.8f);
        }
        //GemsItem g;
        //for (int i = 0; i < gemsItems.Count; i++)
        //{
        //    //����������
        //    foreach (var item in gemsItems[i])
        //    {
        //        g = gemsItemsCollect[item.x, item.y];
        //        if (g.IsBomb == BombType.none)
        //        {
        //            MergeGemAndMove(g.Idx.x, g.Idx.y);
        //            g?.RecycleSelf();
        //        }
        //    }
        //}

        ////��պϲ���Ϣ
        //mergeItemCollect.Clear();
        System.Array.Clear(mapFlag, 0, mapFlag.Length);

        //�������ĸ����͵ķ�����Ϣ
        //gemMergeInfos.Clear();

        //�������ը����,�ȴ���ը��
        if (bombItems.Count > 0)
        {
            //�����Ӧը������
            //�����ó���һ��ը����Ϣ
            //bombManager.HandlerBomb(bombItems[0], gemsItemsCollect, mergeItemCollect,bombItems);
        }
        yield return ws08;

        if (gemMergeCoroutione != null)
        {
            StopCoroutine(gemMergeCoroutione);
            gemMergeCoroutione = null;
        }

        //�������֮���ٽ��м��
        //DetectMergeGems();
    }

    //����һ����������ɵ��Ա�
    void CreateFlyGemItem(MergeInfo mergeInfo)
    {
        //���ӱ������ݼ�¼
        LocalData.Instance.AddScoreData(new ScoreData { type = mergeInfo.type, num = mergeInfo.num});
        //TODO:���Ӵ�һ����������ɵ�ָ��λ��
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
