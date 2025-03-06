using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanleManager : MonoBehaviour
{
    public static HelpPanleManager Instance;
    private CanvasGroup canvasGroup;

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
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void CloseWindow()
    {
        PanleWindowManager.ClosePanle(canvasGroup);
        MiniPanleManager.Instance.OpenWindow();
        TodoWindowManager.Instance.OpenWindow();
    }

    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
    }
}