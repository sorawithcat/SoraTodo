using System;
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
    [SerializeField] private float countdownTime = 60f;
    [SerializeField] private DateTime dateTime = DateTime.Now.AddHours(1);
    private AudioClip alarmClip;
    public float timer;

    // 定时器状态
    private bool isCountingDown = false;
    private bool isAlarmPlayed = false;

    // 是否在按住
    private bool isPointer = false;
    [HideInInspector] public Material newMaterial;

    // 是否完成
    private bool isTodo = false;

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
                Debug.LogError("未找到指定的Shader: " + useShader.name);
            }
        }
        else
        {
            Debug.LogError("当前物体上没有找到Renderer组件！");
        }
    }

    public void SetAlarm()
    {
        // 根据AlarmType加载铃声
        LoadAlarmSound();

        // 根据定时类型设置定时器
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

    // 添加 UpdateTimer 方法
    public bool UpdateTimer()
    {
        // 如果处于倒计时状态，更新倒计时器
        if (isCountingDown && timingType != TimingType.None)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f && !isAlarmPlayed)
            {
                // 时间到，播放铃声
                PlayAlarmSound();
                isAlarmPlayed = true;

                // 设置为无定时状态
                timingType = TimingType.None;
                return false;
            }
        }
        return true;
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
                break;
            case AlarmType.Customize:
                soundPath += "Customize";
                break;
        }

        alarmClip = Resources.Load<AudioClip>(soundPath);

        if (alarmClip == null)
        {
            Debug.LogError("未找到铃声文件: " + soundPath);
        }
    }

    public void PlayAlarmSound()
    {
        if (alarmClip != null && audioSource != null)
        {
            audioSource.clip = alarmClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("铃声或AudioSource为空，无法播放铃声！");
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
    public void UpdateTimerSettings(TimingType newTimingType,AlarmType newAlarmType, float newTime = 60f, DateTime? _newDate = null)
    {
        timingType = newTimingType;
        alarmType = newAlarmType;
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
            TimeSpan timeRemaining = dateTime - currentDate;
            timer = (float)timeRemaining.TotalSeconds;
        }

        isCountingDown = (newTimingType == TimingType.Countdown || newTimingType == TimingType.Date);
        isAlarmPlayed = false;
    }
}
