using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("该物体上的Image组件")]
    public Image targetImage;
    [Header("Image的最小宽度")]
    public float minWidth = 100f;
    [Header("Image的最大宽度")]
    public float maxWidth = 300f;
    [Header("宽度增加的速度")]
    public float growSpeed = 5f;
    [Header("宽度减少的速度")]
    public float shrinkSpeed = 5f;

    private static MouseHover currentClickedObject = null;  // 当前点击的物体
    private bool isHovered = false;     // 标记物体是否处于悬浮状态
    private bool isBeingClicked = false; // 是否已被点击

    private void Start()
    {
        if (targetImage != null)
        {
            // 初始化时，物体的宽度设为最小值
            if (this.GetComponent<TextMeshProUGUI>().text == "Main")
            {
                currentClickedObject = this;
                this.isBeingClicked = true;
                return;
            }
            targetImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
        }


    }

    void Update()
    {
        // 如果该物体已被点击，则不再缩小，保持最大宽度
        if (isBeingClicked && targetImage != null)
        {
            return; // 已点击物体的宽度应该一直保持最大，不受影响
        }

        // 如果有悬停，且没有被点击，宽度增加
        if (isHovered && targetImage != null)
        {
            float newWidth = Mathf.Lerp(targetImage.rectTransform.rect.width, maxWidth, growSpeed * Time.deltaTime);
            targetImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
        else if (!isHovered && targetImage != null && !isBeingClicked)
        {
            // 如果没有悬停，且没有被点击，宽度逐渐减少
            float newWidth = Mathf.Lerp(targetImage.rectTransform.rect.width, minWidth, shrinkSpeed * Time.deltaTime);
            targetImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    // 鼠标进入物体时，开始增加宽度
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null && currentClickedObject != this)
        {
            isHovered = true;  // 标记为悬停状态
        }
    }

    // 鼠标离开物体时，开始减少宽度
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null && currentClickedObject != this)
        {
            isHovered = false;  // 标记为非悬停状态
        }
    }

    // 鼠标点击时，设置当前物体的宽度为最大值，并锁定该状态
    public void OnPointerClick(PointerEventData eventData)
    {
        ShowThing.Instance.Show(this.gameObject.transform.parent.GetSiblingIndex());
        if (this.GetComponent<TextMeshProUGUI>().text == "Github")
        {
            Application.OpenURL("https://github.com/sorawithcat/SoraTodo");
        }
        if (this.GetComponent<TextMeshProUGUI>().text == "Donation")
        {
            Application.OpenURL("https://afdian.com/a/sorawithcat");
        }
        if (targetImage != null)
        {
            // 如果点击了当前物体，则直接返回
            if (currentClickedObject == this)
            {
                return;
            }

            // 先将上一个已点击的物体宽度恢复为最小值
            if (currentClickedObject != null)
            {
                currentClickedObject.targetImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
                currentClickedObject.isBeingClicked = false;  // 标记上一个物体为未被点击
                currentClickedObject.isHovered = false;
            }

            // 将当前点击的物体宽度设为最大
            targetImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

            // 锁定当前点击的物体
            currentClickedObject = this;
            isBeingClicked = true;  // 标记为已被点击
        }
    }
}
