using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;

public class SetAlarm : MonoBehaviour
{
    public static SetAlarm Instance;

    [HideInInspector] public List<Transform> setTransforms;

    private Animator animator;
    [SerializeField] private GameObject fileDropArea;
    private Animator fileAnimator;


    [Header("定时类型的下拉框")]
    [SerializeField] private TMP_Dropdown timeTypeDropdown;
    [Header("倒计时输入框")]
    [SerializeField] private TMP_InputField timeInputField;
    [Header("日期输入框")]
    [SerializeField] private TMP_InputField dateInputField;
    [Header("闹钟类型的下拉框")]
    [SerializeField] private TMP_Dropdown alarmTypeDropdown;
    [Header("自定义框")]
    [SerializeField] private GameObject customizeGO;
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
        fileAnimator = fileDropArea.GetComponent<Animator>();
        timeTypeDropdown.onValueChanged.AddListener(OnTimeDropdownValueChanged);
        alarmTypeDropdown.onValueChanged.AddListener(OnAlarmDropdownValueChanged);

    }
    void Update()
    {

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

    void OnAlarmDropdownValueChanged(int index)
    {
        switch ((AlarmType)index)
        {
            case AlarmType.Customize:
                customizeGO.SetActive(true);
                break;
            case AlarmType.None:
            case AlarmType.Alarm1:
                customizeGO.SetActive(false);
                break;
        }
    }
    public void SetAlarms()
    {
        int selectedTimeType = timeTypeDropdown.value;
        int selectedAlarmType = alarmTypeDropdown.value;

        int timeValue = GetTimeAndConvertToSeconds(timeInputField);
        DateTime _dateTime = DateTime.Now.AddSeconds(GetTimeAndConvertToSeconds(dateInputField));

        foreach (Transform transform in setTransforms)
        {
            TodoManager todoManager = transform.GetComponent<TodoManager>();
            TimerManager.Instance.UpdateTodoTimerSetting(todoManager, (TimingType)selectedTimeType, (AlarmType)selectedAlarmType, timeValue, _dateTime);
        }

    }
    public void CloseWindow()
    {
        animator.SetBool("IsClose", true);

    }
    public void OpenWindow()
    {
        animator.SetBool("IsClose", false);

    }
    public void CloseFileDropAreaWindow()
    {
        fileAnimator.SetBool("IsClose", true);

    }
    public void OpenFileDropAreaWindow()
    {
        fileAnimator.SetBool("IsClose", false);

    }

    public int ConvertTimeToSeconds(string input)
    {
        int totalSeconds = 0;

        string pattern = @"(\d+)(year|mon|day|hour|min|sec)";
        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            string numberStr = match.Groups[1].Value;
            string unit = match.Groups[2].Value.ToLower();

            int number = int.Parse(numberStr);

            // 根据单位进行秒数转换
            switch (unit)
            {
                case "year":
                    totalSeconds += number * 365 * 24 * 60 * 60;
                    break;
                case "mon":
                    totalSeconds += number * 30 * 24 * 60 * 60; 
                    break;
                case "day":
                    totalSeconds += number * 24 * 60 * 60;
                    break;
                case "hour":
                    totalSeconds += number * 60 * 60;
                    break;
                case "min":
                    totalSeconds += number * 60;
                    break;
                case "sec":
                    totalSeconds += number; 
                    break;
                default:
                    Debug.LogWarning("Unknown time unit: " + unit);
                    break;
            }
        }

        return totalSeconds;
    }

    // 获取输入框中的文字，并转换为秒
    public int GetTimeAndConvertToSeconds(TMP_InputField _InputField)
    {
        string inputText = _InputField.text;

        if (string.IsNullOrEmpty(inputText))
        {
            Debug.LogWarning("输入框为空");
            return 0;
        }

        return ConvertTimeToSeconds(inputText);


    }
}
