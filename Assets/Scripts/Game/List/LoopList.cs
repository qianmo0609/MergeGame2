using System.Collections.Generic;
using UnityEngine;

public class LoopList
{
    public UIScrollView scrollView;
    public UIGrid grid;
    public LoopListItem itemPrefab;
    public UIPanel uiPanel;
    public float itemHeight;  // 列表项的高度

    private List<ScoreData> data; //缓存本地分数的地址
    private List<LoopListItem> visibleItems = new List<LoopListItem>();  // 当前可见的列表项
    private Queue<LoopListItem> itemPool = new Queue<LoopListItem>();  // 列表项对象池

    private int visibleItemCount;  // 可见区域内可容纳的最大列表项数量
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
        // 监听滚动事件
        scrollView.onDragStarted = onDragStart;
        scrollView.onDragFinished = onDragEnd;
        startIdx = 0;
    }

    public void UpdatePanel()
    {
        data = LocalData.Instance.ScoreDatas;
        // 计算可见区域内可容纳的最大列表项数量
        visibleItemCount = Mathf.CeilToInt(scrollView.panel.height / itemHeight);
        maxNum = visibleItemCount + 1;
        // 初始化显示的列表项
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
            // 处理向上滚动
            if (currentPosition > lastPosition)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y > topY.y && startIdx < data.Count - visibleItemCount)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y - visibleItemCount * itemHeight, 0);
                        startIdx++;
                        //得到最后一项的索引，取数据，然后更新Item显示
                        int endIdx = startIdx + visibleItemCount - 1;
                        if(endIdx < data.Count)
                        {
                            this.UpdateItem(visibleItems[i],endIdx);
                        }
                    }
                }
            }
            // 处理向下滚动
            else if (currentPosition < lastPosition)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y < bottomY.y && startIdx > 0)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y + visibleItemCount * itemHeight, 0);
                        startIdx--;
                        //得到第一项的索引，取数据，然后更新Item显示
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
