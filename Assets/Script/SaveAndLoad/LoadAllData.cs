using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ClassifyButtonManagerData
{
    public int siblingIndex;  // 直接用 siblingIndex 替代 id
    public string title;
    public string titleColor;
    public string titleBGColor;
    public string[] todos;  // Use array instead of List
}

[System.Serializable]
public class TodoManagerData
{
    public string todoID;
    public string title;
    public string titleColor;
    public string titleBGStartColor;
    public string titleBGEndColor;
    public int clearFX;
    public bool isCountingDown;
    public bool isAlarmPlayed;
    public bool isTodo;
    public long dateTime;
    public int timeType;
    public int alarmType;
    public float timer;
    public string customizePath;
    public bool isChangeCustomize;
    public float countdownTime;
}

public class LoadAllData : MonoBehaviour, ISaveManger
{
    public static LoadAllData Instance;

    [Header("预定义的预制件")]
    public GameObject classifyButtonPrefab;

    public GameObject todoManagerPrefab;

    [Header("UI 相关引用")]
    public Transform classifyButtonContainer;

    private ClassifyButtonManagerData[] classifyButtonManagerDatas;
    private TodoManagerData[] todoManagerDatas;

    private readonly string classifyADD = "Data/ClassifyButtonManagerData.json";
    private readonly string todoADD = "Data/TodoManagerData.json";
    private readonly string classifyButtonInitialJson = "{\"items\": [{\"siblingIndex\": 0, \"title\": \"不急着做\", \"titleColor\": \"#000\", \"titleBGColor\": \"#FFFFFF\", \"todos\": [\"0\"]}]}";
    private readonly string todoManagerInitialJson = "{\"items\": [{\"todoID\": \"0\", \"title\": \"Hello~\", \"titleColor\": \"#000\", \"titleBGStartColor\": \"#FF6F60\", \"titleBGEndColor\": \"#FFFF63\", \"clearFX\": 0, \"isCountingDown\": false, \"isAlarmPlayed\": false, \"isTodo\": false, \"dateTime\": 0, \"timeType\": 0, \"alarmType\": 0, \"timer\": 0.0, \"customizePath\": \"\", \"isChangeCustomize\": true, \"countdownTime\": 60.0}]}";

    public bool showMini;

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

