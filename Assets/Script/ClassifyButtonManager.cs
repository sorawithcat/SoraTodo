using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassifyButtonManager : MonoBehaviour, IPointerClickHandler
{
    [Header("按钮的父节点")]
    [SerializeField] private GameObject buttonsList;
    [Header("Todo的父节点")]
    [SerializeField] private GameObject todoList;

    private float thisSpacing;
    private int isOpen = -1;
    [HideInInspector] public int thisID;
    [Header("TodoList收缩速度")]
    [SerializeField] private float changeSpeed = 0.5f;



    private CustomVerticalLayoutGroup customVerticalLayoutGroup;
    private RectTransform todoListRectTransform;

    private bool isLerpChange = false;
    private void Start()
    {
        // 获取初始高度（todoList 的高度）
        thisSpacing = todoList.GetComponent<RectTransform>().rect.height;
        customVerticalLayoutGroup = buttonsList.GetComponent<CustomVerticalLayoutGroup>();
        todoListRectTransform = todoList.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // 你可以在此处平滑控制其他行为
    }

    /// <summary>
    /// 点击后切换展开和收缩
    /// </summary>
    public void clickToHandoff()
    {
        if (!isLerpChange)
        {
            isLerpChange = true;
            if (isOpen == -1)
            {
                todoList.SetActive(true);
                // 扩展子物体间距
                while (customVerticalLayoutGroup.childSpacings.Count <= thisID)
                {
                    customVerticalLayoutGroup.childSpacings.Add(0);
                }

                // 使用 Lerp 平滑地改变buttonlist的间距
                StartCoroutine(LerpButtonListSpacing(0, thisSpacing));

                // 使用 Lerp 平滑地改变 RectTransform 的位置
                StartCoroutine(LerpTodoListPosition(todoListRectTransform.anchoredPosition.y, 0));
            }
            else
            {
                // 收缩子物体间距
                while (customVerticalLayoutGroup.childSpacings.Count <= thisID)
                {
                    customVerticalLayoutGroup.childSpacings.Add(0);
                }
                //改变子物体的间距
                StartCoroutine(LerpButtonListSpacing(thisSpacing, 0));

                //改变todo位置
                StartCoroutine(LerpTodoListPosition(0, todoListRectTransform.anchoredPosition.y));

                // 延迟隐藏 todoList
                Invoke("SetFalseTodoList", changeSpeed);
            }
            isOpen *= -1;
        }
    }

    /// <summary>
    /// 控制子物体之间间距
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private IEnumerator LerpButtonListSpacing(float startValue, float endValue)
    {
        float elapsedTime = 0f;
        while (elapsedTime < changeSpeed)
        {
            float currentSpacing = Mathf.Lerp(startValue, endValue, elapsedTime / changeSpeed);
            customVerticalLayoutGroup.childSpacings[thisID] = currentSpacing;
            customVerticalLayoutGroup.UpdateChildSpacings();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        customVerticalLayoutGroup.childSpacings[thisID] = endValue;
        customVerticalLayoutGroup.UpdateChildSpacings();
        isLerpChange = false;
    }

    /// <summary>
    /// 改变todo位置
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private IEnumerator LerpTodoListPosition(float startValue, float endValue)
    {
        float elapsedTime = 0f;
        while (elapsedTime < changeSpeed)
        {
            todoList.GetComponent<VerticalLayoutGroup>().spacing = Mathf.Lerp(startValue, endValue, elapsedTime / changeSpeed);
            customVerticalLayoutGroup.UpdateChildSpacings();

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        todoList.GetComponent<VerticalLayoutGroup>().spacing = endValue;
        customVerticalLayoutGroup.UpdateChildSpacings();
        isLerpChange = false;
    }

    /// <summary>
    /// 设置todolist的激活状态为false
    /// </summary>
    private void SetFalseTodoList()
    {
        todoList.SetActive(false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag == "classifyButtonText")
        {
            //分类标签左键
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                clickToHandoff();
                RightMenuManager.Instance.HideRightMenu();

            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RightMenuManager.Instance.GetMenuInfo(MenuTags.classifyButtonText,transform);
            }
        }

    }
}
