using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomVerticalLayoutGroup : LayoutGroup
{
    [Header("子物体间距")]
    [Tooltip("为每个子物体设置不同的间距。")]
    public List<float> childSpacings;

    // 可选：布局的内边距（边缘）
    public float topPadding = 0;    // 上边距
    public float bottomPadding = 0; // 下边距

    // 自定义的间距（替代默认的 spacing 字段）
    public float defaultSpacing = 0f;  // 默认间距
    private float thisHeight;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical()
    {
        // 计算总高度
        float totalHeight = 0;
        float spacingSum = 0;

        // 加上内边距
        totalHeight += topPadding;
        totalHeight += bottomPadding;

        // 遍历每个子物体，计算总高度
        for (int i = 0; i < rectChildren.Count; i++)
        {
            totalHeight += rectChildren[i].rect.height;

            // 如果有为这个子物体指定的间距，则加上该间距
            if (i < childSpacings.Count)
            {
                spacingSum += childSpacings[i];
            }
            else
            {
                // 如果没有为该子物体指定间距，则使用默认的间距
                spacingSum += defaultSpacing;
            }
        }

        // 添加子物体之间的额外间距
        totalHeight += spacingSum;
        // 返回计算出来的总高度，供布局系统使用
        SetLayoutInputForAxis(totalHeight, totalHeight, totalHeight, 1);  // 仅设置垂直方向（轴 1）
    }

    public override void SetLayoutHorizontal()
    {
        // 水平布局不会影响我们的垂直定位
    }

    public override void SetLayoutVertical()
    {
        // 按照每个子物体的相应间距垂直排列它们
        float yOffset = topPadding; // 从上边距开始

        for (int i = 0; i < rectChildren.Count; i++)
        {
            // 设置子物体的位置
            rectChildren[i].anchoredPosition = new Vector2(0, -yOffset);

            // 更新下一个子物体的偏移量
            yOffset += rectChildren[i].rect.height;

            // 在子物体之后加上间距
            if (i < childSpacings.Count)
            {
                yOffset += childSpacings[i];
            }
            else
            {
                yOffset += defaultSpacing; // 如果没有为当前子物体指定间距，则使用默认间距
            }
        }

        // 考虑下边距
        yOffset += bottomPadding;
        thisHeight = yOffset;

        // 更新布局的高度
        UpdateThisHeight(thisHeight);
    }

    // 可选：通过检视器直接设置内边距（topPadding 和 bottomPadding）
    protected override void OnEnable()
    {
        base.OnEnable();
        // 将内边距应用到 LayoutGroup 的内边距（padding）
        padding = new RectOffset(0, 0, Mathf.FloorToInt(topPadding), Mathf.FloorToInt(bottomPadding));
    }

    // 外部脚本调用此方法更新间距
    public void UpdateChildSpacings()
    {
        // 只更新 childSpacings，如果需要额外的验证可以添加
        UpdateThisHeight(thisHeight);

        // 强制重新计算布局
        SetLayoutVertical();
    }

    public void UpdateThisHeight(float _height)
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width, _height);
    }

    /// <summary>
    /// 动态添加子物体
    /// </summary>
    /// <param name="newChild"></param>
    public void AddChild(RectTransform newChild)
    {
        //newChild.SetParent(transform);
        rectChildren.Add(newChild); //更新rectChildren列表

        // 重新计算布局
        SetLayoutVertical();
    }

    // 动态删除子物体
    public void RemoveChild(RectTransform childToRemove)
    {
        rectChildren.Remove(childToRemove); // 从 rectChildren 中移除

        // 销毁子物体
        Destroy(childToRemove.gameObject);

        // 重新计算布局
        SetLayoutVertical();
    }

    /// <summary>
    /// 重新计算总高度并更新布局
    /// </summary>
    public void RecalculateTotalHeight()
    {
        // 计算并设置总高度
        float totalHeight = 0;
        float spacingSum = 0;

        totalHeight += topPadding;
        totalHeight += bottomPadding;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            totalHeight += rectChildren[i].rect.height;

            if (i < childSpacings.Count)
            {
                spacingSum += childSpacings[i];
            }
            else
            {
                spacingSum += defaultSpacing;
            }
        }

        totalHeight += spacingSum;

        // 更新布局的高度
        UpdateThisHeight(totalHeight);
    }

    /// <summary>
    /// 更新指定索引的子物体间距，并更新布局
    /// </summary>
    /// <param name="index">要更新的子物体索引</param>
    /// <param name="newSpacing">新的间距值</param>
    public void UpdateChildSpacingAtIndex(int index, float newSpacing)
    {
        if (index >= 0 && index < childSpacings.Count)
        {
            // 更新指定索引位置的间距
            childSpacings[index] = newSpacing;

            // 重新计算并更新布局
            SetLayoutVertical();
        }
        else
        {
            TipWindowManager.Instance.ShowTip("索引超出范围，无法更新间距！数据已保存，重启以恢复正常");
        }
    }
}
