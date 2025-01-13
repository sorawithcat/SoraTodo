using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuTags
{
    [EnumMember(Value = "None")]
    None,
    [EnumMember(Value = "todoThing")]
    todoThing,
    [EnumMember(Value = "classifyButtonText")]
    classifyButtonText,
    [EnumMember(Value = "menuThing")]
    menuThing
}

public class RightMenuManager : MonoBehaviour, IPointerClickHandler
{
    public static RightMenuManager Instance;

    /// <summary>
    /// 目前所选内容（召唤出菜单的物体）
    /// </summary>
    private static Transform currentClickThing;
    private static List<Transform> currentClickChildsThing;
    [Header("按钮预制件")]
    [SerializeField] private GameObject rightMenuButtonPrefab;
    [Header("画布")]
    [SerializeField] private GameObject toolTipCanvas;
    [Header("base")]
    [SerializeField] private GameObject menuBase;
    [Header("按钮列表")]
    [SerializeField] private GameObject menuButtonList;
    private float offsetX = 10;

    private List<Action> currentActions = new List<Action>();
    private List<string> currentNames = new List<string>();


    private void Start()
    {
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        HideRightMenu();
    }
    /// <summary>
    /// 获取信息并设置按钮
    /// </summary>
    /// <param name="_menuTag"></param>
    public void GetMenuInfo(MenuTags _menuTag, Transform _currentClickThing)
    {
        currentClickThing = _currentClickThing;
        currentClickChildsThing = new List<Transform>();
        foreach (Transform child in currentClickThing)
        {
            foreach (Transform grandChild in child)
            {
                TodoManager todoManager = grandChild.GetComponent<TodoManager>();
                if (todoManager != null)
                {
                    currentClickChildsThing.Add(todoManager.gameObject.transform);
                }
            }
        }
        currentActions = new List<Action>();
        currentNames = new List<string>();
        currentActions = menuFuns[_menuTag];
        currentNames = menuButtonNames[_menuTag];
        ShowToolTip(currentNames, currentActions);
    }

    private SerializableDictionary<MenuTags, List<string>> menuButtonNames = new SerializableDictionary<MenuTags, List<string>>()
    {
        {
            MenuTags.None,new List<string>()
            {
                "！！！",
                "其实我很好奇你是怎么打开这个菜单的"
            }
        },
        {
            MenuTags.classifyButtonText,new List<string>()
            {
                "修改标题",
                "修改标题基础颜色",
                "修改标题字体颜色",
                "统一修改所有子待办背景初始的颜色",
                "统一修改所有子待办背景末尾的颜色",
                "统一修改所有子待办字体的颜色",
                "统一设置所有子待办的提醒",
                "统一设置所有子待办的完成效果",
                "添加新待办",
                "添加新分类"

            }
        },
        {
            MenuTags.todoThing,new List<string>()
            {
                "修改待办事项",
                "修改背景初始颜色",
                "修改背景末尾颜色",
                "修改字体颜色",
                "设置提醒",
                "设置完成效果"
            }
        },
        {
            MenuTags.menuThing,new List<string>()
            {
                "为什么你会想着右键右键菜单",
                "这是没有任何意义的行为",
                "我希望你明白这一点"
            }
        },
    };

