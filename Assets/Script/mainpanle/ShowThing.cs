using System.Collections.Generic;
using UnityEngine;

public class ShowThing : MonoBehaviour
{
    public static ShowThing Instance;
    [SerializeField] private List<GameObject> showThings = new();
    private int currentShow = 0;
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
        showThings[currentShow].SetActive(true);
    }

    public void Show(int index)
    {
        showThings[currentShow].SetActive(false);
        currentShow = index;
        showThings[currentShow].SetActive(true);
        if (currentShow == 0)
        {
            showThings[currentShow].GetComponentInChildren<MainLineChartManager>().chart.AnimationReset();
            showThings[currentShow].GetComponentInChildren<MainLineChartManager>().chart.AnimationFadeIn();
        }
    }
}
