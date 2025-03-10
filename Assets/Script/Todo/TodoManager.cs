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
    /// </summary>
    /// 删除线和字体变灰
    StrikethroughAndFontAreGrayedOut,

    /// <summary>
    /// 删除自身
    /// </summary>
    DestroyThis,
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

[System.Serializable]
public class TodoManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("待办的字体组件")]
    public TextMeshProUGUI todoText;

    public Image bg;

    [Header("渐变的shader")]
    [SerializeField] private Shader useShader;

    [Header("渐变的开始颜色")]
    public Color startColor = new(1f, 0.435f, 0.376f, 1f);

    [Header("渐变的结束颜色")]
    public Color endColor = new(143f / 255f, 0f / 255f, 16f / 255f, 1f);

    [Header("完成效果")]
    public ClearFX clearFX;

    [Header("定时类型")]
    public TimingType timingType;

    [Header("闹钟铃声类型")]
    public AlarmType alarmType;

    [Header("音效设置")]
    [SerializeField] private AudioSource audioSource;

    public float countdownTime = 60;
    public DateTime dateTime;
    private AudioClip alarmClip;
    public float timer;

    [Header("自定义闹钟的文件路径")]
    public string customizePath;

    // 定时器状态
    public bool isCountingDown = false;

    public bool isAlarmPlayed = false;

    // 是否在按住
    private bool isPointer = false;

    [HideInInspector] public Material newMaterial;

    // 是否完成
    public bool isTodo = false;

    //自定义的路径是否更改
    public bool isChangeCustomize = true;

    public string todoID = "";

    private void Start()
    {
        SetAlarm();

        // 渐变shader设置
        if (bg.TryGetComponent<Image>(out var image))
        {
            newMaterial = new Material(useShader)
            {
                renderQueue = 5000
            };
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
                TipWindowManager.Instance.ShowTip(todoText.text + ":未找到指定的Shader: " + useShader.name, Color.yellow);
            }
        }
        else
        {
            TipWindowManager.Instance.ShowTip(todoText.text + ":此物体上没有找到Renderer组件！", Color.red);
        }
    }

    /// <summary>
    /// 设置闹钟并启动
    /// </summary>
    /// <param name="mustTodo">为true时无视条件强制执行</param>
    public void SetAlarm(bool mustTodo = false)
    {
        if (!isTodo || mustTodo)
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
                StepTextColor.Instance.SetTextAndColor(StepTextColor.Instance.currentNumb + 1);
                MainLineChartManager.Instance.Append();
                SetClearFX();
                if (clearFX != ClearFX.DestroyThis)
                {
                    EndSetJson();
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

    private void EndSetJson()
    {
        LoadAllData.Instance.UpdateTodoManager(todoID, "timeType", (int)timingType);
        LoadAllData.Instance.UpdateTodoManager(todoID, "alarmType", (int)alarmType);
        LoadAllData.Instance.UpdateTodoManager(todoID, "timer", timer);
        LoadAllData.Instance.UpdateTodoManager(todoID, "countdownTime", countdownTime);
        LoadAllData.Instance.UpdateTodoManager(todoID, "dateTime", dateTime.Ticks);
        LoadAllData.Instance.UpdateTodoManager(todoID, "customizePath", customizePath);
        LoadAllData.Instance.UpdateTodoManager(todoID, "isTodo", isTodo);
        LoadAllData.Instance.UpdateTodoManager(todoID, "isCountingDown", isCountingDown);
        LoadAllData.Instance.UpdateTodoManager(todoID, "isAlarmPlayed", isAlarmPlayed);
        LoadAllData.Instance.UpdateTodoManager(todoID, "isChangeCustomize", isChangeCustomize);
    }

    public void SaveAllInJson()
    {
        EndSetJson();
        LoadAllData.Instance.UpdateTodoManager(todoID, "titleBGStartColor", SetColor.Instance.RGBToString(startColor));
        LoadAllData.Instance.UpdateTodoManager(todoID, "titleBGEndColor", SetColor.Instance.RGBToString(endColor));
        LoadAllData.Instance.UpdateTodoManager(todoID, "titleColor", SetColor.Instance.RGBToString(todoText.color));
    }

    public void SetClearFX(bool isShow = false)
    {
        if (newMaterial != null)
            newMaterial.SetVector("_GradientStartUV", new Vector4(2f, 0.5f, 0, 0));
        switch (clearFX)
        {
            case ClearFX.StrikethroughAndFontAreGrayedOut:
                todoText.color = Color.gray;
                todoText.text = $"<s>{todoText.text}</s>";
                break;

            case ClearFX.DestroyThis:
                TimerManager.Instance.UnregisterTodoManager(this);
                if (!isShow)
                {
                    LoadAllData.Instance.RemoveTodoManager(todoID, transform.parent.parent.GetSiblingIndex());
                }
                Destroy(this.gameObject);
                break;
        }
    }

    public bool UpdateTimer()
    {
        if (isCountingDown && timingType != TimingType.None)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f && !isAlarmPlayed)
            {
                //想了很久，结果就是个延迟激活就能解决的问题）
                if (alarmType != AlarmType.None)
                {
                    Invoke(nameof(PlayAlarmSound), 0.1f);
                    isAlarmPlayed = true;
                }
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
                if (isChangeCustomize)
                {
                    FolderBrowserHelper.SetAudioClip(customizePath, audioSource);
                }
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
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("todoThing"))
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
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("todoThing"))
        {
            if (eventData.button == 0)
            {
                isPointer = false;
            }
        }
    }

    /// <summary>
    /// 用于外部更新定时器设置
    /// </summary>
    /// <param name="newTimingType"></param>
    /// <param name="newAlarmType"></param>
    /// <param name="newTime"></param>
    /// <param name="_newDate"></param>
    /// <param name="_customizePath"></param>
    public void UpdateTimerSettings(TimingType newTimingType, AlarmType newAlarmType, float newTime = 60f, DateTime? _newDate = null, string _customizePath = "")
    {
        timingType = newTimingType;
        alarmType = newAlarmType;
        if (_customizePath != customizePath && newAlarmType == AlarmType.Customize)
        {
            customizePath = _customizePath;
            isChangeCustomize = true;
        }
        else if (_customizePath == customizePath && newAlarmType == AlarmType.Customize)
        {
            isChangeCustomize = false;
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
        long year = _datetime.Year;
        long month = _datetime.Month;
        long day = _datetime.Day;
        long hour = _datetime.Hour;
        long minute = _datetime.Minute;
        long second = _datetime.Second;

        long totalSeconds = year * 365 * 24 * 60 * 60 + month * 30 * 24 * 60 * 60 + day * 24 * 60 * 60 + hour * 3600 + minute * 60 + second;
        return totalSeconds;
    }
}