        // 检查文件并创建初始内容
        CheckAndCreateFile("Data/ClassifyButtonManagerData.json", classifyButtonInitialJson);
        CheckAndCreateFile("Data/TodoManagerData.json", todoManagerInitialJson);
        if (showMini)
        {
            MainPanle.Instance.CloseWindow();
        }
    }

    private void Start()
    {
        LoadDataFromJson();
    }

    public void LoadData(GameData _data)
    {
        showMini = _data.showMiniPanle;
    }

    public void SaveData(ref GameData _data)
    {
        _data.showMiniPanle = showMini;
    }

    private void CheckAndCreateFile(string filePath, string initialContent)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        string directoryPath = Path.GetDirectoryName(fullPath);

        // 如果目录不存在，创建目录
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 如果文件不存在，创建文件并写入初始内容
        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, initialContent);
        }
    }

    // 从JSON文件加载数据
    public void LoadDataFromJson()
    {
        string classifyButtonPath = Path.Combine(Application.persistentDataPath, classifyADD);
        string todoManagerPath = Path.Combine(Application.persistentDataPath, todoADD);

        if (File.Exists(classifyButtonPath) && File.Exists(todoManagerPath))
        {
            string classifyButtonJson = File.ReadAllText(classifyButtonPath);
            string todoManagerJson = File.ReadAllText(todoManagerPath);

            classifyButtonManagerDatas = JsonUtility.FromJson<Wrapper<ClassifyButtonManagerData>>(classifyButtonJson).items;
            todoManagerDatas = JsonUtility.FromJson<Wrapper<TodoManagerData>>(todoManagerJson).items;

            CreateClassifyButtons();
        }
        else
        {
            TipWindowManager.Instance.ShowTip("未能加载JSON文件，请确保文件存在", Color.red);
        }
    }

    /// <summary>
    /// 创建分类按钮
    /// </summary>
    private void CreateClassifyButtons()
    {
        // 按照 siblingIndex 排序 classifyButtonManagerDatas
        Array.Sort(classifyButtonManagerDatas, (a, b) => a.siblingIndex.CompareTo(b.siblingIndex));
        foreach (var classifyButtonData in classifyButtonManagerDatas)
        {
            // 创建分类按钮的预制件
            GameObject classifyButton = Instantiate(classifyButtonPrefab, classifyButtonContainer);
            classifyButton.GetComponent<ClassifyButtonManager>().buttonsList = classifyButtonContainer.gameObject;
            classifyButton.GetComponent<ClassifyButtonManager>().todoList = classifyButton.GetComponentInChildren<TodoList>().gameObject;

            // 设置分类按钮的文本
            var buttonText = classifyButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = classifyButtonData.title;
            buttonText.color = HexToColor(classifyButtonData.titleColor);

            // 设置分类按钮的背景颜色
            var buttonBg = classifyButton.GetComponent<Image>();
            buttonBg.color = HexToColor(classifyButtonData.titleBGColor);

            // 设置分类按钮的 ID 为 siblingIndex
            classifyButton.GetComponent<ClassifyButtonManager>().siblingIndex = classifyButtonData.siblingIndex;

            // 添加 TodoManager 的子物体
            foreach (var todoId in classifyButtonData.todos)
            {
                var todoData = Array.Find(todoManagerDatas, td => td.todoID == todoId);
                if (todoData != null)
                {
                    CreateTodoManager(todoData, classifyButton.GetComponent<ClassifyButtonManager>().todoList.transform);
                }
                else
                {
                    TipWindowManager.Instance.ShowTip($"错误的TodoID： {todoId}", Color.red);
                }
            }
        }
        // 完成所有UI元素创建后，重新计算并应用布局
        ApplyLayout();
    }

    private void CreateTodoManager(TodoManagerData todoData, Transform parent)
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
        todoManager.todoText.color = HexToColor(todoData.titleColor);
        todoManager.startColor = HexToColor(todoData.titleBGStartColor);
        todoManager.newMaterial.SetColor("_GradientStartColor", HexToColor(todoData.titleBGStartColor));
        todoManager.endColor = HexToColor(todoData.titleBGEndColor);
        todoManager.newMaterial.SetColor("_GradientEndColor", HexToColor(todoData.titleBGEndColor));
        todoManager.clearFX = (ClearFX)todoData.clearFX;
        todoManager.isCountingDown = todoData.isCountingDown;
        todoManager.isAlarmPlayed = todoData.isAlarmPlayed;
        todoManager.isTodo = todoData.isTodo;
        todoManager.isChangeCustomize = todoData.isChangeCustomize;
        todoManager.countdownTime = todoData.countdownTime;
        todoManager.todoID = todoData.todoID;
        DateTime dateTime = new(todoData.dateTime);
        TimerManager.Instance.UpdateTodoTimerSetting(todoManager,
            (TimingType)todoData.timeType,
            (AlarmType)todoData.alarmType,
            todoData.timer,
            dateTime,
            todoData.customizePath
        );
        if (todoManager.isTodo)
        {
            todoManager.SetClearFX();
        }
    }

    /// <summary>
    /// 将十六进制颜色值转换为 Unity Color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
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
    public void UpdateClassifyButton(int siblingIndex, string propertyName, object newValue)
    {
        var classifyButton = Array.Find(classifyButtonManagerDatas, cb => cb.siblingIndex == siblingIndex);
        if (classifyButton != null)
        {
            var propertyInfo = typeof(ClassifyButtonManagerData).GetField(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(classifyButton, newValue);
                SaveDataToJson(); // 保存更新到 JSON 文件
            }
            else
            {
                TipWindowManager.Instance.ShowTip("属性没找到: " + propertyName, Color.red);
            }
        }
        else
        {
            TipWindowManager.Instance.ShowTip("siblingIndex没找到：" + siblingIndex, Color.red);
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
        var todoManager = Array.Find(todoManagerDatas, td => td.todoID == id);
        if (todoManager != null)
        {
            var propertyInfo = typeof(TodoManagerData).GetField(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(todoManager, newValue);
                SaveDataToJson(); // 保存更新到 JSON 文件
            }
            else
            {
                TipWindowManager.Instance.ShowTip("属性没找到: " + propertyName, Color.red);
            }
        }
        else
        {
            TipWindowManager.Instance.ShowTip("id没找到 ：" + id, Color.red);
        }
    }

    // 将数据保存到JSON文件
    private void SaveDataToJson()
    {
        string classifyButtonJson = JsonUtility.ToJson(new Wrapper<ClassifyButtonManagerData>(classifyButtonManagerDatas), true);
        string todoManagerJson = JsonUtility.ToJson(new Wrapper<TodoManagerData>(todoManagerDatas), true);

        string classifyButtonPath = Path.Combine(Application.persistentDataPath, classifyADD);
        string todoManagerPath = Path.Combine(Application.persistentDataPath, todoADD);

        File.WriteAllText(classifyButtonPath, classifyButtonJson);
        File.WriteAllText(todoManagerPath, todoManagerJson);
    }

    /// <summary>
    /// 添加待办事项
    /// </summary>
    /// <param name="newData"></param>
    /// <param name="classifyId"></param>
    public void AddTodoManager(TodoManagerData newData, int classifyId)
    {
        // 添加新的 Todo 数据
        var list = new List<TodoManagerData>(todoManagerDatas)
        {
            newData
        };
        todoManagerDatas = list.ToArray();

        // 更新分类按钮的 todos 数组
        foreach (var classifyButton in classifyButtonManagerDatas)
        {
            if (classifyButton.siblingIndex == classifyId)
            {
                List<string> updatedTodos = new(classifyButton.todos)
                {
                    newData.todoID  // 添加新待办事项 ID
                };
                classifyButton.todos = updatedTodos.ToArray();  // 更新分类中的 todos 数组
                break;
            }
        }

        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="classifyId"></param>
    public void RemoveTodoManager(string id, int classifyId)
    {
        // 从 todoManagerDatas 中删除待办事项
        todoManagerDatas = Array.FindAll(todoManagerDatas, td => td.todoID != id);

        // 更新指定分类数据，移除包含该待办事项 ID 的分类
        foreach (var classifyButton in classifyButtonManagerDatas)
        {
            if (classifyButton.siblingIndex == classifyId)
            {
                List<string> updatedTodos = new();
                foreach (var todoId in classifyButton.todos)
                {
                    if (todoId != id)
                    {
                        updatedTodos.Add(todoId);  // 保留不等于删除 ID 的待办事项
                    }
                }
                classifyButton.todos = updatedTodos.ToArray();  // 更新分类中的 todos 数组
                break;
            }
        }

        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    /// <summary>
    /// 添加分类按钮
    /// </summary>
    /// <param name="newData"></param>
    public void AddClassifyButton(ClassifyButtonManagerData newData)
    {
        var list = new List<ClassifyButtonManagerData>(classifyButtonManagerDatas);
        int insertIndex = newData.siblingIndex;
        if (insertIndex >= 0 && insertIndex <= list.Count)
        {
            list.Insert(insertIndex, newData);
        }
        else
        {
            list.Add(newData);
        }

        classifyButtonManagerDatas = list.ToArray();
        RecalculateSiblingIndexes(classifyButtonManagerDatas);  // 重新计算 siblingIndex
        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    /// <summary>
    /// 删除分类按钮
    /// </summary>
    /// <param name="siblingIndex"></param>
    public void RemoveClassifyButton(int siblingIndex)
    {
        classifyButtonManagerDatas = Array.FindAll(classifyButtonManagerDatas, cb => cb.siblingIndex != siblingIndex);

        // 更新所有待办事项的分类数据，移除这个分类 siblingIndex
        foreach (var todoManager in todoManagerDatas)
        {
            if (!string.IsNullOrEmpty(todoManager.customizePath))
            {
                todoManager.customizePath = todoManager.customizePath.Replace(siblingIndex.ToString(), string.Empty);
            }
        }

        SaveDataToJson(); // 保存到JSON文件
        ApplyLayout(); // 重新计算并应用布局
    }

    // 重新计算并应用布局
    public void ApplyLayout()
    {
        // 对分类按钮和待办事项进行布局更新
        classifyButtonContainer.GetComponent<CustomVerticalLayoutGroup>().RecalculateTotalHeight();
    }

    // 重新计算 siblingIndex
    private void RecalculateSiblingIndexes(ClassifyButtonManagerData[] buttonsData)
    {
        for (int i = 0; i < buttonsData.Length; i++)
        {
            buttonsData[i].siblingIndex = i;
        }
    }

    /// <summary>
    /// 生成一个随机的唯一 ID，并确保没有重复
    /// </summary>
    /// <returns></returns>
    public string GenerateUniqueId()
    {
        string uniqueId = GenerateRandomId();  // 初始生成一个 ID

        // 检测是否存在重复的 ID
        while (IsTodoIdDuplicate(uniqueId))
        {
            // 如果 ID 重复，则重新生成
            uniqueId = GenerateRandomId();
        }

        return uniqueId;
    }

    /// <summary>
    /// 生成一个随机的 ID（时间戳 + 随机数）
    private string GenerateRandomId()
    {
        // 使用当前时间戳和随机数生成唯一 ID
        string timestamp = DateTime.Now.Ticks.ToString();  // 获取当前时间戳
        string randomSuffix = UnityEngine.Random.Range(1000, 9999).ToString();  // 生成随机数字后缀

        // 合并时间戳和随机后缀，确保生成的 ID 唯一
        return timestamp + randomSuffix;
    }

    /// <summary>
    /// 检测指定的 ID 是否已经存在于待办事项数据中
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool IsTodoIdDuplicate(string id)
    {
        // 检查 todoManagerDatas 中是否有重复的 id
        return Array.Exists(todoManagerDatas, td => td.todoID == id);
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
}