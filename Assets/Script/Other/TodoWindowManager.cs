using UnityEngine;

public class TodoWindowManager : MonoBehaviour
{
    public static TodoWindowManager Instance;
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
    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

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
