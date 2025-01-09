using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    private Vector3 storedPosition;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private RectTransform rectTransform;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        storedPosition = rectTransform.position;
    }

    public Vector3 GetStoredPosition()
    {
        return storedPosition;
    }

    public void SetStoredPosition(Vector3 newPosition)
    {
        storedPosition = newPosition;
        rectTransform.position = newPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 记录鼠标按下时的初始位置
        isDragging = true;
        lastMousePosition = Input.mousePosition;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isDragging)
        {
            // 获取当前鼠标位置
            Vector3 currentMousePosition = Input.mousePosition;

            // 计算鼠标的移动差值
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            // 更新物体的位置（基于鼠标移动的差值）
            rectTransform.position += new Vector3(mouseDelta.x, mouseDelta.y, 0);
            storedPosition = rectTransform.position;

            // 更新最后一个鼠标位置
            lastMousePosition = currentMousePosition;
        }
    }
}
