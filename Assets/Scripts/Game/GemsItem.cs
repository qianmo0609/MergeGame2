using DG.Tweening;
using UnityEngine;
public enum DirEnum
{
    left,
    right
}
public class GemsItem : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    int gemType;
    int type;
    DirEnum dirEnum;
    [SerializeField]
    Vector2Int idx; //用于保存物体在什么位置 x表示的是行，y表示的是列
    BombType isBomb;

    FullComponent fullComponent;

    public int GemType { get => gemType; }
    public DirEnum _DirEnum { get => dirEnum; }
    public Vector2Int Idx { get => idx; set => idx = value; }
    public int Type { get => type;}
    public BombType IsBomb { get => isBomb; set{isBomb = value; this.OnUpdateTOBomb(value); } }
    public bool IsFull { 
        get { return isFull; } 
        set { 
            isFull = value;
            fullComponent.UpdateInfo();
        } 
    }

    Vector3 currentPos;

    bool isFull = false;

    private void Start()
    {
        fullComponent = new FullComponent(this.transform);
    }

    public void OnInitInfo(Sprite gemIcon, int type, DirEnum dirEnum, Vector2Int idx, BombType isBomb = BombType.none)
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = gemIcon;
        this.spriteRenderer.sortingOrder = 4;
        this.gemType = 1 << type;
        this.type = type;
        this.dirEnum = dirEnum;
        this.idx = idx;
        this.isBomb = isBomb;
    }

    public void Update()
    {
        if (this.isFull)
        {
            fullComponent?.Update(this.UpdateCB);
        }   
    }

    void UpdateCB()
    {
        this.isFull = false;
        this.RecycleSelf();
    }

    public Tween TweenTOPosition(float duration = .2f)
    {
        this.transform.DOComplete();
        currentPos = Utils.GetNextPos(this.idx.x,this.idx.y);
        return this.transform.DOMove(currentPos, duration).SetEase(Ease.OutBounce);
    }

    void OnUpdateTOBomb(BombType bombType)
    {
        //无类型的直接返回
        if (bombType == BombType.none) return;
        //首先需要播放一个放缩动画
        //vibrato 震动 elasticity 弹性 都不需要
        this.transform.DOPunchScale(Vector3.one * 1.2f,.5f,0,0).SetEase(Ease.OutSine);
        //将图片换成对应的炸弹图片
        this.spriteRenderer.sprite = ResManager.Instance.bombSprites[((int)bombType) - 1]; 
    }

    public void PlayMergeEffect()
    {
        if(isBomb == BombType.none) {
            this.transform.position = new Vector3(10000, 10000, 0);
        }
        //播放爆炸特效动画
        EffectManager.Instance.CreateEffectItem(this.type+1, currentPos);
    }

    /// <summary>
    /// 将自己回收到对象池
    /// </summary>
    /// <param name="p"></param>
    public void RecycleSelf()
    {
        //回收炸弹时需要将类型重置为none
        if (this.isBomb != BombType.none)
        {
            this.isBomb = BombType.none;
        }
        this.transform.parent = null;
        this.transform.position = new Vector3(10000, 10000, 0);
        this.idx = Vector2Int.down;
        this.transform.DOKill();
        PoolManager.Instance.gemsPool.putObjToPool(this);
        this.isFull = false;
    }
}
