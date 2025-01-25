using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AddClassifyButtonManager : MonoBehaviour
{
    public static AddClassifyButtonManager Instance;
    private CanvasGroup canvasGroup;
    [HideInInspector] public Transform setTransform;

    [Header("分类按钮父节点")]
    [SerializeField] private GameObject classifyButtonFather;
    [Header("分类按钮预制体")]
    [SerializeField] private GameObject classifyButtonPrefab;
    [Header("标题")]
    [SerializeField] private TMP_InputField title;
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
    public void AddClassIfyButton()
    {
        GameObject newClass = Instantiate(classifyButtonPrefab, classifyButtonFather.transform);
        int thisSibling = setTransform.transform.GetSiblingIndex() + 1;
        newClass.transform.SetSiblingIndex(thisSibling);
        ClassifyButtonManager classifyButtonManager = newClass.GetComponent<ClassifyButtonManager>();
        classifyButtonManager.buttonsList = classifyButtonFather;
        classifyButtonManager.todoList = newClass.GetComponentInChildren<TodoList>().gameObject;
        List<ClassifyButtonManager> classifyButtons = classifyButtonFather.GetComponentsInChildren<ClassifyButtonManager>().ToList();
        for (int i = 0; i < classifyButtons.Count; i++)
        {
            classifyButtons[i].siblingIndex = i;
            classifyButtons[i].thisID = i;
        }
        TextMeshProUGUI classText = newClass.GetComponentInChildren<TextMeshProUGUI>();
        classText.text = title.text;
        ClassifyButtonManagerData classifyButtonManagerData = new()
        {
            siblingIndex = thisSibling,
            title = title.text,
            titleColor = "#000",
            titleBGColor = "#FFFFFF",
            todos = new string[] { }
        };
        classifyButtonFather.GetComponent<CustomVerticalLayoutGroup>().AddChild(newClass.GetComponent<RectTransform>());
        LoadAllData.Instance.AddClassifyButton(classifyButtonManagerData);
        TimerManager.Instance.classifyToTodoManagers.Add(classifyButtonManager, new());
        CloseWindow();
    }
    public void CloseWindow()
    {
        PanleWindowManager.ClosePanle(canvasGroup);
        TodoWindowManager.Instance.OpenWindow();
    }

    public void OpenWindow()
    {
        PanleWindowManager.OpenPanle(canvasGroup);
    }
}
