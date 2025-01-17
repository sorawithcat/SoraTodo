using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AddTodoManager : MonoBehaviour
{
    public static AddTodoManager Instance;

    private Animator animator;

    [HideInInspector] public Transform setTransform;

    [Header("分类按钮预制体")]
    [SerializeField] private GameObject classifyButtonPrefab;
    [Header("分类按钮父节点")]
    [SerializeField] private GameObject classifyButtonFather;
    [Header("Todo预制体")]
    [SerializeField] private GameObject todoPrefab;

    [Header("定时类型的下拉框")]
    [SerializeField] private TMP_Dropdown timeTypeDropdown;
    [Header("闹钟类型的下拉框")]
    [SerializeField] private TMP_Dropdown alarmTypeDropdown;
    [Header("倒计时输入框")]
    [SerializeField] private TMP_InputField timeInputField;


    [Header("日期输入框")]
    [SerializeField] private TMP_InputField dateInputField;

    [Header("自定义框")]
    [SerializeField] private GameObject customizeGO;
    [Header("音效试听")]
    [SerializeField] private GameObject audioSourceButton;
    [Header("音效设置")]
    [SerializeField] private AudioSource audioSource;
    private AudioClip alarmClip;
    private string customizePath;
    /// <summary>
    /// 分类数据
    /// </summary>
    public List<ClassifyButtonManagerData> classifyButtonManagerDataList = new List<ClassifyButtonManagerData>();
    /// <summary>
    /// 待办数据
    /// </summary>
    public List<TodoManagerData> todoManagerDataList = new List<TodoManagerData>();
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


    }
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    void Update()
    {
    }

    ///// <summary>
    ///// 更新待办索引
    ///// </summary>
    //public void UpdateTodoSiblingIndex(TodoManager _todo)
    //{
    //    _todo.siblingIndex = _todo.transform.GetSiblingIndex();
    //}
    /// <summary>
    /// 更新分类索引
    /// </summary>
    public void UpdateClassifyButtonSiblingIndex(ClassifyButtonManager _classifyButton)
    {
        _classifyButton.siblingIndex = _classifyButton.transform.GetSiblingIndex();
    }
    /// <summary>
    /// 添加待办
    /// </summary>
    /// <param name="_setTransform"></param>
    public void AddTodo(Transform _setTransform)
    {
        GameObject newTodo = Instantiate(todoPrefab, _setTransform);
        TodoManager todoManager = newTodo.GetComponent<TodoManager>();


        int selectedTimeType = timeTypeDropdown.value;
        int selectedAlarmType = alarmTypeDropdown.value;

        long timeValue = SetAlarm.Instance.GetTimeAndConvertToSeconds(timeInputField);
        List<int> ints = SetAlarm.Instance.GetTimeAndConvertToArrary(dateInputField);
        DateTime _dateTime = new DateTime(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], DateTimeKind.Utc);
        TimerManager.Instance.UpdateTodoTimerSetting(todoManager, (TimingType)selectedTimeType, (AlarmType)selectedAlarmType, timeValue, _dateTime, customizePath);
        //Todo:刷新索引，添加json

    }

    public void AddClassify(Transform _setTransform)
    {
        GameObject newClassify = Instantiate(classifyButtonPrefab, classifyButtonFather.transform);
    }

    // 关闭窗口的动画
    public void CloseWindow()
    {
        animator.SetBool("IsClose", true);
        TodoWindowManager.Instance.OpenWindow();
    }

    // 打开窗口的动画
    public void OpenWindow()
    {
        animator.SetBool("IsClose", false);
    }
}
