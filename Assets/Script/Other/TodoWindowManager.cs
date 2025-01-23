using UnityEngine;

public class TodoWindowManager : MonoBehaviour
{
    public static TodoWindowManager Instance;
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
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void CloseWindow()
    {
        PanleWindowManager.ClosePanle(canvasGroup);

    }
    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);

    }
}
