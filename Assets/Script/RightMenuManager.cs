using System;
using System.Collections;
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
    public void GetMenuInfo(MenuTags _menuTag)
    {
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
                "其实我很好奇你是怎么打开这个菜单的",
            }
        },
        {
            MenuTags.classifyButtonText,new List<string>()
            {
                "修改标题",
                "修改标题基础颜色",
                "统一修改所有子待办的颜色",
                "统一设置所有子待办的提醒时间",
                "统一设置所有子待办的提醒闹钟",
                "统一设置所有子待办的完成效果"


            }
        },
        {
            MenuTags.todoThing,new List<string>()
            {
                "修改待办事项",
                "修改颜色",
                "设置提醒时间",
                "设置提醒闹钟",
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
                DefaultFun,
                DefaultFun,
                DefaultFun,
                DefaultFun,
                DefaultFun,
                DefaultFun,

            }
        },
        {
            MenuTags.todoThing,new List<Action>()
            {
                DefaultFun,
                DefaultFun,
                DefaultFun,
                DefaultFun,
                DefaultFun,

            }
        },
        {
            MenuTags.menuThing,new List<Action>()
            {
                DefaultFun,
                DefaultFun,
                DefaultFun,
            }
        },
    };

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
        mousePosition.y = Mathf.Clamp(mousePosition.y, -702 + transform.GetComponent<RectTransform>().rect.height, -167 - transform.GetComponent<RectTransform>().rect.height);

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
        Debug.Log("success");
    }

    #endregion


    #region todoThing




    #endregion

    #region classifyButtonText


    #endregion

    #region menuThing


    #endregion
}
