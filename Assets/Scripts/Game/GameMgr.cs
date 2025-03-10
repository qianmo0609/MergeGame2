using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMgr : MonoSingleton<GameMgr>
{
    GameMap gameMap = null;
    GameObject grid = null;
    BombManager bombManager = null; // 炸弹管理器
    Coroutine creategemCoroutine = null;
    Coroutine gemRandomFullCoroutine = null;
    Coroutine gemMergeCoroutione = null;

    GemsItem[,] gemsItemsCollect; //游戏中的宝石数据
    int[,] mapFlag; //用于标记消除位置的物体类型
    List<HashSet<Vector2Int>> gemsItems; //用于存储分类后的物体

    List<BombItemInfo> bombItems; //存储当前的炸弹信息
    List<MergeInfo> gemMergeInfos; //存储消除分数信息

    List<List<MergeInfo>> bombMergeInfo; //存储炸弹消除分数信息

    private bool isFirst; //是否是第一次执行
    ScoreList scoreList;

    #region yiled return 定义
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
           从棋盘的左下角开始生成,
            0123
           0$$$$
           1$$$$
           2$$$$
           3$$$$
           行和列从左上角开始计算第一行第一列
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
        //生成完宝石开始检测
        DetectMergeGems();

        if (this.creategemCoroutine != null)
        {
            StopCoroutine(this.creategemCoroutine);
            this.creategemCoroutine = null;
        }
    }
    
    /// <summary>
    /// 打印标记数组显示
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
            //需要按概率生成
            int spriteIdx = Utils.getGemsIdx(Utils.RandomIntVale(0, 10001));
            gemItem.OnInitInfo(ResManager.Instance.gemsSprites[spriteIdx], spriteIdx, dir, idx,BombType.none);
        }
        gemItem.TweenTOPosition(isDelay:true);
        return gemItem;
    }

    /// <summary>
    /// 宝石开始随机往上在下落
    /// </summary>
    void StartRandomFull()
    {
        if (GameCfg.gameState != GameState.idle) { Debug.Log(ConstValue.tips); Debug.Log($"当前的游戏状态是：{GameCfg.gameState}"); return; }
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
        //横五个
        //Debug.Log(bombManager.IsLine5(new Vector2Int[] { new Vector2Int(3,1), new Vector2Int(3,2), new Vector2Int(3,3) , new Vector2Int(3,4) , new Vector2Int(3,5) }));
        //竖五个
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
        //掉落完成之后要清除这部分宝石
        System.Array.Clear(gemsItemsCollect, 0, gemsItemsCollect.Length);
        //下落完成后再生成宝石
        yield return ws08;
        if (isReCreateFGems) { StartCreateGems(); }
        
        if (gemRandomFullCoroutine != null)
        {
            StopCoroutine(gemRandomFullCoroutine);
            gemRandomFullCoroutine = null;
        }
    }

    /// <summary>
    /// 检测宝石,在宝石下落完成之后开始检测宝石时候有可消除的
    /// </summary>
    public void DetectMergeGems()
    {
        //如果检测到有可以清除的宝石，执行清除方法
        if (DetectGemsMethod())
        {
            //将点分类
            this.SearchPoint();
            gemMergeCoroutione = StartCoroutine(MergeGems());
        }
        else
        {
            //没有检测到可以清除的宝石则合并状态结束
            GameCfg.gameState = GameState.idle;
            //如果是挂机状态,在没有可消除的物体时才下落
            if (GameCfg.isHandUp)
            {
                //如果没有检测到可以合并的宝石，则全部宝石下落，重新生成所有宝石
                StartRandomFull();
            }
            GameCfg.isEnableBtnStart = true;
            EventCenter.Instance.ExcuteEvent(EventNum.EnableOrDisableBtnStartEvent);
        }
    }

    /// <summary>
    /// 检测时候可以消除的宝石方法
    /// </summary>
    /// <returns></returns>
    public bool DetectGemsMethod()
    {
        bool isMatch = false;
        #region 暴力检测法
        #endregion
        isMatch = this.CheckThree();
        #region 滑动窗口算法
        /*
         * 如果是3个可以消除，那么以滑动窗口的大小为5*5的，第二次检测就可以跳过前三个元素，
         * 直接跳到第四个元素开始检查,因为这三个没必要检测了
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
    /// 检查3x3
    /// </summary>+
    /// <returns></returns>
    bool CheckThree()
    {
        bool isMatch = false;
        GemsItem g1, g2, g3;
        //横向检测
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col - 2; j++)
            {
                //因为存储是从左下角开始存储的，所以从头遍历是oK的
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

        //纵向检测
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
    /// 将遍历标记完的地图，筛选出符合要求的点
    /// </summary>
    void SearchPoint()
    {
        //1.首先找到地图中一个不为0的点，设置为当前点
        //2.使用DFS/BFS 找到这个点的连通的所有点，遍历的时候每拿到一个点就将标记清除
        //3.从当前点开始遍历，再找到一个合适点，直到遍历完成
        GemsItem g = null;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (mapFlag[i,j] >= 1)
                {
                    //就从当前点开始查找
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

        // 初始化队列
        queue.Enqueue(new Vector2Int(x,y));
        mapFlag[x, y] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            matches.Add(current);

            foreach (Vector2Int dir in Utils.directions)
            {
                Vector2Int next = current + dir;

                // 边界检查
                if (next.x < 0 || next.x >= GameCfg.row) continue;
                if (next.y < 0 || next.y >= GameCfg.col) continue;

                // 类型检查 && 访问标记 
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
    /// 清除宝石
    /// </summary>
    IEnumerator MergeGems(bool isBombMerge = false)
    {
        HashSet<Vector2Int> gems;
        bool isPlayEffectTxt = false;
        for (int i = 0; i < gemsItems.Count; i++)
        {
            gems = gemsItems[i];
            //根据数量形状判断生成炸弹
            if (!isBombMerge) {
                //不是炸弹的消除才需要执行炸弹的判断
                bombManager.SwitchBomb(gems.Count,gems.ToArray(),gemsItemsCollect, bombItems);
            }
            isPlayEffectTxt = true; 
            //先清除棋盘上的gem和播放特效
            foreach (var item in gems)
            {
                if (isPlayEffectTxt)
                {
                    //生成一个物体飞到旁边，生成一个分数特效文字
                    //特效文字
                    EffectManager.Instance.CreateEffectTextItem(100 * gems.Count, Utils.GetNGUIPos(item.x), UIManager.Instance.UIRoot);
                    //非炸弹合并时执行
                    if (!isBombMerge)
                        this.CreateFlyGemItem(gemMergeInfos[i]);
                    isPlayEffectTxt = false;
                }
                //Debug.Log(gemsItemsCollect[item.x, item.y]);
                gemsItemsCollect[item.x,item.y]?.PlayMergeEffect();
            }

            if (isBombMerge)
            {
                //炸弹合并消除时需要另一种方式,每颗炸弹消除的物体类型不一样，需要将消除的类型一一显示
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
            //如果是炸弹合并需要先排序，再合并
            if (isBombMerge) {
                var sortedEnumerable = gems.OrderBy(v => v.x).ThenBy(v => v.y);
                //再整理棋盘
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

        //清空合并信息
        System.Array.Clear(mapFlag, 0, mapFlag.Length);
        gemsItems.Clear();

        //清除缓存的各类型的分数信息
        gemMergeInfos.Clear();
        bombMergeInfo.Clear();

        yield return ws08;
        if (gemMergeCoroutione != null)
        {
            StopCoroutine(gemMergeCoroutione);
            gemMergeCoroutione = null;
        }
        //如果生成炸弹了,先处理炸弹
        if (bombItems.Count > 0)
        {
            for (int i = 0; i < bombItems.Count; i++)
            {
                //处理对应炸弹功能
                bombManager.HandlerBomb(bombItems[i], gemsItemsCollect, gemsItems,bombItems,bombMergeInfo);
            }
            //清除炸弹信息
            bombItems.Clear();
            //炸弹将所有需要移除的物体添加后，执行合并操作
            gemMergeCoroutione = StartCoroutine(MergeGems(true));
        }
        else
        {
            //没有生成炸弹整理完成之后就进行检测
            DetectMergeGems();
        }
    }

    //生成一个飞行物体飞到旁边
    void CreateFlyGemItem(MergeInfo mergeInfo)
    {
        //增加从一个飞行物体飞到指定位置
        scoreList.AddItem(mergeInfo);
        GameCfg.comboNum++;
        EventCenter.Instance.ExcuteEvent(EventNum.ShowComboLabel);
    }

    void MergeGemAndMove(int x, int y) 
    {
        int xIdx = x;
        //如g是第二行的元素，要从第三行、第四行将元素下移
        for (int i = x; i > 0; i--)
        {
            if (Utils.IsCorner(new Vector2Int(i, y))) continue;
            //得到上一行的GemItem
            GemsItem g1 = gemsItemsCollect[i-1,y];
            if (g1 == null)
            {
                this.ReplenishGem(i, y, Utils.GetStartPos(0, y), false);
                continue;
            }
            //将得到的GemItem赋值给下一行
            gemsItemsCollect[i,y] = g1;
            g1.Idx = new Vector2Int(xIdx, y);
            //清空原位置数据
            gemsItemsCollect[i - 1, y] = null;
            g1.TweenTOPosition();
            xIdx--;
        }
        bool isCreateBomb = false;
        //补充新的GemsItem到顶部位置
        this.ReplenishGem(0, y, Utils.GetStartPos(0, y), isCreateBomb);
    }

    /// <summary>
    /// 补充新的Gem
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
