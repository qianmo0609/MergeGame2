using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoopList2:MonoBehaviour
{
    public UIScrollView scrollView;
    public UIGrid grid;
    public GameObject itemPrefab;
    public int itemCount;
    public float itemHeight;

    private List<GameObject> visibleItems = new List<GameObject>();
    private List<int> dataList = new List<int>();
    private int visibleItemCount;
    private float lastScrollPosition;

    void Start()
    {
        // 初始化数据列表
        for (int i = 0; i < itemCount; i++)
        {
            dataList.Add(i);
        }

        // 计算可见区域内可容纳的最大列表项数量
        visibleItemCount = Mathf.FloorToInt(scrollView.panel.height / itemHeight);

        // 初始化显示的列表项
        InitializeVisibleItems();

        // 监听滚动事件
        scrollView.onDragFinished = OnScrollFinished;
    }

    void InitializeVisibleItems()
    {
        for (int i = 0; i < visibleItemCount; i++)
        {
            GameObject item = NGUITools.AddChild(grid.gameObject, itemPrefab);
            UpdateItem(item, i);
            visibleItems.Add(item);
        }
        grid.Reposition();
    }

    void UpdateItem(GameObject item, int index)
    {
        if (index < dataList.Count)
        {
            UILabel label = item.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                label.text = "Item " + dataList[index].ToString();
            }
        }
    }

    void OnScrollFinished()
    {
        float currentScrollPosition = grid.transform.localPosition.y;
        if (currentScrollPosition > lastScrollPosition)
        {
            // 向上滚动
            while (currentScrollPosition > itemHeight)
            {
                MoveLastItemToTop();
                currentScrollPosition -= itemHeight;
                grid.transform.localPosition = new Vector3(grid.transform.localPosition.x, currentScrollPosition, grid.transform.localPosition.z);
            }
        }
        else if (currentScrollPosition < lastScrollPosition)
        {
            // 向下滚动
            while (currentScrollPosition < -itemHeight)
            {
                MoveFirstItemToBottom();
                currentScrollPosition += itemHeight;
                grid.transform.localPosition = new Vector3(grid.transform.localPosition.x, currentScrollPosition, grid.transform.localPosition.z);
            }
        }
        lastScrollPosition = currentScrollPosition;
    }

    void MoveLastItemToTop()
    {
        GameObject lastItem = visibleItems[visibleItems.Count - 1];
        visibleItems.RemoveAt(visibleItems.Count - 1);
        visibleItems.Insert(0, lastItem);
        int newIndex = GetFirstVisibleIndex() - 1;
        if (newIndex < 0)
        {
            newIndex = dataList.Count - 1;
        }
        lastItem.transform.SetAsFirstSibling();
        UpdateItem(lastItem, newIndex);
        grid.Reposition();
    }

    void MoveFirstItemToBottom()
    {
        GameObject firstItem = visibleItems[0];
        visibleItems.RemoveAt(0);
        visibleItems.Add(firstItem);
        int newIndex = GetLastVisibleIndex() + 1;
        if (newIndex >= dataList.Count)
        {
            newIndex = 0;
        }
        firstItem.transform.SetAsLastSibling();
        UpdateItem(firstItem, newIndex);
        grid.Reposition();
    }

    int GetFirstVisibleIndex()
    {
        float offset = scrollView.transform.localPosition.y / itemHeight;
        int index = Mathf.FloorToInt(offset);
        return index;
    }

    int GetLastVisibleIndex()
    {
        return GetFirstVisibleIndex() + visibleItemCount - 1;
    }
}
