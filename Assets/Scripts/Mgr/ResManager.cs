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
    public GemsItem gemItemPrefab;
    public Bomb bombItemPrefab;
    public EffectFlyItem effectFlyItemPrefab;
    public ScoreListItem scoreListItem;
    public EffectTextItem effectTextItems;
    public EffectItem[] effcts;
    public Sprite[] gemsSprites;
    public Sprite[] comboSprites;
    public Sprite[] bombSprites;
    public Dictionary<string, UIBase> uiWinsPrefab;

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
        effectTextItems = Resources.Load<EffectTextItem>(ConstValue.effectItemPath);
        scoreListItem = Resources.Load<ScoreListItem>(ConstValue.scoreListItemPath);

        gemItemPrefab = Resources.Load<GemsItem>(ConstValue.gemPrefabPath);
        bombItemPrefab = Resources.Load<Bomb>(ConstValue.bombItemPrefabPath);
        effectFlyItemPrefab = Resources.Load<EffectFlyItem>(ConstValue.effectFlyItemPrefabPath);

        this.OnLoadUIWindows();
        this.OnLoadSprite();
        this.OnLoadBombSprite();
        this.OnLoadEffct();
        this.OnLoadComboSprites();
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
    
    void OnLoadUIWindows()
    {
        uiWinsPrefab = new Dictionary<string, UIBase>();
        uiWinsPrefab.Add(typeof(MainUI).Name, Resources.Load<MainUI>(ConstValue.mainUIPath));
    }

    void OnLoadBombSprite()
    {
        bombSprites = new Sprite[4];
        bombSprites[0] = Resources.Load<Sprite>(ConstValue.horBombPath);
        bombSprites[1] = Resources.Load<Sprite>(ConstValue.verBombPath);
        bombSprites[2] = Resources.Load<Sprite>(ConstValue.superBombPath);
        bombSprites[3] = Resources.Load<Sprite>(ConstValue.largeBombPath);
    }

    void OnLoadEffct()
    {
        effcts = new EffectItem[5];
        for (int i = 1; i < 6; i++)
        {
           effcts[i-1] = Resources.Load<EffectItem>(string.Format(ConstValue.gemEffectPath,i));
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

    #region 实例化
    public T CreateGameObj<T>(GameObjEunm type, int id = 0) where T : class
    {
        switch (type)
        {
            case GameObjEunm.gemItem:
                return PoolManager.Instance.gemsPool.getObjFromPool(type, id) as T;
            case GameObjEunm.effectTextItem:
                return PoolManager.Instance.EffectTextPool.getObjFromPool(type, id) as T;
            case GameObjEunm.effectItem:
                return PoolManager.Instance.EffectItemDic[id].getObjFromPool(type, id) as T;
            case GameObjEunm.effectFlyItem:
                return PoolManager.Instance.EffFlyItemPool.getObjFromPool(type, id) as T;
            case GameObjEunm.loopListItem:
                return PoolManager.Instance.LoopListItemPool.getObjFromPool(type, id) as T;
            case GameObjEunm.scoreListItem:
                return PoolManager.Instance.ScoreListItemPool.getObjFromPool(type, id) as T;
            case GameObjEunm.bomb:
                return PoolManager.Instance.BombItemPool.getObjFromPool(type, id) as T;
            case GameObjEunm.bombEffct:
                return PoolManager.Instance.BombEffctPool.getObjFromPool(type, id) as T;
            default:
                return null;
        }
    }

    public T GetObjPrefab<T>(GameObjEunm type, int id = 0) where T : class
    {
        switch (type)
        {
            case GameObjEunm.gemItem:
                return gemItemPrefab as T;
            case GameObjEunm.bomb:
                return bombItemPrefab as T;
            case GameObjEunm.effectItem:
                return effcts[id - 1] as T;
            case GameObjEunm.effectTextItem:
                return effectTextItems as T;
            case GameObjEunm.effectFlyItem:
                return effectFlyItemPrefab as T;
            case GameObjEunm.scoreListItem:
                return scoreListItem as T;
            case GameObjEunm.bg:
                return slotBGPrefab as T;
            case GameObjEunm.uiRoot:
                return uiRootPrefab as T;
            default:
                return null;
        }
    }

    public T InstantiateObj<T>(GameObjEunm type) where T : class
    {
        switch (type)
        {
            case GameObjEunm.bg:
                return GameObject.Instantiate<GameObject>(slotBGPrefab) as T;
        }
        return null;
    }

    public T InstantiateMonoObj<T>(GameObjEunm type, int id = 0) where T : MonoBehaviour
    {
        return GameObject.Instantiate<T>(this.GetObjPrefab<T>(type, id));
    }

    #endregion

    #region 回收
    public void PutObjToPool<T>(GameObjEunm type, T t, int id = 0) where T : class
    {
        switch (type)
        {
            case GameObjEunm.gemItem:
                PoolManager.Instance.gemsPool.putObjToPool(t as GemsItem);
                break;
            case GameObjEunm.bomb:
                PoolManager.Instance.BombItemPool.putObjToPool(t as Bomb);
                break;
            case GameObjEunm.effectItem:
                PoolManager.Instance.EffectItemDic[id].putObjToPool(t as EffectItem);
                break;
            case GameObjEunm.effectTextItem:
                PoolManager.Instance.EffectTextPool.putObjToPool(t as EffectTextItem);
                break;
            case GameObjEunm.effectFlyItem:
                PoolManager.Instance.EffFlyItemPool.putObjToPool(t as EffectFlyItem);
                break;
            case GameObjEunm.loopListItem:
                PoolManager.Instance.LoopListItemPool.putObjToPool(t as LoopListItem);
                break;
            case GameObjEunm.scoreListItem:
                PoolManager.Instance.ScoreListItemPool.putObjToPool(t as ScoreListItem);
                break;
            case GameObjEunm.bombEffct:
                PoolManager.Instance.BombEffctPool.putObjToPool(t as BombEffctItem);
                break;
            default:
                break;
        }
    }
    #endregion


    public void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
