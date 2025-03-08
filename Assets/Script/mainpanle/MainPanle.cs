using UnityEngine;

public class MainPanle : MonoBehaviour
{
    public static MainPanle Instance;
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
        TodoWindowManager.Instance.OpenWindow();
        MiniPanleManager.Instance.OpenWindow();
    }

    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
        NearTodoManager.Instance.UpdateThings();
    }
}