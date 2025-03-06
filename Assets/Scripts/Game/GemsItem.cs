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
    bool isBomb;

    FullComponent fullComponent;

    public int GemType { get => gemType; }
    public DirEnum _DirEnum { get => dirEnum; }
    public Vector2Int Idx { get => idx; set => idx = value; }
    public int Type { get => type;}
    public bool IsBomb { get => isBomb;}
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

    public void OnInitInfo(Sprite gemIcon, int type, DirEnum dirEnum, Vector2Int idx, bool isBomb = false)
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

    public Tween TweenTOPosition(float duration = .2f,bool isDelay = false)
    {
        this.transform.DOComplete();
        currentPos = Utils.GetNextPos(this.idx.x,this.idx.y);
        return this.transform.DOMove(currentPos, duration).SetEase(Ease.OutQuad).SetDelay(isDelay?Random.Range(.1f,.3f):0);
    }

    public void PlayMergeEffect()
    {
        this.transform.position = new Vector3(10000, 10000, 0);
        //播放爆炸特效动画
        //EffectManager.Instance.CreateEffectItem(this.type+1, currentPos);
    }

    /// <summary>
    /// 将自己回收到对象池
    /// </summary>
    /// <param name="p"></param>
    public void RecycleSelf()
    {
        if (this.isBomb)
        {
            this.BombRecycleSelf();
        }
        this.transform.parent = null;
        this.transform.position = new Vector3(10000, 10000, 0);
        this.idx = Vector2Int.down;
        this.transform.DOKill();
        PoolManager.Instance.gemsPool.putObjToPool(this);
        this.isFull = false;
    }

    public void BombRecycleSelf()
    {
        this.isBomb = false;
        this.gameObject.SetActive(true);
    }
}
