using UnityEngine;

public class Utils 
{
    public static int RandomIntVale(int minValue,int maxValue)
    {
        return Random.Range(minValue,maxValue);
    }

    public static float RandomFloatVale(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }

    public static Vector3 GetGemItemPos(int row , int col)
    {
        return new Vector3(GameCfg.offsetX * col, -GameCfg.offsetY * (row - 1), 0);
    }

    public static Vector3 GetNextPos(int row,int col)
    {
       return GetCurrentPos(row,col) + Vector3.down * GameCfg.offsetY;
    }

    public static Vector3 GetCurrentPos(int row, int col)
    {
        return GameCfg.startPoss + GetGemItemPos(row, col);
    }

    public static Vector3 GetStartPos(int row, int col)
    {
        return GameCfg.startPoss + new Vector3(GameCfg.offsetX * col, GameCfg.offsetY * 5, 0);
    }

    public static Vector3 GetNGUIPos(int row)
    {
        return Vector3.up * (GameCfg.uiYStartPos[GameCfg.level - 1] - row * GameCfg.uiYInterval);
    }

    public static Vector3 WorldTONGUIPos(Vector3 pos,Camera uiCamera)
    {
        // 步骤1: 世界坐标 → 屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        // 步骤2: 屏幕坐标 → NGUI坐标
        Vector3 uiPos = uiCamera.ScreenToWorldPoint(screenPos);
        return uiCamera.transform.TransformPoint(uiPos);
    }

    // 计算二次贝塞尔曲线上的点
    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public static int getGemsIdx(int percentNum)
    {
        for (int i = 0; i < GameCfg.gemsPercentages.Length; i++)
        {
            if(percentNum <= GameCfg.gemsPercentages[i])
            {
                return i;
            }
        }
        return 0;
    }

    public static bool IsCorner(int x, int y, int maxX, int maxY)
    {
        // 利用位运算优化条件判断
        bool isEdgeX = (x & (maxX - 1)) == 0; // maxX应为2的幂时生效
        bool isEdgeY = (y & (maxY - 1)) == 0;
        return (x == 0 || x == maxX) && (y == 0 || y == maxY);
    }

    public static bool IsCorner(Vector2Int key)
    {
        bool isCreateSlot = false;
        GameCfg.claybankSlotIdxs.TryGetValue(key, out isCreateSlot);
        return isCreateSlot;
    }
}