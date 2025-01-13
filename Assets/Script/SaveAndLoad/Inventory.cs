using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Inventory : MonoBehaviour, ISaveManger
{

    public static Inventory Instance;



    [Header("数据库")]
    public List<ThemeColor> themeDataBase;
    public SerializableDictionary<string, TodoManager> loadedTodds = new SerializableDictionary<string, TodoManager>();
    public SerializableDictionary<string, ClassifyButtonManager> loadedClassifyButtons = new SerializableDictionary<string, ClassifyButtonManager>();
    /// <summary>
    /// 分类的所属待办guid
    /// </summary>
    public SerializableDictionary<string, List<string>> classifyButtonSons = new SerializableDictionary<string, List<string>>();
    /// <summary>
    /// 分类的guid
    /// </summary>
    public List<string> classifyButtonGuid = new List<string>();
    /// <summary>
    /// 分类数据
    /// </summary>
    public List<ClassifyButtonManagerData> classifyButtonManagerDataList = new List<ClassifyButtonManagerData>();
    /// <summary>
    /// 待办数据
    /// </summary>
    public List<TodoManagerData> todoManagerDataList = new List<TodoManagerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FillUpItemDataBase();

    }

    /// <summary>
    /// 返回分类的所属待办guid
    /// </summary>
    public SerializableDictionary<string, List<string>> GetClassifyButtonSons() => classifyButtonSons;
    /// <summary>
    /// 返回所有待办
    /// </summary>
    /// <returns></returns>
    public SerializableDictionary<string, TodoManager> GetAllTodo() => loadedTodds;
    /// <summary>
    /// 返回所有分类
    /// </summary>
    /// <returns></returns>
    public SerializableDictionary<string, ClassifyButtonManager> GetAllClassifyButton() => loadedClassifyButtons;
    public void LoadData(GameData _data)
    {
        loadedTodds = new SerializableDictionary<string, TodoManager>();
        loadedClassifyButtons = new SerializableDictionary<string, ClassifyButtonManager>();
        //获取分类的guid
        foreach (KeyValuePair<ClassifyButtonManagerData, ClassifyButtonManager> pair in _data.classifyButtons)
        {
            //获取分类的信息
            loadedClassifyButtons.Add(pair.Key.classifyButtonManagerDataGuid, pair.Value);
            if (!classifyButtonGuid.Contains(pair.Key.classifyButtonManagerDataGuid))
            {
                classifyButtonGuid.Add(pair.Key.classifyButtonManagerDataGuid);
            }
        }
        //获取每个分类下的待办
        foreach (KeyValuePair<TodoManagerData, TodoManager> pair in _data.todos)
        {
            //获取todo的信息
            loadedTodds.Add(pair.Key.todoGuid, pair.Value);
            if (classifyButtonGuid.Contains(pair.Value.fatherClassifyButtonGuid))
            {
                classifyButtonSons[pair.Value.fatherClassifyButtonGuid].Add(pair.Key.todoGuid);
            }
            else
            {
                classifyButtonSons.Add(pair.Value.fatherClassifyButtonGuid, new List<string> { pair.Key.todoGuid });
            }


        }
    }

    public void SaveData(ref GameData _data)
    {
        //_data.todos.Clear();
        //_data.classifyButtons.Clear();

        //foreach (KeyValuePair<string, TodoManager> pair in loadedTodds)
        //{
        //    _data.todos.Add(pair.Key, pair.Value);
        //}
        //foreach (KeyValuePair<string, ClassifyButtonManager> pair in loadedClassifyButtons)
        //{
        //    _data.classifyButtons.Add(pair.Key, pair.Value);
        //}

    }

    [ContextMenu("填充项目数据库")]
    private void FillUpItemDataBase()
    {
        themeDataBase = new List<ThemeColor>(GetThemeDataBase());
        classifyButtonManagerDataList = new List<ClassifyButtonManagerData>(GetAllClassifyButtonData());
        todoManagerDataList = new List<TodoManagerData>(GetAllTodoManagerData());
    }


    // 加载所有主题颜色数据
    public List<ThemeColor> GetThemeDataBase()
    {
        List<ThemeColor> itemDataBase = new List<ThemeColor>();
        ThemeColor[] themeColors = Resources.LoadAll<ThemeColor>("Themes");

        foreach (var themeColor in themeColors)
        {
            itemDataBase.Add(themeColor);
        }

        return itemDataBase;
    }

    // 获取所有分类数据
    public List<ClassifyButtonManagerData> GetAllClassifyButtonData()
    {
        List<ClassifyButtonManagerData> itemDataBase = new List<ClassifyButtonManagerData>();
        ClassifyButtonManagerData[] classifyButtonData = Resources.LoadAll<ClassifyButtonManagerData>("ClassifyButtonManagerData");

        foreach (var data in classifyButtonData)
        {
            itemDataBase.Add(data);
        }

        return itemDataBase;
    }

    // 获取所有待办事项数据
    public List<TodoManagerData> GetAllTodoManagerData()
    {
        List<TodoManagerData> itemDataBase = new List<TodoManagerData>();
        TodoManagerData[] todoManagerData = Resources.LoadAll<TodoManagerData>("TodoManagerData");

        foreach (var data in todoManagerData)
        {
            itemDataBase.Add(data);
        }

        return itemDataBase;
    }
}
