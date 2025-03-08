using UnityEngine;

public class TodoWindowManager : MonoBehaviour, ISaveManger
{
    public static TodoWindowManager Instance;
    private CanvasGroup canvasGroup;

    private Vector3 todoPosition;

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

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void CloseWindow()
    {
        PanleWindowManager.ClosePanle(canvasGroup);
        //WindowTransparent.Instance.SetWindowTransparency(false);
    }

    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
        //WindowTransparent.Instance.SetWindowTransparency(true);
    }

    public void ResetPosition()
    {
        GetComponent<RectTransform>().position = todoPosition;
    }

    public void LoadData(GameData _data)
    {
        if (_data.currentTodoPosition == Vector3.zero)
        {
            _data.currentTodoPosition = GetComponent<RectTransform>().position;
        }
        if (_data.todoPosition == Vector3.zero)
        {
            _data.todoPosition = GetComponent<RectTransform>().position;
        }
        GetComponent<RectTransform>().position = _data.currentTodoPosition;
        todoPosition = _data.todoPosition;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currentTodoPosition = GetComponent<RectTransform>().position;
    }
}