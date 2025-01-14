using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

[System.Serializable]
public class TodoManagerData
{
    public string id;
    public string title;
    public string titleColor;
    public string titleBGStartColor;
    public string titleBGEndColor;
    public int clearFX;
    public int timeType;
    public int alarmType;
    public float countdownTime;
    public long dateTime;
    public float timer;
    public string customizePath;
    public bool isCountingDown;
    public bool isAlarmPlayed;
    public bool isTodo;
    public bool isChangeCustomize;
}

[System.Serializable]
public class ClassifyButtonManagerData
{
    public string id;
    public int siblingIndex;
    public string title;
    public string titleColor;
    public string titleBGColor;
    public string[] todos;  // Use array instead of List
}

public class LoadAllData : MonoBehaviour
{
    public static LoadAllData Instance;
    [Header("预定义的预制件")]
    public GameObject classifyButtonPrefab;
    public GameObject todoManagerPrefab;

    [Header("UI 相关引用")]
    public Transform classifyButtonContainer;

    private ClassifyButtonManagerData[] classifyButtonManagerDatas;
    private TodoManagerData[] todoManagerDatas;
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
        LoadDataFromJson();
    }

    // 从JSON文件加载数据
    void LoadDataFromJson()
    {
        string classifyButtonJson = File.ReadAllText("Assets/Data/ClassifyButtonManagerData.json");
        string todoManagerJson = File.ReadAllText("Assets/Data/TodoManagerData.json");

        classifyButtonManagerDatas = JsonUtility.FromJson<Wrapper<ClassifyButtonManagerData>>(classifyButtonJson).items;
        todoManagerDatas = JsonUtility.FromJson<Wrapper<TodoManagerData>>(todoManagerJson).items;

        CreateClassifyButtons();
    }

    /// <summary>
    /// 创建分类按钮
    /// </summary>
    void CreateClassifyButtons()
    {
        // 按照 siblingIndex 排序 classifyButtonManagerDatas
        Array.Sort(classifyButtonManagerDatas, (a, b) => int.Parse(a.siblingIndex.ToString()).CompareTo(int.Parse(b.siblingIndex.ToString())));

        foreach (var classifyButtonData in classifyButtonManagerDatas)
        {
            // 创建分类按钮的预制件
            GameObject classifyButton = Instantiate(classifyButtonPrefab, classifyButtonContainer);
            classifyButton.GetComponent<ClassifyButtonManager>().buttonsList = classifyButtonContainer.gameObject;
            classifyButton.GetComponent<ClassifyButtonManager>().todoList = classifyButton.GetComponentInChildren<TodoList>().gameObject;

            // 设置分类按钮的文本
            var buttonText = classifyButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = classifyButtonData.title;

            // 设置分类按钮的背景颜色
            var buttonBg = classifyButton.GetComponent<Image>();
            buttonBg.color = HexToColor(classifyButtonData.titleBGColor);

            // 添加 TodoManager 的子物体
            // 根据 todos 数组中的顺序来生成 TodoManager
            foreach (var todoId in classifyButtonData.todos)
            {
                var todoData = Array.Find(todoManagerDatas, td => td.id == todoId);
                if (todoData != null)
                {
                    CreateTodoManager(todoData, classifyButton.GetComponent<ClassifyButtonManager>().todoList.transform);
                }
                else
                {
                    Debug.LogWarning($"Todo with ID {todoId} not found.");
                }
            }
        }

        // 完成所有UI元素创建后，重新计算并应用布局
        ApplyLayout();
    }

    void CreateTodoManager(TodoManagerData todoData, Transform parent)
    {
        GameObject todoManagerObj = Instantiate(todoManagerPrefab, parent);
        TodoManager todoManager = todoManagerObj.GetComponent<TodoManager>();

        // 延迟设置 TodoManager 的数据，不延迟会有clip为空的错误
        StartCoroutine(DelayedSetTodoData(0.2f, todoData, todoManager));
    }

    private IEnumerator DelayedSetTodoData(float delay, TodoManagerData todoData, TodoManager todoManager)
    {
        yield return new WaitForSeconds(delay);
        SetTodoData(todoData, todoManager);
    }
    private void SetTodoData(TodoManagerData todoData, TodoManager todoManager)
    {
        todoManager.todoText.text = todoData.title;
        todoManager.startColor = HexToColor(todoData.titleBGStartColor);
        todoManager.endColor = HexToColor(todoData.titleBGEndColor);
        todoManager.clearFX = (ClearFX)todoData.clearFX;
        todoManager.isCountingDown = todoData.isCountingDown;
        todoManager.isAlarmPlayed = todoData.isAlarmPlayed;
        todoManager.isTodo = todoData.isTodo;
        DateTime dateTime = new DateTime(todoData.dateTime);

        TimerManager.Instance.UpdateTodoTimerSetting(todoManager,
            (TimingType)todoData.timeType,
            (AlarmType)todoData.alarmType,
            todoData.timer,
            dateTime,
            todoData.customizePath
        );
    }


    /// <summary>
    /// 将十六进制颜色值转换为 Unity Color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return Color.white; // 默认白色
    }

    /// <summary>
    /// 更新分类json文件的某个属性
    /// </summary>
    /// <param name="id"></param>
    /// <param name="propertyName"></param>
    /// <param name="newValue"></param>
    public void UpdateClassifyButton(string id, string propertyName, object newValue)
    {
        var classifyButton = Array.Find(classifyButtonManagerDatas, cb => cb.id == id);
        if (classifyButton != null)
        {
            // 使用反射找到该属性并更新
            var propertyInfo = typeof(ClassifyButtonManagerData).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(classifyButton, newValue);
                SaveDataToJson(); // 保存更新到 JSON 文件
            }
            else
            {
                Debug.LogError("Property not found: " + propertyName);
            }
        }
        else
        {
            Debug.LogError("ClassifyButton with id " + id + " not found");
        }
    }
    /// <summary>
    /// 更新todo json文件的某个属性
    /// </summary>
    /// <param name="id"></param>
    /// <param name="propertyName"></param>
    /// <param name="newValue"></param>
    public void UpdateTodoManager(string id, string propertyName, object newValue)
    {
        var todoManager = Array.Find(todoManagerDatas, td => td.id == id);
        if (todoManager != null)
        {
            // 使用反射找到该属性并更新
            var propertyInfo = typeof(TodoManagerData).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(todoManager, newValue);
                SaveDataToJson(); // 保存更新到 JSON 文件
            }
            else
            {
                Debug.LogError("属性没找到: " + propertyName);
            }
        }
        else
        {
            Debug.LogError("id没找到 " + id);
        }
    }


    // 将数据保存到JSON文件
    void SaveDataToJson()
    {
        string classifyButtonJson = JsonUtility.ToJson(new Wrapper<ClassifyButtonManagerData>(classifyButtonManagerDatas), true);
        string todoManagerJson = JsonUtility.ToJson(new Wrapper<TodoManagerData>(todoManagerDatas), true);

        File.WriteAllText("Assets/Data/ClassifyButtonManagerData.json", classifyButtonJson);
        File.WriteAllText("Assets/Data/TodoManagerData.json", todoManagerJson);
    }

    // 添加分类按钮
    public void AddClassifyButton(ClassifyButtonManagerData newData)
    {
        var list = new List<ClassifyButtonManagerData>(classifyButtonManagerDatas);
        list.Add(newData);
        classifyButtonManagerDatas = list.ToArray();
        SaveDataToJson(); // 保存到JSON文件
        CreateClassifyButtons(); // 重新创建分类按钮并应用布局
    }

    // 删除分类按钮
    public void RemoveClassifyButton(string id)
    {
        classifyButtonManagerDatas = Array.FindAll(classifyButtonManagerDatas, cb => cb.id != id);
        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    // 添加待办事项
    public void AddTodoManager(TodoManagerData newData)
    {
        var list = new List<TodoManagerData>(todoManagerDatas);
        list.Add(newData);
        todoManagerDatas = list.ToArray();
        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    // 删除待办事项
    public void RemoveTodoManager(string id)
    {
        todoManagerDatas = Array.FindAll(todoManagerDatas, td => td.id != id);
        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    // 重新计算并应用布局
    private void ApplyLayout()
    {
        // 对分类按钮和待办事项进行布局更新
        classifyButtonContainer.GetComponent<CustomVerticalLayoutGroup>().RecalculateTotalHeight();
    }
}

// 用于包装数组的类，适配 JsonUtility
[System.Serializable]
public class Wrapper<T>
{
    public T[] items;

    public Wrapper(T[] items)
    {
        this.items = items;
    }
}
