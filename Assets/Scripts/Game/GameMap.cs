using UnityEngine;

public class GameMap
{
    GameObject grid = null;
    GameObject bg = null;

    Vector2Int key;

    public GameObject Bg { get => bg; }

    public void OnInitLayout(GameObject grid)
    {
        this.grid = grid;
        this.CreateBG();
        this.CreteSlot(grid);
        key = new Vector2Int();
    }

    void CreateBG()
    {
        bg = ResManager.Instance.InstantiateObj<GameObject>(GameObjEunm.bg);
    }

    void CreteSlot(GameObject grid)
    {
        GameObject slot;
        Vector3 startPos = GameCfg.startPoss;
        for (int i = 0; i < GameCfg.row; i++)
        {
            for (int j = 0; j < GameCfg.col; j++)
            {
                slot = GameObject.Instantiate<GameObject>(this.CreateSlotOrSlotClaybank(i,j)?ResManager.Instance.slotClaybankPrefab:ResManager.Instance.slotPrefab);
                slot.transform.SetParent(grid.transform);
                slot.transform.position = startPos - new Vector3(-GameCfg.offsetX * i,GameCfg.offsetY * j, 0);
            }
        }
    }

    bool CreateSlotOrSlotClaybank(int i, int j)
    {
        key.Set(i, j);
        return Utils.IsCorner(key);
    }

    public void OnDestroy()
    {
        grid = null;
        bg = null;
    }
}
