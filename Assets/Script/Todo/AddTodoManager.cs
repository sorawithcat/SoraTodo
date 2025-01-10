using System;
using TMPro;
using UnityEngine;

public class AddTodoManager : MonoBehaviour
{
    public static AddTodoManager Instance;

    private Animator animator;

    [HideInInspector] public Transform setTransform;

    [Header("分类按钮预制体")]
    [SerializeField] private GameObject classifyButtonPrefab;
    [Header("Todo预制体")]
    [SerializeField] private GameObject todoPrefab;

    [Header("定时类型的下拉框")]
    [SerializeField] private TMP_Dropdown timeTypeDropdown;
    [Header("闹钟类型的下拉框")]
    [SerializeField] private TMP_Dropdown alarmTypeDropdown;
    [Header("倒计时输入框")]
    [SerializeField] private TMP_InputField timeInputField;
    [Header("年份")]
    [SerializeField] private TextMeshProUGUI yearText;
    [Header("月份")]
    [SerializeField] private TextMeshProUGUI monthText;
    [Header("日期")]
    [SerializeField] private TextMeshProUGUI dayText;
    [Header("小时")]
    [SerializeField] private TextMeshProUGUI hourText;
    [Header("分钟")]
    [SerializeField] private TextMeshProUGUI minuteText;
    [Header("秒")]
    [SerializeField] private TextMeshProUGUI secondText;

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
    }

    void Update()
    {
    }

    public void AddTodo(Transform _setTransform)
    {
        GameObject newTodo = Instantiate(todoPrefab, _setTransform);
        TodoManager todoManager = newTodo.GetComponent<TodoManager>();

        int selectedTimeType = timeTypeDropdown.value;
        int selectedAlarmType = alarmTypeDropdown.value;
        int timeValue = int.Parse(timeInputField.text);

        int year = int.Parse(yearText.text);
        int month = int.Parse(monthText.text);
        int day = int.Parse(dayText.text);
        int hour = int.Parse(hourText.text);
        int minute = int.Parse(minuteText.text);
        int second = int.Parse(secondText.text);

        DateTime _dateTime = new DateTime(year, month, day, hour, minute, second);
        TimerManager.Instance.UpdateTodoTimerSetting(todoManager, (TimingType)selectedTimeType, (AlarmType)selectedAlarmType, timeValue, _dateTime, customizePath);
    }

    public void AddClassify(Transform _setTransform)
    {

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
