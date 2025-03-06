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
    //Stack<GemsItem> mergeItemCollect; //储藏需要消除的宝石

    HashSet<GemsItem> mergeItemCollect; //储藏需要消除的宝石;

    ScoreList scoreList;

    Dictionary<int,MergeInfo> gemMergeInfos;

    int bombNumEatchRound = 1;
    Stack<GemsItem> bombCollecion;

    private bool[,] visited;// 检测标记数组

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
                GemsItem gemItem = CreateOneGemItem(Utils.GetStartPos(j, i), i <= 1 ? DirEnum.left : DirEnum.right, new Vector2Int(j, i),isDelay:true);
                gemsItemsCollect[j, i] = gemItem;
            }
        }
        yield return ws02;
        //生成完宝石开始检测
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
            //需要按概率生成
            int spriteIdx = Utils.getGemsIdx(Utils.RandomIntVale(0, 10001));
            gemItem.OnInitInfo(isCreateBomb?ResManager.Instance.bombSprite:ResManager.Instance.gemsSprites[spriteIdx], spriteIdx, dir, idx,isCreateBomb);
        }
        gemItem.TweenTOPosition(isDelay:isDelay);
        return gemItem;
    }

    /// <summary>
    /// 宝石开始随机往上在下落
    /// </summary>
    void StartRandomFull()
    {
        if (GameCfg.gameState != GameState.idle) { Debug.Log(ConstValue.tips); return; }
        gemRandomFullCoroutine = StartCoroutine(RandomFull());
        //如果不是在挂机状态下，每次点击都需要禁用掉开始按钮
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
        //清空分数显示
        //scoreList.OnRestInfo();
        //掉落完成之后要清除这部分宝石
        System.Array.Clear(gemsItemsCollect, 0, gemsItemsCollect.Length);
        //重置每轮炸弹数
        this.bombNumEatchRound = 1;
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
        //如果游戏结束了，就不需要检测了
        if (GameCfg.gameState == GameState.gameOver) return;
        //如果检测到有可以清除的宝石，执行清除方法
        if (DetectGemsMethod())
        {
            GameCfg.comboNum++;
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
            else
            {
                //如果不是挂机状态，则需要开启StartBtn
                GameCfg.isEnableBtnStart = true;
            }
        }
    }

    /// <summary>
    /// 检测时候可以消除的宝石方法
    /// </summary>
    /// <returns></returns>
    public bool DetectGemsMethod()
    {
        bool isMatch = false;
        int matchNum = 0;
        int gemType = -1;

        #region 暴力检测法
        //横向检测
        for (int i = 0; i < GameCfg.row; i++)
        {
            matchNum = 0;
            gemType = -1;
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (Utils.IsCorner(new Vector2Int(i,j))) continue;
                //因为存储是从左下角开始存储的，所以从头遍历是oK的
                //g1 = gemsItemsCollect[i, j];
                //g2 = gemsItemsCollect[i, j + 1];
                //g3 = gemsItemsCollect[i, j + 2];
                //if ((g1.GemType & g2.GemType & g3.GemType) != 0)//& gemsItemsCollect[4 * i + j + 3].GemType) != 0)
                //{
                //    //那么说明这个三个宝石的类型是一样的
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
        //纵向检测
        for (int i = 0; i < GameCfg.row; i++)
        {
            matchNum = 0;
            gemType = -1;
            for (int j = 0; j < GameCfg.col; j++)
            {
                if (Utils.IsCorner(new Vector2Int(i, j))) continue;
                //num = 0;
                ////因为存储是从左下角开始存储的，所以从头遍历是oK的
                //g1 = gemsItemsCollect[i, j];
                //g2 = gemsItemsCollect[i + 1, j];
                //g3 = gemsItemsCollect[i + 2, j];
                //if ((g1.GemType & g2.GemType & g3.GemType) != 0)//& gemsItemsCollect[i + j * 4 + 3].GemType) != 0)
                //{
                //    //那么说明这个三个宝石的类型是一样的
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
    /// 清除宝石
    /// </summary>
    IEnumerator MergeGems()
    {
        //生成一个物体飞到旁边，生成一个分数特效文字
        //Dictionary<int, MergeInfo>.ValueCollection merges = gemMergeInfos.Values;
        //foreach (var item in merges)
        //{
        //    EffectManager.Instance.CreateEffectTextItem(item.score, Utils.GetNGUIPos(item.row), UIManager.Instance.UIRoot);
        //    this.CreateFlyGemItem(item);
        //    yield return new WaitForSeconds(.1f);
        //}
        //先清除棋盘上的gem和播放特效
        foreach (var item in mergeItemCollect)
        {
            item.PlayMergeEffect();
        }
        //yield return new WaitForSeconds(.5f);
        //再整理棋盘
        foreach (var item in mergeItemCollect)
        {
            MergeGemAndMove(item.Idx.x, item.Idx.y);
            item.RecycleSelf();
        }

        //清空合并信息
        mergeItemCollect.Clear();

        //清除缓存的各类型的分数信息
        //gemMergeInfos.Clear();

        //如果生成炸弹了,先处理炸弹
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
        //整理完成之后再进行检测
        DetectMergeGems();
    }

    //生成一个飞行物体飞到旁边
    void CreateFlyGemItem(MergeInfo mergeInfo)
    {
        //增加本地数据记录
        LocalData.Instance.AddScoreData(new ScoreData { type = mergeInfo.type, num = mergeInfo.num});
        //增加从一个飞行物体飞到指定位置
        scoreList.AddItem(mergeInfo);
    }

    void MergeGemAndMove(int x, int y) 
    {
        int xIdx = x;
        //如g是第二行的元素，要从第三行、第四行将元素下移
        //Sequence sequence = DOTween.Sequence();
        for (int i = x; i > 0; i--)
        {
            if (Utils.IsCorner(new Vector2Int(i-1, y)) || Utils.IsCorner(new Vector2Int(i, y))) continue;
            //得到上一行的GemItem
            GemsItem g1 = gemsItemsCollect[i-1,y];
            //清空原位置数据
            gemsItemsCollect[i-1,y] = null;
            //将得到的GemItem赋值给下一行
            gemsItemsCollect[i,y] = g1;
            g1.Idx = new Vector2Int(xIdx, y);
            //将GemItem滑动到下一行
            //sequence.Join(g1.TweenTOPosition());
            g1.TweenTOPosition();
            xIdx--;
        }
        //sequence.Play();
        bool isCreateBomb = false;
        //先计算炸弹概率，是否需要生成炸弹
        //if (this.CalacBombPercentage())
        //{
        //    //如果小于30，则说明本次生成的是炸弹
        //    //this.CreateBomb(Utils.GetCurrentPos(0, y), 0, y);
        //    isCreateBomb = true;
        //    //将生成的炸弹数减1
        //    this.bombNumEatchRound--;
        //}
        //补充新的GemsItem到顶部位置
        this.ReplenishGem(0, y, Utils.GetStartPos(0, y), isCreateBomb);
    }

    void CreateBomb(Vector3 curPos,int x, int y)
    {
        Bomb b = CreateFactory.Instance.CreateGameObj<Bomb>(GameObjEunm.bomb);
        //b.OnInitInfo(new MergeInfo { row = x,col = y}, gameMap.GetCurrentWallPos(), ResManager.Instance.bombSprite, this.BombCb,true);
    }

    /// <summary>
    /// 炸弹执行完后的回调函数
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
    /// 计算炸弹概率
    /// </summary>
    bool CalacBombPercentage()
    {
        //需要计算产生新的Gem还是炸弹，如果是产生炸弹，则需要先生成炸弹再生成新的Gem，新产生的炸弹目前默认是在第0行，如需在任意位置则需要修改此时行列值
        if (this.bombNumEatchRound > 0 &&  Utils.RandomFloatVale(0.0f, GameCfg.bombPercentageDenominator) < GameCfg.bombPercentageNumerator)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 补充新的Gem
    /// </summary>
    void ReplenishGem(int x, int y, Vector3 curPos, bool isCreateBomb = false)
    {
        if (Utils.IsCorner(new Vector2Int(x, y))) return;
        GemsItem gNew = CreateOneGemItem(curPos, y < 3 ? DirEnum.left : DirEnum.right, new Vector2Int(0, y),isCreateBomb);
        gemsItemsCollect[0,y] = gNew;
        //如果是炸弹则添加
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
