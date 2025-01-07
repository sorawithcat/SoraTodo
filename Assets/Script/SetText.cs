using System.Collections;
using System.Collections.Generic;
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

    private Animator animator;

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
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOldText(Transform _textTransform)
    {
        oldTMP.text = _textTransform.GetComponentInChildren<TextMeshProUGUI>().text;
    }
    public void SetTexts()
    {
        setTransform.GetComponentInChildren<TextMeshProUGUI>().text = newTMP.text;
        CloseWindow();
    }
    public void CloseWindow()
    {
        animator.SetBool("IsClose", true);

    }
    public void OpenWindow()
    {
        animator.SetBool("IsClose", false);

    }
}
