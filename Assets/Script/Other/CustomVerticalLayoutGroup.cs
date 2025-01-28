using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomVerticalLayoutGroup : LayoutGroup
{
    [Header("子物体间距")]
    [Tooltip("为每个子物体设置不同的间距。")]
    public List<float> childSpacings;

    // 布局的内边距（边缘）
    public float topPadding = 0;    // 上边距
    public float bottomPadding = 0; // 下边距

    // 自定义的间距（替代默认的 spacing 字段）
    public float defaultSpacing = 0f;
    private float thisHeight;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical()
    {
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
        SetLayoutInputForAxis(totalHeight, totalHeight, totalHeight, 1);
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
        float yOffset = topPadding;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rectChildren[i].anchoredPosition = new Vector2(0, -yOffset);
            yOffset += rectChildren[i].rect.height;
            if (i < childSpacings.Count)
            {
                yOffset += childSpacings[i];
            }
            else
            {
                yOffset += defaultSpacing;
            }
        }
        yOffset += bottomPadding;
        thisHeight = yOffset;
        UpdateThisHeight(thisHeight);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        padding = new RectOffset(0, 0, Mathf.FloorToInt(topPadding), Mathf.FloorToInt(bottomPadding));
    }

    /// <summary>
    /// 更新间距
    /// </summary>
    public void UpdateChildSpacings()
    {
        UpdateThisHeight(thisHeight);
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
        rectChildren.Insert(newChild.gameObject.GetComponent<ClassifyButtonManager>().siblingIndex, newChild);
        childSpacings.Insert(newChild.gameObject.GetComponent<ClassifyButtonManager>().siblingIndex, 0);
        SetLayoutVertical();
    }

    /// <summary>
    /// 动态删除子物体
    /// </summary>
    /// <param name="childToRemove"></param>
    public void RemoveChild(RectTransform childToRemove)
    {
        rectChildren.Remove(childToRemove);
        Destroy(childToRemove.gameObject);
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
            childSpacings[index] = newSpacing;
            SetLayoutVertical();
        }
        else
        {
            TipWindowManager.Instance.ShowTip("索引超出范围，无法更新间距！数据已保存，重启以恢复正常");
        }
    }
}
