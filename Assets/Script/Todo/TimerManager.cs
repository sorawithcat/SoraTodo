using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    [SerializeField] private Transform todoFather;
    public SerializableDictionary<ClassifyButtonManager, List<TodoManager>> classifyToTodoManagers = new();
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
    private void Start()
    {
        Invoke(nameof(Init), 0.3f);
    }
    public void Init()
    {
        AddTodoManagersRecursively(todoFather);

    }


    /// <summary>
    /// 递归遍历所有子物体，检查 ClassifyButtonManager 和 TodoManager
    /// </summary>
    /// <param name="parent"></param>
    private void AddTodoManagersRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // 查找 ClassifyButtonManager 组件
            if (child.TryGetComponent<ClassifyButtonManager>(out var classifyButtonManager))
            {
                // 如果找到了 ClassifyButtonManager，初始化该分类的 TodoManager 列表
                if (!classifyToTodoManagers.ContainsKey(classifyButtonManager))
                {
                    classifyToTodoManagers[classifyButtonManager] = new List<TodoManager>();
                }

                // 查找子物体中的 TodoManager 并添加到列表中
                foreach (Transform grandChild in classifyButtonManager.transform)
                {
                    foreach (Transform todo in grandChild)
                    {
                        TodoManager todoManager = todo.GetComponentInChildren<TodoManager>();
                        if (todoManager != null)
                        {
                            classifyToTodoManagers[classifyButtonManager].Add(todoManager);
                            //todoManager.SetAlarm();
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        //更新所有TodoManager的定时器
        foreach (var classifyButtonManager in classifyToTodoManagers.Keys)
        {
            List<TodoManager> todoManagers = classifyToTodoManagers[classifyButtonManager];
            foreach (var todoManager in todoManagers)
            {
                if (todoManager != null)
                {
                    bool isUpdata = todoManager.UpdateTimer();
                    if (!isUpdata)
                    {
                        ClassifyButtonManager classifyButtonManage = GetKeyForTodoManager(todoManager);
                        if (classifyButtonManage != null && classifyButtonManage.isOpen == -1)
                        {
                            classifyButtonManage.ClickToHandoff();
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// 注册 TodoManager
    /// </summary>
    /// <param name="todoManager"></param>
    /// <param name="_classifyButtonManager"></param>
    public void RegisterTodoManager(TodoManager todoManager, ClassifyButtonManager _classifyButtonManager)
    {
        if (classifyToTodoManagers.ContainsKey(_classifyButtonManager))
        {
            if (classifyToTodoManagers[_classifyButtonManager].Contains(todoManager)) return;
            else
            {
                classifyToTodoManagers[_classifyButtonManager].Add(todoManager);
            }
        }
        else
        {
            classifyToTodoManagers.Add(_classifyButtonManager, new List<TodoManager> { todoManager });
        }
        todoManager.SetAlarm();
    }

    /// <summary>
    /// 注销 TodoManager
    /// </summary>
    /// <param name="todoManager"></param>
    public void UnregisterTodoManager(TodoManager todoManager)
    {
        foreach (var classifyButtonManager in classifyToTodoManagers.Keys)
        {
            if (classifyToTodoManagers[classifyButtonManager].Contains(todoManager))
            {
                classifyToTodoManagers[classifyButtonManager].Remove(todoManager);
            }
        }
    }

    /// <summary>
    /// 重置所有定时器
    /// </summary>
    public void ResetAllTimers()
    {
        foreach (var classifyButtonManager in classifyToTodoManagers.Keys)
        {
            List<TodoManager> todoManagers = classifyToTodoManagers[classifyButtonManager];
            foreach (var todoManager in todoManagers)
            {
                if (todoManager != null)
                {
                    todoManager.UpdateTimerSettings(TimingType.None, AlarmType.None); // 重置定时器为无定时状态
                }
            }
        }
    }
    /// <summary>
    /// 为新的todo设置属性
    /// </summary>
    /// <param name="_todoManager"></param>
    /// <param name="timingType"></param>
    /// <param name="newTime"></param>
    /// <param name="_newDate"></param>
    public void UpdateTodoTimerSetting(TodoManager _todoManager, TimingType timingType = TimingType.None, AlarmType alarmType = AlarmType.None, float newTime = 60f, DateTime? _newDate = null, string _customizePath = "")
    {
        if (!_newDate.HasValue)
        {
            _newDate = DateTime.Now.AddHours(1);
        }
        if (_todoManager != null)
        {
            _todoManager.UpdateTimerSettings(timingType, alarmType, newTime, _newDate, _customizePath);
            _todoManager.SetAlarm();
        }
    }

    public ClassifyButtonManager GetKeyForTodoManager(TodoManager targetTodoManager)
    {
        foreach (var classifyButtonManager in classifyToTodoManagers.Keys)
        {
            List<TodoManager> todoManagers = classifyToTodoManagers[classifyButtonManager];
            if (todoManagers.Contains(targetTodoManager))
            {
                return classifyButtonManager;
            }
        }

        return null;
    }
}
