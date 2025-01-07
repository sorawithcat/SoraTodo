using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileItem : MonoBehaviour
{
    public TextMeshProUGUI fileNameText;  // 文件名显示
    public Image backgroundImage;  // 文件项背景图（用于改变颜色）
    [HideInInspector] public string filePath;
    private bool isDirectory;
    private Color originalColor; // 用来保存文件项的原始颜色
    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_TIME = 0.3f;  // 双击时间间隔

    // 设置文件项的信息，并传递单击和双击的回调方法
    public void Setup(string fileName, bool isDirectory, string path, System.Action<string, bool> onSingleClick, System.Action<string, bool> onDoubleClick)
    {
        fileNameText.text = isDirectory ? $"{fileName}/" : $"{fileName}";
        this.isDirectory = isDirectory;
        this.filePath = path;

        // 保存原始颜色
        originalColor = backgroundImage.color;

        // 添加单击和双击事件
        Button itemButton = GetComponentInChildren<Button>();
        itemButton.onClick.AddListener(() => OnItemClick(onSingleClick, onDoubleClick));
    }

    // 处理单击与双击事件
    void OnItemClick(System.Action<string, bool> onSingleClick, System.Action<string, bool> onDoubleClick)
    {
        float currentTime = Time.time;
        if (currentTime - lastClickTime <= DOUBLE_CLICK_TIME)
        {
            // 双击事件
            onDoubleClick?.Invoke(filePath, isDirectory);
        }
        else
        {
            // 单击事件
            onSingleClick?.Invoke(filePath, isDirectory);
            // 改变背景颜色为红色
            backgroundImage.color = Color.red;
        }
        lastClickTime = currentTime;
    }

    // 恢复原始颜色
    public void Deselect()
    {
        backgroundImage.color = originalColor;
    }
}
