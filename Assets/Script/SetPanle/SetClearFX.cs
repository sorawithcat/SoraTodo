using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetClearFX : MonoBehaviour
{
    public static SetClearFX Instance;
    private Animator animator;
    [HideInInspector] public List<Transform> setTransforms;

    [Header("完成效果的下拉框")]
    [SerializeField] private TMP_Dropdown clearTypeDropdown;
    [Header("演示用todo")]
    [SerializeField] private TodoManager todoManager;
    [Header("演示用todo父节点")]
    [SerializeField] private GameObject todoManagerFather;
    [Header("Todo预制体")]
    [SerializeField] private GameObject todoPrefab;
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
        animator = GetComponent<Animator>();
        clearTypeDropdown.onValueChanged.AddListener(OnclearTypeDropdownValueChanged);
        todoManager.isTodo = true;
        todoManager.SetClearFX();

    }
    private void OnclearTypeDropdownValueChanged(int index)
    {
        if (todoManager != null)
        {
            todoManager.clearFX = (ClearFX)index;
            todoManager.SetClearFX();
        }
        else
        {
            GameObject newTodo = Instantiate(todoPrefab, todoManagerFather.transform);
            todoManager = newTodo.GetComponent<TodoManager>();
            todoManager.isTodo = true;
            todoManager.SetClearFX();
        }
    }
    public void SetClearFXs()
    {
        foreach (Transform transform in setTransforms)
        {
            TodoManager _todoManager = transform.GetComponent<TodoManager>();
            _todoManager.clearFX = (ClearFX)clearTypeDropdown.value;
            if (_todoManager.isTodo)
            {
                _todoManager.SetClearFX();
            }
        }
        CloseWindow();
    }

    public void CloseWindow()
    {
        animator.SetBool("IsClose", true);
        TodoWindowManager.Instance.OpenWindow();

    }
    public void OpenWindow()
    {
        animator.SetBool("IsClose", false);
    }
}
