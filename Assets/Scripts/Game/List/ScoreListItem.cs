using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreListItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer icon;
    [SerializeField] SpriteRenderer symbol;
    [SerializeField] SpriteRenderer scoreShi;
    [SerializeField] SpriteRenderer scoreGe;

    // �洢�������Կ�
    private MaterialPropertyBlock materialPropertyBlock1;
    private MaterialPropertyBlock materialPropertyBlock2;
    private MaterialPropertyBlock materialPropertyBlock3;
    private MaterialPropertyBlock materialPropertyBlock4;

    FullComponent fullComponent;

    float alphaV = 1;

    Coroutine fadeCoroutine = null;

    bool isFull = false;

    public bool IsFull { get { return this.isFull; } set { this.isFull = value;  } }

    private void OnEnable()
    {
        materialPropertyBlock1 = new MaterialPropertyBlock();
        materialPropertyBlock2 = new MaterialPropertyBlock();
        materialPropertyBlock3 = new MaterialPropertyBlock();
        materialPropertyBlock4 = new MaterialPropertyBlock();
        fullComponent = new FullComponent(this.transform);
        fullComponent.UpdateInfo();
    }

    private void Update()
    {
        if (this.isFull)
        {
            fullComponent.Update(this.UpadteCB);
        }
    }

    void UpadteCB()
    {
        this.isFull = false;
        this.OnRecycleSelf();
    }

    public void OnSetInfo(Sprite icon, int num)
    {
        this.icon.sprite = icon;
        //alphaV = 0;
        //this.OnSetRenderInfo();
        Vector3 pos = scoreGe.transform.localPosition;
        int shiValue = num / 10;
        if (shiValue > 0)
        {
            //�������λ��
            scoreShi.gameObject.SetActive(true);
            pos.x = GameCfg.scoreListNumDoubleX;
            //����ʮλ����
            scoreShi.sprite = ResManager.Instance.comboSprites[shiValue - 1];
        }
        else
        {
            //�����һλ��
            scoreShi.gameObject.SetActive(false);
            pos.x = GameCfg.scoreListNumSingleX;
        }
        scoreGe.transform.localPosition = pos;
        //���ø�λ����
        scoreGe.sprite = ResManager.Instance.comboSprites[num % 10];
        
        //fadeCoroutine = StartCoroutine(this.OnFade());
    }

    IEnumerator OnFade()
    {
        while (alphaV <= 1)
        {
            alphaV += 0.1f;
            yield return new WaitForSeconds(0.1f);
            this.OnSetRenderInfo();
        }
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    void OnSetRenderInfo()
    {
        this.OnSetMatInfo(materialPropertyBlock1, icon, alphaV);
        this.OnSetMatInfo(materialPropertyBlock2, symbol, alphaV);
        this.OnSetMatInfo(materialPropertyBlock3, scoreGe, alphaV);
        this.OnSetMatInfo(materialPropertyBlock4, scoreShi, alphaV);
    }

    void OnSetMatInfo(MaterialPropertyBlock materialPropertyBlock,SpriteRenderer rendererComponent,float value)
    {
        rendererComponent.GetPropertyBlock(materialPropertyBlock);
        // ���� _alphaCoeff ���Ե�ֵ
        materialPropertyBlock.SetFloat(ConstValue.alphaCoeff, value);
        // ���޸ĺ�����Կ�Ӧ�õ���Ⱦ����
        rendererComponent.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetItemState(bool isActive, Vector4 Range)
    {
        this.OnHide(isActive);
        this.OnSetRenderClipRange(Range);
    }

    public void OnHide(bool isActive)
    {
        this.symbol.gameObject.SetActive(isActive);
        this.scoreShi.gameObject.SetActive(isActive);
        this.scoreGe.gameObject.SetActive(isActive);
    }

    void OnSetRenderClipRange(Vector4 Range)
    {
        this.OnSetMatClipRange(materialPropertyBlock1, icon, Range);
        this.OnSetMatClipRange(materialPropertyBlock2, symbol, Range);
        this.OnSetMatClipRange(materialPropertyBlock3, scoreGe, Range);
        this.OnSetMatClipRange(materialPropertyBlock4, scoreShi, Range);
    }

    void OnSetMatClipRange(MaterialPropertyBlock materialPropertyBlock, SpriteRenderer rendererComponent, Vector4 Range)
    {
        rendererComponent.GetPropertyBlock(materialPropertyBlock);
        // ���� _alphaCoeff ���Ե�ֵ
        materialPropertyBlock.SetVector(ConstValue.Rect, Range);
        // ���޸ĺ�����Կ�Ӧ�õ���Ⱦ����
        rendererComponent.SetPropertyBlock(materialPropertyBlock);
    }

    /// <summary>
    /// ���Լ�����
    /// </summary>
    public void OnRecycleSelf()
    {
        this.transform.parent = null;
        this.transform.position = new Vector3(10000, 10000, 0);
        this.OnHide(true);
        this.OnSetRenderClipRange(GameCfg.spriteClipRange);
        PoolManager.Instance.ScoreListItemPool.putObjToPool(this);
    }
}
