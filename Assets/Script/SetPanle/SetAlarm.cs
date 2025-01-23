using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SetAlarm : MonoBehaviour
{
    public static SetAlarm Instance;

    [HideInInspector] public List<Transform> setTransforms;

    private CanvasGroup canvasGroup;

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
    [Header("音效试听")]
    [SerializeField] private GameObject audioSourceButton;
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
        canvasGroup = GetComponent<CanvasGroup>();
        timeTypeDropdown.onValueChanged.AddListener(OnTimeDropdownValueChanged);
        alarmTypeDropdown.onValueChanged.AddListener(OnAlarmDropdownValueChanged);
    }


    /// <summary>
    /// 设置内容为首个todo的设置，以免重复设置一些不需要更改的项
    /// </summary>
    public void SetCurrentAlarmThing()
    {
        TodoManager todomanager = setTransforms[0].GetComponent<TodoManager>();
        alarmTypeDropdown.value = (int)todomanager.alarmType;
        timeTypeDropdown.value = (int)todomanager.timingType;
        timeInputField.text = ConvertSecondsToTimeString((long)todomanager.countdownTime);
        if (todomanager.timingType != TimingType.Date)
        {
            dateInputField.text = ConvertSecondsToTimeString(ConvertDateTimeToSeconds(DateTime.Now));

        }
        else
        {
            dateInputField.text = ConvertSecondsToTimeString(ConvertDateTimeToSeconds(todomanager.dateTime));
        }
        OnAlarmDropdownValueChanged(alarmTypeDropdown.value);
        OnTimeDropdownValueChanged(timeTypeDropdown.value);
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
    public void SetAlarms()
    {
        int selectedTimeType = timeTypeDropdown.value;
        int selectedAlarmType = alarmTypeDropdown.value;
        long timeValue;
        if (timeInputField.IsActive())
        {
            timeValue = GetTimeAndConvertToSeconds(timeInputField);
        }
        else
        {
            timeValue = 0;
        }
        DateTime _dateTime;
        if (dateInputField.IsActive())
        {
            List<int> ints = GetTimeAndConvertToArrary(dateInputField);
            _dateTime = new DateTime(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], DateTimeKind.Utc);
        }
        else
        {
            _dateTime = DateTime.Now;
        }
        foreach (Transform transform in setTransforms)
        {
            TodoManager todoManager = transform.GetComponent<TodoManager>();
            TimerManager.Instance.UpdateTodoTimerSetting(todoManager, (TimingType)selectedTimeType, (AlarmType)selectedAlarmType, timeValue, _dateTime, customizePath);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "timeType", selectedTimeType);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "alarmType", selectedAlarmType);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "timer", timeValue);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "countdownTime", timeValue);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "dateTime", _dateTime.Ticks);
            LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "customizePath", customizePath);
            if ((AlarmType)selectedAlarmType != AlarmType.Customize)
            {
                LoadAllData.Instance.UpdateTodoManager(todoManager.todoID, "customizePath", "");

            }
        }
        if ((TimingType)selectedTimeType == TimingType.None && (AlarmType)selectedAlarmType != AlarmType.None)
        {
            TipWindowManager.Instance.ShowTip("因为你选择了无定时，所以设置的闹钟类型仅设置，不会生效");
        }
        CloseWindow();

    }
    public void CloseWindow()
    {
        audioSource.Stop();
        PanleWindowManager.ClosePanle(canvasGroup);
        TodoWindowManager.Instance.OpenWindow();

    }
    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
        TipWindowManager.Instance.ShowTip("自定义铃声不建议导入过大的音频资源");

    }
    public long ConvertDateTimeToSeconds(DateTime _datetime)
    {
        long year = _datetime.Year;
        long month = _datetime.Month;
        long day = _datetime.Day;
        long hour = _datetime.Hour;
        long minute = _datetime.Minute;
        long second = _datetime.Second;

        long totalSeconds = year * 365 * 24 * 60 * 60 + month * 30 * 24 * 60 * 60 + day * 24 * 60 * 60 + hour * 3600 + minute * 60 + second;
        return totalSeconds;
    }


    public List<int> ConvertTimeToArray(string input)
    {
        int year = 0;
        int month = 0;
        int day = 0;
        int hour = 0;
        int minute = 0;
        int second = 0;

        string pattern = @"(\d+)(year|mon|day|hour|min|sec)";
        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            string numberStr = match.Groups[1].Value;
            string unit = match.Groups[2].Value.ToLower();

            int number = int.Parse(numberStr);

            switch (unit)
            {
                case "year":
                    year = number;
                    break;
                case "mon":
                    month = number;
                    break;
                case "day":
                    day = number;
                    break;
                case "hour":
                    hour = number;
                    break;
                case "min":
                    minute = number;
                    break;
                case "sec":
                    second = number;
                    break;
                default:
                    TipWindowManager.Instance.ShowTip("未知的单位 " + unit, Color.red);
                    break;
            }
        }

        List<int> longs = new() { year, month, day, hour, minute, second };
        return longs;
    }
    /// <summary>
    /// 字符串转秒
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public long ConvertTimeToSeconds(string input)
    {
        long totalSeconds = 0;

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
                    TipWindowManager.Instance.ShowTip("未知的单位 " + unit, Color.red);
                    break;
            }
        }

        return totalSeconds;
    }
    /// <summary>
    /// 秒转字符串
    /// </summary>
    /// <param name="totalSeconds"></param>
    /// <returns></returns>
    public string ConvertSecondsToTimeString(long totalSeconds)
    {
        // 时间单位的秒数
        long years = totalSeconds / (365 * 24 * 60 * 60);
        totalSeconds %= (365 * 24 * 60 * 60);

        long months = totalSeconds / (30 * 24 * 60 * 60);
        totalSeconds %= (30 * 24 * 60 * 60);

        long days = totalSeconds / (24 * 60 * 60);
        totalSeconds %= (24 * 60 * 60);

        long hours = totalSeconds / (60 * 60);
        totalSeconds %= (60 * 60);

        long minutes = totalSeconds / 60;
        totalSeconds %= 60;

        long seconds = totalSeconds;

        // 拼接字符串（没有空格）
        List<string> timeParts = new();

        if (years > 0)
            timeParts.Add(years + "year");
        if (months > 0)
            timeParts.Add(months + "mon");
        if (days > 0)
            timeParts.Add(days + "day");
        if (hours > 0)
            timeParts.Add(hours + "hour");
        if (minutes > 0)
            timeParts.Add(minutes + "min");
        if (seconds > 0)
            timeParts.Add(seconds + "sec");

        // 返回拼接后的字符串，如果没有任何单位则返回 "0seconds"
        return timeParts.Count > 0 ? string.Join("", timeParts) : "0sec";
    }

    // 获取输入框中的文字，并转换为秒
    public long GetTimeAndConvertToSeconds(TMP_InputField _InputField)
    {
        string inputText = _InputField.text;

        if (string.IsNullOrEmpty(inputText))
        {
            TipWindowManager.Instance.ShowTip("输入框为空", Color.black);
            return 0;
        }

        return ConvertTimeToSeconds(inputText);
    }

    public List<int> GetTimeAndConvertToArrary(TMP_InputField _InputField)
    {
        string inputText = _InputField.text;

        if (string.IsNullOrEmpty(inputText))
        {
            TipWindowManager.Instance.ShowTip("输入框为空", Color.black);
            return new List<int> { 0, 0, 0, 0, 0, 0 };
        }

        return ConvertTimeToArray(inputText);
    }
}
