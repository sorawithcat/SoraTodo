using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [Header("字体组件，为null时会获取为此脚本物体上的TextMeshProUGUI")]
    public TextMeshProUGUI scrollingText;
    [Header("字体组件的父物体，为null时会获取为此脚本的父物体")]
    [SerializeField] private GameObject textMeshProFather;
    [Header("滚动速度")]
    public float scrollSpeed;
    [Header("不超过则暂停？默认为true")]
    [SerializeField] private bool noOverToStop = true;
    private float containerWidth;
    private Vector2 textPosition;
    private bool isScrollingLeft = true;
    public bool whenLeftDel = true;

    void Start()
    {
        if (scrollingText == null)
        {
            scrollingText = GetComponent<TextMeshProUGUI>();
        }
        if (textMeshProFather == null)
        {
            textMeshProFather = transform.parent.gameObject;
        }

        RectTransform containerRect = textMeshProFather.GetComponent<RectTransform>();
        containerWidth = containerRect.rect.width;
        textPosition = new Vector2(containerWidth, scrollingText.rectTransform.anchoredPosition.y);
        scrollingText.rectTransform.anchoredPosition = textPosition;
        CompareContainerAndTextWidth();
    }


    /// <summary>
    /// 比较容器和文本的宽度
    /// </summary>
    private void CompareContainerAndTextWidth()
    {
        if (noOverToStop)
        {
            if (containerWidth < textPosition.x)
            {
                isScrollingLeft = false;
            }
            else
            {
                isScrollingLeft = true;

            }
        }
    }

    void Update()
    {
        CompareContainerAndTextWidth();
        if (isScrollingLeft)
        {
            textPosition.x -= scrollSpeed * Time.deltaTime;

            if (textPosition.x < -scrollingText.preferredWidth)
            {
                if (whenLeftDel)
                {
                    DestroyThis();
                }
                textPosition.x = containerWidth;
            }

            scrollingText.rectTransform.anchoredPosition = textPosition;
        }
        else
        {
            textPosition.x = 0;
            scrollingText.rectTransform.anchoredPosition = textPosition;
        }
    }
    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}