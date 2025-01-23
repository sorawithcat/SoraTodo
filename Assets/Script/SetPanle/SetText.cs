using TMPro;
using UnityEngine;

public class SetText : MonoBehaviour
{
    public static SetText Instance;

    [HideInInspector] public Transform setTransform;
    [Header("旧的文字")]
    [SerializeField] private TextMeshProUGUI oldTMP;
    [Header("新的文字")]
    [SerializeField] private TMP_InputField newTMP;

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


    public void SetOldText(Transform _textTransform)
    {
        oldTMP.text = _textTransform.GetComponentInChildren<TextMeshProUGUI>().text;
    }
    public void SetTexts()
    {
        setTransform.GetComponentInChildren<TextMeshProUGUI>().text = newTMP.text;
        if (setTransform.GetComponent<ClassifyButtonManager>() != null)
        {
            LoadAllData.Instance.UpdateClassifyButton(setTransform.GetComponent<ClassifyButtonManager>().siblingIndex, "title", newTMP.text);
        }
        else if (setTransform.GetComponent<TodoManager>() != null)
        {
            LoadAllData.Instance.UpdateTodoManager(setTransform.GetComponent<TodoManager>().todoID, "title", newTMP.text);

        }
        else
        {
            TipWindowManager.Instance.ShowTip("无法更新该属性。");
        }
        CloseWindow();
    }
    public void CloseWindow()
    {
        PanleWindowManager.Instance.ClosePanle(canvasGroup);
        TodoWindowManager.Instance.OpenWindow();
    }
    public void OpenWindow()
    {
        PanleWindowManager.Instance.OpenPanle(canvasGroup);
    }
}
