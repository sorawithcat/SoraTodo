using System;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    /// <summary>
    /// 最后关闭应用的时间戳
    /// </summary>
    public long lastLeaveTime;

    /// <summary>
    /// 选择的主题
    /// </summary>
    public int selectedTheme;

    /// <summary>
    /// 主题
    /// </summary>
    public List<ThemeColor> themeColors;

    /// <summary>
    /// 自动删除7天前的文件
    /// </summary>
    public bool autoClearLogs;

    /// <summary>
    /// 当前完成的待办
    /// </summary>
    public int currentClearTodo;

    /// <summary>
    /// 最近7天清除的待办数
    /// </summary>
    public List<double> nearClearTodoNumbs;

    /// <summary>
    /// 备忘录
    /// </summary>
    public string memorandum;

    /// <summary>
    /// 设置界面的开关按钮是否打开
    /// </summary>
    public SerializableDictionary<string, bool> settingToggleButtonsIsOn;

    /// <summary>
    /// 默认展示小面板
    /// </summary>
    public bool showMiniPanle;

    /// <summary>
    /// 窗口背景是否透明
    /// </summary>
    public bool isOpaque;

    public GameData()
    {
        lastLeaveTime = DateTime.UtcNow.Ticks;
        selectedTheme = 0;
        themeColors = new List<ThemeColor>();
        autoClearLogs = false;
        currentClearTodo = 0;
        nearClearTodoNumbs = new List<double>() { 0, 0, 0, 0, 0, 0, 0 };
        memorandum = "";
        settingToggleButtonsIsOn = new SerializableDictionary<string, bool>();
        showMiniPanle = false;
        isOpaque = true;
    }
}