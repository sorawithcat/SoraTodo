using UnityEngine;

public class MiniPanleManager : MonoBehaviour
{
    public static MiniPanleManager Instance;
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
        MainPanle.Instance.OpenWindow();
        SetAlarm.Instance.CloseWindow();
        SetClearFX.Instance.CloseWindow();
        SetColor.Instance.CloseWindow();
        SetText.Instance.CloseWindow();
        TodoWindowManager.Instance.CloseWindow();
    }

    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
    }

    public void OpenHelpPanle()
    {
        HelpPanleManager.Instance.OpenWindow();
        CloseWindow();
        MainPanle.Instance.CloseWindow();
        TodoWindowManager.Instance.CloseWindow();
    }

    public void Exit()
    {
        SaveManager.Instance.ExitGame();
    }
}