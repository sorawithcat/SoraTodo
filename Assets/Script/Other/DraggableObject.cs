using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    private Vector3 storedPosition;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private RectTransform rectTransform;
    private Vector3 originalPosition; // 自身初始位置

    public bool notThis;

    [ConditionalHide(nameof(notThis)), Header("被拖动的物体")]
    public GameObject draggableObject;

    [ConditionalHide(nameof(notThis)), Header("是否为摇杆")]
    public bool isRocker;

    private RectTransform targetRectTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position; // 始终记录自身初始位置
        storedPosition = originalPosition;

        // 初始化目标对象
        if (notThis && draggableObject != null)
        {
            targetRectTransform = draggableObject.GetComponent<RectTransform>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition;

        // 更新目标引用（支持运行时修改）
        if (notThis && draggableObject != null)
        {
            targetRectTransform = draggableObject.GetComponent<RectTransform>();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 mouseDelta = currentMousePosition - lastMousePosition;

        try
        {
            if (notThis)
            {
                // 同步移动自身和目标对象
                if (targetRectTransform != null)
                {
                    // 移动目标对象
                    targetRectTransform.position += mouseDelta;
                    // 同时移动自身
                    rectTransform.position += mouseDelta;
                    storedPosition = rectTransform.position;
                }
            }
            else
            {
                // 只移动自身
                rectTransform.position += mouseDelta;
                storedPosition = rectTransform.position;
            }
        }
        finally
        {
            lastMousePosition = currentMousePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        if (!isRocker || !notThis)
        {
            originalPosition = rectTransform.position;
        }
        // 摇杆模式仅复位自身位置
        if (isRocker && notThis)
        {
            rectTransform.position = originalPosition;
            storedPosition = originalPosition;
        }
    }

    public Vector3 GetStoredPosition() => storedPosition;

    public void SetStoredPosition(Vector3 newPosition)
    {
        storedPosition = newPosition;
        rectTransform.position = newPosition;
    }
}