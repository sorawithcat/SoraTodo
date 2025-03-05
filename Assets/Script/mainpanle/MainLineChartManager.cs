using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MainLineChartManager : MonoBehaviour
{
    public LineChart chart;

    private void Start()
    {
        //Todo:之后记得改成从save里获取
        SetChartData(new List<double>() { });
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
}