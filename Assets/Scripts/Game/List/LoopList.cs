using System.Collections.Generic;
using UnityEngine;

public class LoopList
{
    public UIScrollView scrollView;
    public UIGrid grid;
    public LoopListItem itemPrefab;
    public UIPanel uiPanel;
    public float itemHeight;  // �б���ĸ߶�

    private List<ScoreData> data; //���汾�ط����ĵ�ַ
    private List<LoopListItem> visibleItems = new List<LoopListItem>();  // ��ǰ�ɼ����б���
    private Queue<LoopListItem> itemPool = new Queue<LoopListItem>();  // �б�������

    private int visibleItemCount;  // �ɼ������ڿ����ɵ�����б�������
    private int maxNum;

    private Vector3 topY;
    private Vector3 bottomY;

    private bool isDrag;
    private float currentPosition;
    private float lastPosition;
    private int startIdx;

    public LoopList(UIScrollView scrollView, UIGrid grid, LoopListItem itemPrefab, UIPanel uiPanel, float itemHeight)
    {
        this.scrollView = scrollView;
        this.grid = grid;
        this.itemPrefab = itemPrefab;
        this.uiPanel = uiPanel;
        this.itemHeight = itemHeight;
        this.OnInit();
    }

    public void OnInit()
    {
        topY = grid.transform.TransformPoint(new Vector3(0, uiPanel.height / 2 + scrollView.transform.position.y, 0));
        bottomY = grid.transform.TransformPoint(new Vector3(0, scrollView.transform.position.y - uiPanel.height / 2, 0));
        isDrag = false;
        // ���������¼�
        scrollView.onDragStarted = onDragStart;
        scrollView.onDragFinished = onDragEnd;
        startIdx = 0;
    }

    public void UpdatePanel()
    {
        data = LocalData.Instance.ScoreDatas;
        // ����ɼ������ڿ����ɵ�����б�������
        visibleItemCount = Mathf.CeilToInt(scrollView.panel.height / itemHeight);
        maxNum = visibleItemCount + 1;
        // ��ʼ����ʾ���б���
        InitializeVisibleItems();
    }

    public void OnResetList()
    {
        for (int i = 0; i < visibleItems.Count; i++)
        {
            this.RecycleItem(visibleItems[i]);
        }
    }

    void onDragStart()
    {
        isDrag = true;
    }

    void onDragEnd()
    {
        isDrag = false;
    }

    public void Update()
    {
        if (isDrag)
        {
            currentPosition = grid.transform.position.y;
            // �������Ϲ���
            if (currentPosition > lastPosition)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y > topY.y && startIdx < data.Count - visibleItemCount)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y - visibleItemCount * itemHeight, 0);
                        startIdx++;
                        //�õ����һ���������ȡ���ݣ�Ȼ�����Item��ʾ
                        int endIdx = startIdx + visibleItemCount - 1;
                        if(endIdx < data.Count)
                        {
                            this.UpdateItem(visibleItems[i],endIdx);
                        }
                    }
                }
            }
            // �������¹���
            else if (currentPosition < lastPosition)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y < bottomY.y && startIdx > 0)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y + visibleItemCount * itemHeight, 0);
                        startIdx--;
                        //�õ���һ���������ȡ���ݣ�Ȼ�����Item��ʾ
                        this.UpdateItem(visibleItems[i], startIdx);
                    }
                }
            }
            lastPosition = currentPosition;
        }
    }

    void InitializeVisibleItems()
    {
        int num = Mathf.Min(data.Count,visibleItemCount);
        for (int i = 0; i < num; i++)
        {
            LoopListItem item = GetItemFromPool();
            item.transform.SetParent(grid.transform, false);
            item.gameObject. SetActive(true);
            item.gameObject.name = i.ToString();
            UpdateItem(item, i);
            visibleItems.Add(item);
        }
        scrollView.enabled = data.Count > visibleItemCount;
        grid.Reposition();
    }

    LoopListItem GetItemFromPool()
    {
        if (itemPool.Count > 0)
        {
            return itemPool.Dequeue();
        }
        else
        {
            return GameObject.Instantiate<LoopListItem>(itemPrefab);
        }
    }

    void UpdateItem(LoopListItem item, int index)
    {
        int id = data[index].type + 1;
        item.onInitInfo($"gem_{id}", data[index].num);
    }

    void RecycleItem(LoopListItem item)
    {
        item.gameObject.SetActive(false);
        itemPool.Enqueue(item);
    }
}
