using System;
using System.Collections.Generic;
using TMPro;
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
    [Header("音效试听")]
    [SerializeField] private GameObject audioSourceButton;
    [Header("日期输入框")]
    [SerializeField] private TMP_InputField dateInputField;
    [Header("倒计时输入框")]
    [SerializeField] private TMP_InputField timeInputField;
    [Header("自定义框")]
    [SerializeField] private GameObject customizeGO;

    [Header("标题")]
    [SerializeField] private TMP_InputField title;


    [Header("完成效果的下拉框")]
    [SerializeField] private TMP_Dropdown clearTypeDropdown;
    [Header("演示用todo")]
    [SerializeField] private TodoManager todoManager;
    [Header("演示用todo父节点")]
    [SerializeField] private GameObject todoManagerFather;

    [Header("音效设置")]
    [SerializeField] private AudioSource audioSource;
    private AudioClip alarmClip;
    private string customizePath;

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
        timeTypeDropdown.onValueChanged.AddListener(OnTimeDropdownValueChanged);
        alarmTypeDropdown.onValueChanged.AddListener(OnAlarmDropdownValueChanged);
        clearTypeDropdown.onValueChanged.AddListener(OnclearTypeDropdownValueChanged);
        todoManager.isTodo = true;
        todoManager.SetClearFX();
    }

    private void OnclearTypeDropdownValueChanged(int index)
    {
        if (todoManager != null)
        {
            todoManager.clearFX = (ClearFX)index;
            todoManager.SetClearFX();
        }
        else
        {
            GameObject newTodo = Instantiate(todoPrefab, todoManagerFather.transform);
            todoManager = newTodo.GetComponent<TodoManager>();
            todoManager.isTodo = true;
            todoManager.SetClearFX();
        }
    }
    public void PlayAlarmSound()
    {
        if (alarmClip != null && audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            TipWindowManager.Instance.ShowTip("铃声或AudioSource为空，无法播放铃声！", Color.red);
        }
    }
    void OnTimeDropdownValueChanged(int index)
    {
        switch ((TimingType)index)
        {
            case TimingType.None:
                timeInputField.gameObject.SetActive(false);
                dateInputField.gameObject.SetActive(false);
                break;
            case TimingType.Countdown:
                timeInputField.gameObject.SetActive(true);
                dateInputField.gameObject.SetActive(false); break;
            case TimingType.Date:
                timeInputField.gameObject.SetActive(false);
                dateInputField.gameObject.SetActive(true);
                break;
        }
    }

    public void OpenAlarmFile()
    {
        FolderBrowserHelper.SelectFile((filePath) =>
        {
            // if (filePath == customizePath) return;
            customizePath = filePath;
            FolderBrowserHelper.SetAudioClip(customizePath, audioSource);

            ;
        }, FolderBrowserHelper.AUDIOFILTER);
    }
    void OnAlarmDropdownValueChanged(int index)
    {
        audioSource.Stop();

        string soundPath = "AlarmClockSounds/";

        switch ((AlarmType)index)
        {
            case AlarmType.None:
                audioSourceButton.SetActive(false);
                customizeGO.SetActive(false);
                break;
            case AlarmType.Customize:
                audioSourceButton.SetActive(true);
                customizeGO.SetActive(true);
                break;
            case AlarmType.Alarm1:
                soundPath += "Alarm1";
                alarmClip = Resources.Load<AudioClip>(soundPath);
                if (alarmClip == null)
                {
                    TipWindowManager.Instance.ShowTip("未找到铃声文件: " + soundPath, Color.red);
                }
                else
                {
                    audioSource.clip = alarmClip;

                }
                audioSourceButton.SetActive(true);

                customizeGO.SetActive(false);
                break;
        }
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
    public void AddTodo()
    {
        setTransform.GetComponent<ClassifyButtonManager>().thisSpacing += todoPrefab.GetComponent<RectTransform>().rect.height;

        if (setTransform.GetComponent<ClassifyButtonManager>().isOpen == -1)
        {
            setTransform.GetComponent<ClassifyButtonManager>().clickToHandoff();
        }
        GameObject newTodo = Instantiate(todoPrefab, setTransform.GetComponentInChildren<TodoList>().transform);
        newTodo.transform.SetParent(setTransform.GetComponentInChildren<TodoList>().transform, false);
        TodoManager todoManager = newTodo.GetComponent<TodoManager>();
        todoManager.todoText.text = title.text;

        int selectedTimeType = timeTypeDropdown.value;
        int selectedAlarmType = alarmTypeDropdown.value;
        long timeValue;
        if (timeInputField.IsActive())
        {
            timeValue = SetAlarm.Instance.GetTimeAndConvertToSeconds(timeInputField);
        }
        else
        {
            timeValue = 0;
        }
        DateTime _dateTime;
        if (dateInputField.IsActive())
        {
            List<int> ints = SetAlarm.Instance.GetTimeAndConvertToArrary(dateInputField);
            _dateTime = new DateTime(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], DateTimeKind.Utc);
        }
        else
        {
            _dateTime = DateTime.Now;
        }
        TimerManager.Instance.UpdateTodoTimerSetting(todoManager, (TimingType)selectedTimeType, (AlarmType)selectedAlarmType, timeValue, _dateTime, customizePath);
        todoManager.clearFX = (ClearFX)clearTypeDropdown.value;
        if (todoManager.isTodo)
        {
            todoManager.SetClearFX();
        }
        TodoManagerData todoManagerData = new TodoManagerData()
        {
            id = LoadAllData.Instance.GenerateUniqueId(),
            title = title.text,
            titleColor = "Color.Black",
            titleBGStartColor = "#FF6F60",
            titleBGEndColor = "#8F0010",
            clearFX = clearTypeDropdown.value,
            timeType = selectedTimeType,
            alarmType = selectedAlarmType,
            countdownTime = timeValue,
            dateTime = _dateTime.Ticks,
            timer = timeValue,
            customizePath = customizePath,
            isCountingDown = false,
            isAlarmPlayed = false,
            isTodo = false,
            isChangeCustomize = true
        };
        classifyButtonFather.GetComponent<CustomVerticalLayoutGroup>().UpdateChildSpacingAtIndex(setTransform.GetComponent<ClassifyButtonManager>().thisID, setTransform.GetComponent<ClassifyButtonManager>().thisSpacing);

        LoadAllData.Instance.AddTodoManager(todoManagerData, setTransform.GetComponent<ClassifyButtonManager>().siblingIndex);
        TimerManager.Instance.RegisterTodoManager(todoManager, setTransform.GetComponent<ClassifyButtonManager>());
        CloseWindow();
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
