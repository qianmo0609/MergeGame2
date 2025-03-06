using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResManager : Singleton<ResManager>
{
    //这里简单将后续要用到的预制体加载进来
    public UIRoot uiRootPrefab;
    public GameObject slotPrefab; //普通墙的预制体
    public GameObject slotClaybankPrefab; //棕黄色墙的预制体
    public GameObject slotBGPrefab; //墙背景预制体
    
    public GameObject wall;
    public GameObject buttomWall;
    public Sprite[] gemsSprites;
    public Sprite[] comboSprites;
    public Sprite bombSprite;
    public GameObject[] effcts;
    public GameObject effectBomb;
    public EffectTextItem effectTextItems;
    public ScoreListItem scoreListItem;
    public Material customSpriteMat;
    public Dictionary<string, UIBase> uiWinsPrefab;
    public Sprite[] levelSprite;
    public override void OnInit()
    {
        base.OnInit();
        this.Onload();
    }
    public void Onload()
    {
        uiRootPrefab = Resources.Load<UIRoot>(ConstValue.uiRootPath);
        slotBGPrefab = Resources.Load<GameObject>(ConstValue.slotBGPath);
        slotPrefab = Resources.Load<GameObject>(ConstValue.slotPath);
        slotClaybankPrefab = Resources.Load<GameObject>(ConstValue.slotClaybankPath);
        this.OnLoadUIWindows();
        this.OnLoadSprite();


        //effectTextItems = Resources.Load<EffectTextItem>($"Res/Prefabs/EffectText");
        //wall = Resources.Load<GameObject>("Res/Prefabs/Wall");
        //buttomWall = Resources.Load<GameObject>("Res/Prefabs/ButtomWall");
        //effectBomb = Resources.Load<GameObject>("Res/Prefabs/Effect/elem_bomb_0");
        //scoreListItem = Resources.Load<ScoreListItem>("Res/Prefabs/ScoreItem");
        //bombSprite = Resources.Load<Sprite>("Res/lhdb/lhdb_ui_gems/gem_bomb");
        //customSpriteMat = Resources.Load<Material>("Res/Mat/CustomSpriteClip");

        //this.OnLoadEffct();
        //this.OnLoadComboSprites();
        //this.OnLoadLevelSprite();
    }

    void OnLoadSprite()
    {
        StringBuilder sb = new StringBuilder(50);
        gemsSprites = new Sprite[5];
        for (int i = 0; i < 5; i++)
        {
            sb.Clear();
            sb.Append(ConstValue.gemItemPath);
            sb.Append(i);
            gemsSprites[i] = Resources.Load<Sprite>(sb.ToString());
        }
    }

    void OnLoadEffct()
    {
        effcts = new GameObject[5];
        for (int i = 1; i < 6; i++)
        {
           effcts[i-1] = Resources.Load<GameObject>($"Res/Prefabs/Effect/elem_eli_{i}_0");
        }
    }

    void OnLoadComboSprites()
    {
        comboSprites = new Sprite[10];
        for (int i = 0; i < 10; i++)
        {
            comboSprites[i] = Resources.Load<Sprite>($"Res/lhdb/lhdb_font_combo/{i}");
        }
    }

    void OnLoadUIWindows()
    {
        uiWinsPrefab = new Dictionary<string, UIBase>();
        uiWinsPrefab.Add(typeof(MainUI).Name,Resources.Load<MainUI>(ConstValue.mainUIPath));
    }

    void OnLoadLevelSprite()
    {
        levelSprite = new Sprite[3];
        levelSprite[0] = Resources.Load<Sprite>("Res/lhdb/lhdb_ui_main/h5by_xyx_dyg");
        levelSprite[1] = Resources.Load<Sprite>("Res/lhdb/lhdb_ui_main/h5by_xyx_deg");
        levelSprite[2] = Resources.Load<Sprite>("Res/lhdb/lhdb_ui_main/h5by_xyx_dsg");
    }
    public void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
