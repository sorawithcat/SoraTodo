using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetClearFX : MonoBehaviour
{
    public static SetClearFX Instance;
    private Animator animator;
    [HideInInspector] public List<Transform> setTransforms;

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

    }
    void Update()
    {

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
