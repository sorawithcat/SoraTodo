using System;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MainLineChartManager : MonoBehaviour, ISaveManger
{
    public static MainLineChartManager Instance;
    public LineChart chart;

    private List<double> nearClearTodoNumbs = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetChartData(nearClearTodoNumbs);
    }

    /// <summary>
    /// 设置折线数据
    /// </summary>
    /// <param name="data"></param>
    public void SetChartData(List<double> data)
    {
        chart.ClearData();
        while (data.Count < 7 && data.Count >= 0)
        {
            data.Add(0);
        }

        for (int i = 0; i < data.Count; i++)
        {
            chart.AddXAxisData(i.ToString());
            chart.AddData(0, data[i]);
        }

        chart.RefreshChart();
    }

    public void LoadData(GameData _data)
    {
        nearClearTodoNumbs = _data.nearClearTodoNumbs;
        if (DateTime.Now.Date != new DateTime(_data.lastLeaveTime).Date)
        {
            MoveData();
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.nearClearTodoNumbs = nearClearTodoNumbs;
    }

    [ContextMenu("MoveData")]
    private void MoveData()
    {
        for (int i = 0; i < nearClearTodoNumbs.Count - 1; i++)
        {
            nearClearTodoNumbs[i] = nearClearTodoNumbs[i + 1];
        }
        nearClearTodoNumbs[nearClearTodoNumbs.Count - 1] = 0;
    }

    public void Append()
    {
        nearClearTodoNumbs[nearClearTodoNumbs.Count - 1]++;
        SetChartData(nearClearTodoNumbs);
    }
}