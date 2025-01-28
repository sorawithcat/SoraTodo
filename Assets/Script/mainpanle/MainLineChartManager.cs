using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MainLineChartManager : MonoBehaviour
{
    public LineChart chart;

    /// <summary>
    /// 设置折线数据
    /// </summary>
    /// <param name="data"></param>
    public void SetChartData(List<double> data)
    {
        chart.ClearData();
        if (data.Count < 7)
        {
            for (int i = 0; i <= 7 - data.Count; i++)
            {
                data.Add(0);
            }
        }
        for (int i = 0; i < data.Count; i++)
        {
            chart.AddXAxisData("x" + i);
            chart.AddData(0, data[i]);
        }

        chart.RefreshChart();
    }
}
