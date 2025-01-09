using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 完成效果
/// </summary>
public enum ClearFX
{
    /// <summary>
    /// 删除线和字体变灰
    /// </summary>
    StrikethroughAndFontAreGrayedOut,
}

/// <summary>
/// 定时类型
/// </summary>
public enum TimingType
{
    /// <summary>
    /// 无定时
    /// </summary>
    None,
    /// <summary>
    /// 倒计时
    /// </summary>
    Countdown,
    /// <summary>
    /// 日期
    /// </summary>
    Date,
}

/// <summary>
/// 闹钟音效类型
/// </summary>
public enum AlarmType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 预设1
    /// </summary>
    Alarm1,
    /// <summary>
    /// 自定义
    /// </summary>
    Customize
}

public class TodoManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("待办的字体组件")]
    [SerializeField] private TextMeshProUGUI todoText;
    [SerializeField] private Image bg;

    [Header("渐变的shader")]
    [SerializeField] private Shader useShader;

    [Header("渐变的开始颜色")]
    [SerializeField] private Color startColor = new Color(1f, 0.435f, 0.376f, 1f);

    [Header("渐变的结束颜色")]
    [SerializeField] private Color endColor = new Color(143f / 255f, 0f / 255f, 16f / 255f, 1f);

    [Header("完成效果")]
    [SerializeField] private ClearFX clearFX;

    [Header("定时类型")]
    public TimingType timingType;

    [Header("闹钟铃声类型")]
    public AlarmType alarmType;

    [Header("音效设置")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] public float countdownTime = 60;
    public DateTime dateTime;
    private AudioClip alarmClip;
    public float timer;

    [Header("自定义闹钟的文件路径")]
    public string customizePath;

    // 定时器状态
    private bool isCountingDown = false;
    private bool isAlarmPlayed = false;

    // 是否在按住
    private bool isPointer = false;
    [HideInInspector] public Material newMaterial;

    // 是否完成
    private bool isTodo = false;
    //自定义的路径是否更改
    //private bool isChangeCustomize = false;

    void Start()
    {
            SetAlarm();

        // 渐变shader设置
        if (bg.TryGetComponent<Image>(out var image))
        {
            newMaterial = new Material(useShader);
            newMaterial.renderQueue = 5000;
            if (newMaterial.shader != null)
            {
                newMaterial.SetColor("_GradientStartColor", startColor);
                newMaterial.SetColor("_GradientEndColor", endColor);
                newMaterial.SetVector("_GradientDirection", new Vector4(-1, 0, 0, 0));
                newMaterial.SetVector("_GradientStartUV", new Vector4(0, 0.5f, 0, 0));
                image.material = newMaterial;
            }
            else
            {
                TipWindowManager.Instance.ShowTip("未找到指定的Shader: " + useShader.name, Color.yellow);
            }
        }
        else
        {
            TipWindowManager.Instance.ShowTip("当前物体上没有找到Renderer组件！", Color.red);
        }
    }

    public void SetAlarm()
    {
        LoadAlarmSound();

        if (timingType == TimingType.Countdown)
        {
            timer = countdownTime; // 设置倒计时的初始时间
            isCountingDown = true;
        }
        else if (timingType == TimingType.Date)
        {
            // 计算日期倒计时，假设目标日期是某个特定时间点
            DateTime targetDate = dateTime;
            DateTime currentDate = DateTime.Now;
            TimeSpan timeRemaining = targetDate - currentDate;
            timer = (float)timeRemaining.TotalSeconds;
            isCountingDown = true;
        }
        else if (timingType == TimingType.None)
        {
            // 没有定时任务时，不做任何操作
            timer = 0f;
            isCountingDown = false;
        }
    }

    private float _GradientStartUVNumb;

    private void Update()
    {
        // 渐变效果和完成状态
        if (isPointer && !isTodo)
        {
            if (_GradientStartUVNumb >= 2f)
            {
                isTodo = true;
                switch (clearFX)
                {
                    case ClearFX.StrikethroughAndFontAreGrayedOut:
                        todoText.color = Color.gray;
                        todoText.text = $"<s>{todoText.text}</s>";
                        break;
                }
            }
            else
            {
                _GradientStartUVNumb = newMaterial.GetVector("_GradientStartUV").x + Time.deltaTime * 2f;
                _GradientStartUVNumb = Mathf.Clamp(_GradientStartUVNumb, 0f, 2f);
                newMaterial.SetVector("_GradientStartUV", new Vector4(_GradientStartUVNumb, 0.5f, 0, 0));
            }
        }
        else if (!isPointer && !isTodo)
        {
            _GradientStartUVNumb = newMaterial.GetVector("_GradientStartUV").x - Time.deltaTime * 4f;
            _GradientStartUVNumb = Mathf.Clamp(_GradientStartUVNumb, 0f, 2f);
            newMaterial.SetVector("_GradientStartUV", new Vector4(_GradientStartUVNumb, 0.5f, 0, 0));
        }
    }

    public bool UpdateTimer()
    {
        bool isTimer = false;

        if (isCountingDown && timingType != TimingType.None)
        {
            timer -= Time.deltaTime;

            if (timer <= 1f && timer > 0f && isAlarmPlayed)
            {
                isTimer = true; 
            }

            if (timer <= 0f && !isAlarmPlayed)
            {
                PlayAlarmSound();
                isAlarmPlayed = true;

                timingType = TimingType.None;
                isTimer = false; 
            }
        }

        return isTimer;
    }


    private void LoadAlarmSound()
    {
        if (alarmType == AlarmType.None)
        {
            return;
        }

        string soundPath = "AlarmClockSounds/";

        switch (alarmType)
        {
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
                break;
            case AlarmType.Customize:
                //if (isChangeCustomize)
                //{
                    FolderBrowserHelper.SetAudioClip(customizePath, audioSource);
               // }
                break;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        RightMenuManager.Instance.HideRightMenu();
        if (eventData.pointerCurrentRaycast.gameObject.tag == "todoThing")
        {
            if (eventData.button == 0)
            {
                isPointer = true;
            }
            else if ((int)eventData.button == 1)
            {
                RightMenuManager.Instance.GetMenuInfo(MenuTags.todoThing, transform);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag == "todoThing")
        {
            if (eventData.button == 0)
            {
                isPointer = false;
            }
        }
    }
    // 用于外部更新定时器设置
    public void UpdateTimerSettings(TimingType newTimingType, AlarmType newAlarmType, float newTime = 60f, DateTime? _newDate = null, string _customizePath = "")
    {
        timingType = newTimingType;
        alarmType = newAlarmType;
        if (_customizePath != customizePath && newAlarmType == AlarmType.Customize)
        {
            customizePath = _customizePath;
            //isChangeCustomize = true;
        }
        else if (_customizePath == customizePath && newAlarmType == AlarmType.Customize)
        {
            //isChangeCustomize = false;
        }
        countdownTime = newTime;

        if (!_newDate.HasValue)
        {
            _newDate = DateTime.Now.AddHours(1);
        }
        dateTime = _newDate.Value;
        if (newTimingType == TimingType.Countdown)
        {

            timer = newTime;
        }
        else if (newTimingType == TimingType.Date)
        {
            DateTime currentDate = DateTime.Now;
            long currentTime = ConvertDateTimeToSeconds(currentDate);
            long toTime = ConvertDateTimeToSeconds(dateTime);
            timer = toTime - currentTime;
            if (timer <= 0)
            {
                timer = 0;
            }
        }

        isCountingDown = (newTimingType == TimingType.Countdown || newTimingType == TimingType.Date);
        isAlarmPlayed = false;
    }
    public long ConvertDateTimeToSeconds(DateTime _datetime)
    {
        long totalSeconds = 0;

        long year = _datetime.Year;
        long month = _datetime.Month;
        long day = _datetime.Day;
        long hour = _datetime.Hour;
        long minute = _datetime.Minute;
        long second = _datetime.Second;

        totalSeconds = year * 365 * 24 * 60 * 60 + month * 30 * 24 * 60 * 60 + day * 24 * 60 * 60 + hour * 3600 + minute * 60 + second;
        return totalSeconds;
    }
}