    private SerializableDictionary<MenuTags, List<Action>> menuFuns = new SerializableDictionary<MenuTags, List<Action>>()
    {
        {
            MenuTags.None,new List<Action>()
            {
                DefaultFun,
                DefaultFun

            }
        },
        {
            MenuTags.classifyButtonText,new List<Action>()
            {
                SetTitle,
                SetBGColor,
                SetTitleColor,
                SetChildsGradientStartColor,
                SetChildsGradientEndColor,
                SetChildsTitleColor,
                SetChildsAlarm,
                SetChildsClearFX,
                DefaultFun,
                DefaultFun

            }
        },
        {
            MenuTags.todoThing,new List<Action>()
            {
                SetTitle,
                SetGradientStartColor,
                SetGradientEndColor,
                SetTitleColor,
                SetTodoAlarm,
                SetClearFx,

            }
        },
        {
            MenuTags.menuThing,new List<Action>()
            {
                DefaultFun,
                DefaultFun,
                DefaultFun
            }
        },
    };
    //无效
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag == "menuThing")
        {
            //左键-隐藏右键菜单
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                HideRightMenu();
            }
        }
    }

    /// <summary>
    /// 弹出菜单并设置
    /// </summary>
    /// <param name="_names"></param>
    /// <param name="_funs"></param>
    public void ShowToolTip(List<string> _names, List<Action> _funs)
    {
        HideRightMenu();
        gameObject.SetActive(true);
        //获取最大数量以生成按钮
        int numbs = _names.Count >= _funs.Count ? _names.Count : _funs.Count;
        float maxWidth = 100f;//最大宽度，最低为100f
        for (int i = 0; i < numbs; i++)
        {
            string name = _names[i] != null ? _names[i] : "快去添加菜单名";
            GameObject currentPrefab = Instantiate(rightMenuButtonPrefab, menuButtonList.transform);
            Button newButton = currentPrefab.GetComponentInChildren<Button>();
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = name;
            newButton.onClick.RemoveAllListeners();
            Canvas.ForceUpdateCanvases();
            maxWidth = Mathf.Max(maxWidth, newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().rect.width);
            newButton.GetComponentInParent<RightMenuButtonManager>().buttonID = i;
            newButton.GetComponentInParent<RightMenuButtonManager>().actions = _funs;

        }
        //设置base的宽度和高度
        menuBase.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + offsetX, menuButtonList.GetComponent<GridLayoutGroup>().cellSize.y * numbs);
        //设置按钮宽度
        menuButtonList.GetComponent<GridLayoutGroup>().cellSize = new Vector2(maxWidth + offsetX, menuButtonList.GetComponent<GridLayoutGroup>().cellSize.y);

        ChangeRightMenuPosition();


    }

    /// <summary>
    /// 修正菜单位置
    /// </summary>
    private void ChangeRightMenuPosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(toolTipCanvas.transform as RectTransform, Input.mousePosition, null, out mousePosition);
        mousePosition = new Vector2(mousePosition.x - 200, mousePosition.y - 400);
        mousePosition.x = Mathf.Clamp(mousePosition.x, -732 + transform.GetComponent<RectTransform>().rect.width, 739 - transform.GetComponent<RectTransform>().rect.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, -902 + transform.GetComponent<RectTransform>().rect.height, -367 - transform.GetComponent<RectTransform>().rect.height);

        this.GetComponent<RectTransform>().anchoredPosition = mousePosition;
    }
    /// <summary>
    /// 隐藏右键菜单
    /// </summary>
    public void HideRightMenu()
    {
        foreach (Transform trans in menuButtonList.transform)
        {
            Destroy(trans.gameObject);
        }
        gameObject.SetActive(false);
    }

    #region None
    /// <summary>
    /// 空函数
    /// </summary>
    public static void DefaultFun()
    {
        TipWindowManager.Instance.ShowTip("你点击了一个空的按钮，如果此按钮是功能性按钮，请联系作者修改。", Color.red);
    }

    #endregion


    #region todoThing

    public static void SetTitle()
    {
        SetText.Instance.setTransform = currentClickThing;
        SetText.Instance.SetOldText(currentClickThing);
        SetText.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetTitleColor()
    {
        SetColor.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetColor.Instance.setType = SetThingType.Text;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();

    }
    public static void SetTodoAlarm()
    {
        SetAlarm.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetAlarm.Instance.SetCurrentAlarmThing();
        TipWindowManager.Instance.ShowTip("已自动填充该待办的属性.");
        SetAlarm.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetChildsAlarm()
    {
        SetAlarm.Instance.setTransforms = currentClickChildsThing;
        SetAlarm.Instance.SetCurrentAlarmThing();
        TipWindowManager.Instance.ShowTip("已自动填充第一个待办的属性.");
        SetAlarm.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetChildsTitleColor()
    {
        SetColor.Instance.setTransforms = currentClickChildsThing;
        SetColor.Instance.setType = SetThingType.Text;
        SetColor.Instance.OpenWindow();

        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetBGColor()
    {
        SetColor.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetColor.Instance.setType = SetThingType.Image;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetChildsClearFX()
    {
        SetClearFX.Instance.setTransforms = currentClickChildsThing;
        SetClearFX.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetClearFx()
    {
        SetClearFX.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetClearFX.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetGradientStartColor()
    {
        SetColor.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetColor.Instance.setType = SetThingType.GradientStart;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetChildsGradientStartColor()
    {
        SetColor.Instance.setTransforms = currentClickChildsThing;
        SetColor.Instance.setType = SetThingType.GradientStart;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetGradientEndColor()
    {
        SetColor.Instance.setTransforms = new List<Transform>() { currentClickThing };
        SetColor.Instance.setType = SetThingType.GradientEnd;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    public static void SetChildsGradientEndColor()
    {
        SetColor.Instance.setTransforms = currentClickChildsThing;
        SetColor.Instance.setType = SetThingType.GradientEnd;
        SetColor.Instance.OpenWindow();
        TodoWindowManager.Instance.CloseWindow();
    }
    #endregion

    #region classifyButtonText


    #endregion

    #region menuThing


    #endregion
}
