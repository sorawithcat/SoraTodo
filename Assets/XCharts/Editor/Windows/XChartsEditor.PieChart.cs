using UnityEditor;
using XCharts.Runtime;

namespace XCharts.Editor
{
    public partial class XChartsEditor
    {
        [MenuItem("XCharts/PieChart/Pie", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Pie", priority = 46)]
        public static void AddPieChart()
        {
            AddChart<PieChart>("PieChart");
        }

        [MenuItem("XCharts/PieChart/Pie With Label", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Pie With Label", priority = 46)]
        public static void AddPieChart_WithLabel()
        {
            var chart = AddChart<PieChart>("PieChart");
            chart.DefaultLabelPieChart();
        }

        [MenuItem("XCharts/PieChart/Donut", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Donut", priority = 46)]
        public static void AddPieChart_Donut()
        {
            var chart = AddChart<PieChart>("PieChart");
            chart.DefaultDonutPieChart();
        }

        [MenuItem("XCharts/PieChart/Donut With Label", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Donut With Label", priority = 46)]
        public static void AddPieChart_DonutWithLabel()
        {
            var chart = AddChart<PieChart>("PieChart");
            chart.DefaultLabelDonutPieChart();
        }

        [MenuItem("XCharts/PieChart/Radius Rose", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Radius Rose", priority = 46)]
        public static void AddPieChart_RadiusRose()
        {
            var chart = AddChart<PieChart>("PieChart");
            chart.DefaultRadiusRosePieChart();
        }

        [MenuItem("XCharts/PieChart/Area Rose", priority = 46)]
        [MenuItem("GameObject/UI/XCharts/PieChart/Area Rose", priority = 46)]
        public static void AddPieChart_AreaRose()
        {
            var chart = AddChart<PieChart>("PieChart");
            chart.DefaultAreaRosePieChart();
        }
    }
}