using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ClassifyButtonManager : MonoBehaviour, IPointerClickHandler
{
    [Header("按钮的父节点")]
    public GameObject buttonsList;
    [Header("Todo的父节点")]
    public GameObject todoList;

    public float thisSpacing;
    [HideInInspector] public int isOpen = -1;
    [HideInInspector] public int thisID;
    [Header("TodoList收缩速度")]
    [SerializeField] private float changeSpeed = 0.5f;

    private CustomVerticalLayoutGroup customVerticalLayoutGroup;
    private RectTransform todoListRectTransform;

    private bool isLerpChange = false;

    /// <summary>
    /// 自己所处第几个元素
    /// </summary>
    public int siblingIndex;


    private void Start()
    {
        Invoke(nameof(Init), 0.1f);
    }

    private void Init()
    {
        thisSpacing = todoList.GetComponent<RectTransform>().rect.height;
        customVerticalLayoutGroup = buttonsList.GetComponent<CustomVerticalLayoutGroup>();
        todoListRectTransform = todoList.GetComponent<RectTransform>();
        ClickToHandoff();
    }

    /// <summary>
    /// 点击后切换展开和收缩
    /// </summary>
    public void ClickToHandoff()
    {
        if (!isLerpChange)
        {
            isLerpChange = true;
            if (isOpen == -1)
            {
                todoList.SetActive(true);
                while (customVerticalLayoutGroup.childSpacings.Count <= thisID)
                {
                    customVerticalLayoutGroup.childSpacings.Add(0);
                }
                StartCoroutine(LerpButtonListSpacing(0, thisSpacing));
                StartCoroutine(LerpTodoListPosition(todoListRectTransform.anchoredPosition.y, 0));
            }
            else
            {
                while (customVerticalLayoutGroup.childSpacings.Count <= thisID)
                {
                    customVerticalLayoutGroup.childSpacings.Add(0);
                }
                StartCoroutine(LerpButtonListSpacing(thisSpacing, 0));
                StartCoroutine(LerpTodoListPosition(0, todoListRectTransform.anchoredPosition.y));
                Invoke(nameof(SetFalseTodoList), changeSpeed);
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
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("classifyButtonText"))
        {
            //分类标签左键
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ClickToHandoff();
                RightMenuManager.Instance.HideRightMenu();

            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RightMenuManager.Instance.GetMenuInfo(MenuTags.classifyButtonText, transform);
            }
        }

    }
}
