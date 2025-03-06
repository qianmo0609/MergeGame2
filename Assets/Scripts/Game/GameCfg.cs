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
    public static Dictionary<Vector2Int,bool> claybankSlotIdxs = new Dictionary<Vector2Int, bool> {
    { new Vector2Int(0,0),true},{new Vector2Int(0,1),true},{new Vector2Int(1,0),true }, //����
    { new Vector2Int(0,7),true},{new Vector2Int(0,8),true},{new Vector2Int(1,8), true},//����
    { new Vector2Int(7,0),true},{new Vector2Int(8,0),true},{new Vector2Int(8,1),true } ,//����
    { new Vector2Int(7,8),true},{new Vector2Int(8,7),true},{new Vector2Int(8,8),true }  //����
    };

    public static int totalScore = 0;
    public static int comboNum = 0;

    public static bool isEnableBtnStart = false;

    public static Vector2Int[] gameLayout = new Vector2Int[3] { new Vector2Int(4, 4), new Vector2Int(5, 5), new Vector2Int(6, 6) };

    public static int[] gridPerRow = new int[3] { 4, 5, 6 };

    public static int level = 1;

    public static bool isHandUp = false; //�Ƿ����

    //public static Vector3[] startPoss = new Vector3[] { new Vector3(-1f, 0.03f, 0), new Vector3(-1f, 0.68f, 0), new Vector3(-1.68f, 1.313f, 0) };

    public static GameState gameState = GameState.idle; //�Ƿ�����ƥ��

    public static Vector3[] buttomWallStartPos = new Vector3[3] { new Vector3(-1.85f, -2.378f, 0), new Vector3(-1.85f, -2.378f, 0), new Vector3(-2.49f, -2.378f, 0) };
    public static int[] buttomWallNum = new int[3] { 16, 19, 21 };

    public static Vector3[,] wall = new Vector3[,] { { new Vector3(-1.669f, -2.057f, 0), new Vector3(1.709f, -2.057f, 0) }, { new Vector3(-1.669f, -2.057f, 0), new Vector3(2.36f, -2.057f, 0) }, { new Vector3(-2.33f, -2.057f, 0),new Vector3(2.36f, -2.057f, 0)} };

    public static float uiYInterval = 75f;

    public static float[] uiYStartPos = new float[] { 0, 0, 0 };

    public static float scoreListItemInterval = .5f;

    public static int scoreListItemMaxNum = 6;

    public static Vector3[] scoreListStartPoss = new Vector3[] { new Vector3(0, -0.154f,0), new Vector3(0, 0, 0) , new Vector3(0, 0, 0) };

    public static float scoreListNumDoubleX = 0.565f;

    public static float scoreListNumSingleX = 0.488f;

    public static int wallNum = 15;

    public static float flyTOPosOffsetX = -0.2686f;

    public static float flyBezierOffsetY = 3f;

    public static Vector4 spriteClipRange = new Vector4(-5,-3,-1.8f,1.15f);
    public static Vector4 spriteRange = new Vector4(-10,10,-10,10);

    public static float bombPercentageDenominator = 100; //ը�����ʵķ�ĸ
    public static float bombPercentageNumerator = 30;  //ը�����ʵķ���

    public static int[] gemsPercentages = new int[5] { 2000,4000,6000,8000,10000};
}

public static class ConstValue
{
    public static string btnStartNormalSpriteName = "h5by_xyx_ks"; //��ʼ��ť��������ʾ������
    public static string btnStartHandUpSpriteName = "h5by_xyx_qxgj"; //��ʼ��ť�ڹһ�ʱ��ʾ�ľ�����
    public static string btnStartDisableSpriteName = "h5by_xyx_hsks"; //��ʼ��ť�ڽ�����ʾ�ľ�����
    
    public static string btnHandUpNormalSpriteName = "h5by_xyx_gj"; //�һ���ť������ʱ��ʾ�ľ�����
    public static string btnHandUpHandUpSpriteName = "h5by_xyx_gjz"; //�һ���ť�ڹһ�ʱ��ʾ�ľ�����   

    public static string symbolX = "x";

    public static string tips = "����У������ظ������"; //�����ʾ��Ϣ

    public static string alphaCoeff = "_alphaCoeff";
    public static string Rect = "_Rect";


    public static string uiRootPath = "Res/Prefabs/UIRoot";
    public static string mainUIPath = "Res/Prefabs/UI/GamePanel";
    public static string slotPath = "Res/Prefabs/Slot";
    public static string slotClaybankPath = "Res/Prefabs/SlotClaybank";
    public static string slotBGPath = "Res/Prefabs/BG";
    public static string gemItemPath = "Res/GameSprite/GameItem/sxyx_main sxyx_main_";

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
