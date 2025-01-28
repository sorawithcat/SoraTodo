using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemorandumManager : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public TMP_InputField inputField;
    public Button toggleButton;

    private bool isEditing = false;

    private void Start()
    {
        // 初始化状态，默认显示状态
        SetDisplayMode();

        // 设置按钮点击事件
        toggleButton.onClick.AddListener(ToggleState);
    }

    // 切换状态的函数
    private void ToggleState()
    {
        // 切换状态
        isEditing = !isEditing;
        SetDisplayMode();
    }

    // 设置当前状态：编辑模式或显示模式
    private void SetDisplayMode()
    {
        if (isEditing)
        {
            //编辑模式
            displayText.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);
            inputField.text = displayText.text;
            toggleButton.GetComponentInChildren<TMP_Text>().text = "完成";
        }
        else
        {
            //显示模式
            inputField.gameObject.SetActive(false);
            displayText.gameObject.SetActive(true);
            if (inputField.text == "")
            {
                displayText.text = "记录此刻";
            }
            else
            {
                displayText.text = inputField.text;
            }
            toggleButton.GetComponentInChildren<TMP_Text>().text = "编辑";
        }
    }
}
