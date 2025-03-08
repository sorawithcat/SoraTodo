using UnityEngine;

public class MiniPanleManager : MonoBehaviour, ISaveManger
{
    public static MiniPanleManager Instance;
    private CanvasGroup canvasGroup;

    public bool miniIsDrag = false;
    public bool autoClose = true;

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

    public void LoadData(GameData _data)
    {
        autoClose = _data.autoClose;
        miniIsDrag = _data.miniIsDrag;
        GetComponent<DraggableObject>().notThis = miniIsDrag;
    }

    public void SaveData(ref GameData _data)
    {
        _data.autoClose = autoClose;
        _data.miniIsDrag = miniIsDrag;
        if (autoClose) _data.miniIsDrag = false;
    }
}