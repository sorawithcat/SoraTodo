using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CloseTimeType
{
    OneHour,
    TwoHour,
    ThreeHour,
    FiveHour,
    OneDay
}
public class NearTodoManager : MonoBehaviour
{
    public static NearTodoManager Instance;
    [Header("接近的事容器")]
    public GameObject closeThingsFather;
    [Header("接近的事预制件")]
    public GameObject closeThingsPrefab;
    [Header("更新下拉框")]
    public TMP_Dropdown updateDropDown;

    private readonly List<string> closeThings = new();

    private List<long> nearTodoTime = new();
    private List<string> nearTodo = new();
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
        updateDropDown.onValueChanged.AddListener(OnDropdownValueChange);
        Invoke(nameof(UpdateThings), 0.5f);
    }
    public void AddCloseThings(List<string> _newThings)
    {
        foreach (var title in _newThings)
        {
            closeThings.Add(title);
        }
        CreatNearThing();
    }

    public void UpdateCloseThings(List<string> _newThings)
    {
        closeThings.Clear();
        foreach (Transform child in closeThingsFather.transform)
        {
            Destroy(child.gameObject);
        }
        closeThings.AddRange(_newThings);
        CreatNearThing();
    }

    private void CreatNearThing()
    {
        if (closeThings.Count == 0)
        {
            Instantiate(closeThingsPrefab, closeThingsFather.transform);
        }
        else
        {
            for (int i = 0; i < closeThings.Count; i++)
            {
                GameObject newThing = Instantiate(closeThingsPrefab, closeThingsFather.transform);
                newThing.GetComponent<TextMeshProUGUI>().text = closeThings[i];
            }
        }
    }

    public void UpdateThings()
    {
        OnDropdownValueChange(updateDropDown.value);
    }
    private void OnDropdownValueChange(int index)
    {
        nearTodo.Clear();
        nearTodoTime.Clear();
        int closeTime = 0;
        switch ((CloseTimeType)index)
        {
            case CloseTimeType.OneHour:
                closeTime = 1;
                break;
            case CloseTimeType.TwoHour:
                closeTime = 2;
                break;
            case CloseTimeType.ThreeHour:
                closeTime = 3;
                break;
            case CloseTimeType.FiveHour:
                closeTime = 5;
                break;
            case CloseTimeType.OneDay:
                closeTime = 24;
                break;
        }
        foreach (List<TodoManager> todoManagers in TimerManager.Instance.classifyToTodoManagers.Values)
        {
            foreach (TodoManager todoManager in todoManagers)
            {
                todoManager.SaveAllInJson();
                if ((TimingType)todoManager.timingType == TimingType.Countdown)
                {
                    if (UnOverTimeByHour(DateTime.Now.AddSeconds(todoManager.timer), closeTime))
                    {
                        nearTodoTime.Add(DateTime.Now.AddSeconds(todoManager.timer).Ticks);
                        nearTodo.Add($"{DateTime.Now.AddSeconds(todoManager.timer)} ：{todoManager.todoText.text}");
                    }
                }
                else if ((TimingType)todoManager.timingType == TimingType.Date)
                {
                    if (UnOverTimeByHour(todoManager.dateTime, closeTime))
                    {
                        nearTodoTime.Add(todoManager.dateTime.Ticks);
                        nearTodo.Add($"{todoManager.dateTime} ：{todoManager.todoText.text}");
                    }
                }
            }
        }
        (nearTodoTime, nearTodo) = ListHelper.SortTwoLists(nearTodoTime, nearTodo);
        UpdateCloseThings(nearTodo);
    }

    public static class ListHelper
    {
        public static (List<T>, List<U>) SortTwoLists<T, U>(List<T> numbers, List<U> correspondingValues)
        {
            T[] numberArray = numbers.ToArray();
            U[] valueArray = correspondingValues.ToArray();

            Array.Sort(numberArray, valueArray);


            return (new List<T>(numberArray), new List<U>(valueArray));
        }
    }
    /// <summary>
    /// 判断是否未超时
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="hour"></param>
    /// <returns></returns>
    public bool UnOverTimeByHour(DateTime dateTime, int hour)
    {
        DateTime now = DateTime.Now;
        TimeSpan timeSpan = now - dateTime;
        return timeSpan.Duration().TotalHours <= hour;
    }
}
