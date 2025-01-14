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



}
