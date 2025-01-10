using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// 待办
    /// </summary>
    public SerializableDictionary<string, TodoManager> todos;
    /// <summary>
    /// 分类
    /// </summary>
    public SerializableDictionary<string, ClassifyButtonManager> classifyButtons;


    public GameData()
    {
        lastLeaveTime = DateTime.UtcNow.Ticks;
        selectedTheme = 0;
        todos = new SerializableDictionary<string, TodoManager>();
        classifyButtons = new SerializableDictionary<string, ClassifyButtonManager>();
    }
}
