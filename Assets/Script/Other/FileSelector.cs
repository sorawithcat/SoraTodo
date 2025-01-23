using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileSelector : MonoBehaviour
{
    public GameObject fileBrowserPanel; // 文件浏览器面板
    public Button selectButton; // 选择按钮
    public Button cancelButton; // 取消按钮
    public Button backButton; // 返回按钮
    public TMP_InputField pathText; // 可编辑的路径输入框（原 pathText）
    public Transform fileListParent; // 文件列表父节点
    public GameObject fileItemPrefab; // 文件项的预制体
    public string initialPath = "C:/"; // 初始路径

    [Header("文件类型过滤")]
    public TMP_Dropdown fileTypeDropdown; // 文件类型下拉框
    private string[] supportedFileTypes = { "*.txt", "*.png", "*.jpg", "*.json", "*.wav", "*.mp3", "*.mp4", "*.mkv" }; // 支持的文件类型

    // 用于显示自动补全建议的下拉框
    public GameObject suggestionPanel;
    public Transform suggestionListParent;
    public GameObject suggestionItemPrefab;

    public TextMeshProUGUI tip;

    private string currentPath;
    private bool isPathTextFocused = false; // 判断pathText是否被点击
    private FileItem currentlySelectedItem; // 当前选中的文件项

    void Start()
    {
        currentPath = initialPath;
        OpenFileBrowser(currentPath);

        selectButton.onClick.AddListener(OnSelectButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);  // 监听返回按钮

        // 设置文件类型过滤器
        fileTypeDropdown.onValueChanged.AddListener(OnFileTypeChanged);

        // 监听路径文本框的变化（现在pathText是可编辑的）
        pathText.onSelect.AddListener(OnPathTextSelected); // 点击路径文本框时显示建议
        pathText.onValueChanged.AddListener(OnPathInputChanged); // 输入路径时更新建议
    }

    // 当用户选择文件类型时更新文件浏览器
    void OnFileTypeChanged(int index)
    {
        // 获取选择的文件类型
        string selectedType = fileTypeDropdown.options[index].text;

        if (selectedType == "All Files")
        {
            supportedFileTypes = new string[] { "*.*" }; // 显示所有文件
        }
        else
        {
            supportedFileTypes = new string[] { "*" + selectedType }; // 根据选择的类型更新过滤器
        }

        // 重新加载当前文件夹，进行类型过滤
        OpenFileBrowser(currentPath);
    }

    // 打开文件浏览器
    void OpenFileBrowser(string path)
    {
        // 验证路径是否有效
        if (Directory.Exists(path))
        {
            currentPath = path;
            pathText.text = path;

            // 清除文件列表
            foreach (Transform child in fileListParent)
            {
                Destroy(child.gameObject);
            }

            try
            {
                // 获取文件夹和文件
                DirectoryInfo directoryInfo = new(path);
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                FileInfo[] files = directoryInfo.GetFiles();

                // 显示文件夹
                foreach (var dir in directories)
                {
                    CreateFileItem(dir.Name, true, dir.FullName, null);  // 传递null作为后缀
                }

                // 显示文件（根据选择的文件类型进行过滤）
                foreach (var file in files)
                {
                    if (IsValidFileType(file))
                    {
                        CreateFileItem(file.Name, false, file.FullName, file.Extension);  // 传递文件扩展名
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowWarning("无法访问该目录: " + path + " (" + ex.Message + ")");
            }
            catch (DirectoryNotFoundException)
            {
                ShowWarning("路径不存在！");
            }
            catch (Exception ex)
            {
                ShowWarning("无法访问路径: " + ex.Message);
            }
        }
        else
        {
            ShowWarning("该路径无效！");
        }
    }

    // 判断文件是否符合选择的文件类型
    bool IsValidFileType(FileInfo file)
    {
        string extension = file.Extension.ToLower();
        foreach (var filter in supportedFileTypes)
        {
            if (extension == filter.Substring(1).ToLower())  // 去掉 "*" 并比较
            {
                return true;
            }
        }
        return false;
    }

    // 创建文件项（文件或文件夹）
    void CreateFileItem(string name, bool isDirectory, string path, string fileExtension)
    {
        GameObject fileItem = Instantiate(fileItemPrefab, fileListParent);
        FileItem fileItemScript = fileItem.GetComponent<FileItem>();

        // 设置文件项
        fileItemScript.Setup(name, isDirectory, path, OnFileItemSingleClicked, OnFileItemDoubleClicked);
    }

    // 单击事件（选中）时触发
    void OnFileItemSingleClicked(string path, bool isDirectory)
    {
        if (currentlySelectedItem != null)
        {
            // 恢复之前选中项的颜色
            currentlySelectedItem.Deselect();
        }

        // 当前选中项
        currentlySelectedItem = GetFileItemByPath(path);
        if (currentlySelectedItem != null)
        {
            // 改变背景颜色为红色
            currentlySelectedItem.backgroundImage.color = Color.red;
        }
    }

    // 双击事件（打开文件夹或选中文件）时触发
    void OnFileItemDoubleClicked(string path, bool isDirectory)
    {
        if (isDirectory)
        {
            // 如果是文件夹，进入该文件夹
            OpenFileBrowser(path);
        }
        else
        {
            // 如果是文件，选中文件，相当于点击选择按钮
            currentPath = path;
            pathText.text = "Selected: " + currentPath;
            suggestionPanel.SetActive(false);

            // 触发选择按钮点击
            OnSelectButtonClicked();
        }
    }

    // 获取指定路径的 FileItem
    FileItem GetFileItemByPath(string path)
    {
        foreach (Transform child in fileListParent)
        {
            FileItem fileItem = child.GetComponent<FileItem>();
            if (fileItem != null && fileItem.filePath == path)
            {
                return fileItem;
            }
        }
        return null;
    }

    // 选择按钮的点击事件
    void OnSelectButtonClicked()
    {
        // 如果用户手动输入了路径，尝试打开该路径
        string manualPath = pathText.text;

        if (Directory.Exists(manualPath))
        {
            OpenFileBrowser(manualPath);
        }
        else
        {
            ShowWarning("无效的路径: " + manualPath);
        }

        fileBrowserPanel.SetActive(false);  // 关闭文件选择器
    }

    // 取消按钮的点击事件
    void OnCancelButtonClicked()
    {
        fileBrowserPanel.SetActive(false);  // 关闭文件选择器
    }

    // 返回按钮的点击事件
    void OnBackButtonClicked()
    {
        // 获取当前路径的父目录
        string parentDirectory = Path.GetDirectoryName(currentPath);

        if (parentDirectory != null)
        {
            // 返回上一级目录
            OpenFileBrowser(parentDirectory);
        }
        else
        {
            ShowWarning("已到达根目录，无法返回上一级。");
        }
    }

    // 当路径输入框的内容变化时，进行自动补全
    void OnPathInputChanged(string input)
    {
        if (string.IsNullOrEmpty(input) || !isPathTextFocused)
        {
            suggestionPanel.SetActive(false);
            return;
        }

        List<string> suggestions = GetPathSuggestions(input);

        // 清空建议列表
        foreach (Transform child in suggestionListParent)
        {
            Destroy(child.gameObject);
        }

        // 显示建议
        foreach (var suggestion in suggestions)
        {
            GameObject suggestionItem = Instantiate(suggestionItemPrefab, suggestionListParent);
            suggestionItem.GetComponentInChildren<TextMeshProUGUI>().text = suggestion;

            Button suggestionButton = suggestionItem.GetComponentInChildren<Button>();
            suggestionButton.onClick.AddListener(() => OnSuggestionItemClicked(suggestion));
        }

        // 显示/隐藏建议面板
        suggestionPanel.SetActive(suggestions.Count > 0);
    }

    // 获取路径建议（例如：输入路径的一部分，提供补全建议）
    List<string> GetPathSuggestions(string input)
    {
        List<string> suggestions = new();

        // 获取所有驱动器
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            if (drive.IsReady)
            {
                string fullPath = Path.Combine(drive.RootDirectory.FullName, input);

                if (Directory.Exists(fullPath))
                {
                    suggestions.Add(fullPath);
                }
            }
        }

        return suggestions.Distinct().ToList();
    }

    // 当选择了路径建议时，自动填充到输入框
    void OnSuggestionItemClicked(string suggestion)
    {
        pathText.text = suggestion;
        suggestionPanel.SetActive(false);

        OpenFileBrowser(suggestion);
    }

    // 路径文本框被点击时，显示路径建议
    void OnPathTextSelected(string text)
    {
        isPathTextFocused = true;  // 标记为路径文本框被点击
    }

    // 显示警告信息
    void ShowWarning(string message)
    {
        tip.text = message;
    }
}
