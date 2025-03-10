using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCfg
{
    public static int row = 9;
    public static int col = 9;

    public static float offsetX = 0.5256f;
    public static float offsetY = 0.534f;

    public static Vector3 startPoss = new Vector3(-1.8958f,1.73f,0);
    public static Dictionary<Vector2Int, bool> claybankSlotIdxs = new Dictionary<Vector2Int, bool> {
    { new Vector2Int(0,0),true},{new Vector2Int(0,1),true},{new Vector2Int(1,0),true }, //左上
    { new Vector2Int(0,7),true},{new Vector2Int(0,8),true},{new Vector2Int(1,8), true},//右上
    { new Vector2Int(7,0),true},{new Vector2Int(8,0),true},{new Vector2Int(8,1),true } ,//左下
    { new Vector2Int(7,8),true},{new Vector2Int(8,7),true},{new Vector2Int(8,8),true }  //右下
    };

    public static float[] uiYStartPos = new float[] { 180, 0, 0 };
    public static float uiYInterval = 60f;

    public static bool isHandUp = false; //是否挂起

    public static float scoreListItemInterval = .5f;
    public static int scoreListItemMaxNum = 4;

    public static int comboNum = 0;

   
    public static int totalScore = 0;
   
    public static bool isEnableBtnStart = false;

    public static Vector2Int[] gameLayout = new Vector2Int[3] { new Vector2Int(4, 4), new Vector2Int(5, 5), new Vector2Int(6, 6) };

    public static int[] gridPerRow = new int[3] { 4, 5, 6 };

    public static int level = 1;

    //public static Vector3[] startPoss = new Vector3[] { new Vector3(-1f, 0.03f, 0), new Vector3(-1f, 0.68f, 0), new Vector3(-1.68f, 1.313f, 0) };

    public static GameState gameState = GameState.idle; //是否正在匹配

    public static Vector3[] buttomWallStartPos = new Vector3[3] { new Vector3(-1.85f, -2.378f, 0), new Vector3(-1.85f, -2.378f, 0), new Vector3(-2.49f, -2.378f, 0) };
    public static int[] buttomWallNum = new int[3] { 16, 19, 21 };

    public static Vector3[,] wall = new Vector3[,] { { new Vector3(-1.669f, -2.057f, 0), new Vector3(1.709f, -2.057f, 0) }, { new Vector3(-1.669f, -2.057f, 0), new Vector3(2.36f, -2.057f, 0) }, { new Vector3(-2.33f, -2.057f, 0),new Vector3(2.36f, -2.057f, 0)} };

    public static Vector3[] scoreListStartPoss = new Vector3[] { new Vector3(0, -0.154f,0), new Vector3(0, 0, 0) , new Vector3(0, 0, 0) };

    public static float scoreListNumDoubleX = 0.565f;

    public static float scoreListNumSingleX = 0.488f;

    public static int wallNum = 15;

    public static float flyTOPosOffsetX = -0.2686f;

    public static float flyBezierOffsetY = 3f;

    public static Vector4 spriteClipRange = new Vector4(-5,-3,-1.8f,1.15f);
    public static Vector4 spriteRange = new Vector4(-10,10,-10,10);

    public static float bombPercentageDenominator = 100; //炸弹概率的分母
    public static float bombPercentageNumerator = 30;  //炸弹概率的分子

    public static int[] gemsPercentages = new int[5] { 2000,4000,6000,8000,10000};
}

public static class ConstValue
{
    public static string btnStartNormalSpriteName = "sxyx_main sxyx_main_92"; //开始按钮的正常显示精灵名
    public static string btnStartHandUpSpriteName = "sxyx_main sxyx_main_78"; //开始按钮在挂机时显示的精灵名
    
    public static string btnHandUpNormalSpriteName = "sxyx_main sxyx_main_67"; //挂机按钮在正常时显示的精灵名
    public static string btnHandUpHandUpSpriteName = "sxyx_main sxyx_main_95"; //挂机按钮在挂机时显示的精灵名   

    public static string symbolX = "x";

    public static string tips = "检测中，请勿重复点击！"; //点击提示信息

    public static string alphaCoeff = "_alphaCoeff";
    public static string Rect = "_Rect";


    public static string uiRootPath = "Res/Prefabs/UIRoot";
    public static string mainUIPath = "Res/Prefabs/UI/GamePanel";
    public static string slotPath = "Res/Prefabs/Slot";
    public static string slotClaybankPath = "Res/Prefabs/SlotClaybank";
    public static string slotBGPath = "Res/Prefabs/BG";
    public static string effectItemPath = "Res/Prefabs/EffectText";
    public static string scoreListItemPath = "Res/Prefabs/ScoreListItem";
    public static string gemItemPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_";
    public static string largeBombPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_97"; //大炸弹
    public static string verBombPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_94"; //竖向炸弹
    public static string horBombPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_88"; //横向炸弹
    public static string superBombPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_96";//超级炸弹
    public static string gemEffectPath = "Res/Prefabs/Effect/elem_eli_{0}_0"; //宝石消除特效
}

public enum GameState
{
    idle,
    isMatching,
    gameOver,
}

public enum GameObjEunm
{
    None,
    gemItem,
    bomb,
    slot,
    slotClaybank,
    effectItem,
    effectTextItem,
    effectFlyItem,
    loopListItem,
    scoreListItem,
    bombEffct,
    bottomWall
}
