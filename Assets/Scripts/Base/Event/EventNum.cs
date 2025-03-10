using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNum
{
    public static int StartEvent = 0x0;  //开始掉落
    public static int UpdateTotalScoreEvent = 0x1; //更新总分数
    public static int ComboDisplayNumEvent = 0x2;  //显示暴击数量
    public static int EnableOrDisableBtnStartEvent = 0x3; //开始按钮的启用与禁用事件
    public static int ClearScoreListEvent = 0x4; //清空分数列表事件

    public static int HideComboLabel = 0x5; //隐藏暴击文字
    public static int ShowComboLabel = 0x6; //显示暴击文字
}
