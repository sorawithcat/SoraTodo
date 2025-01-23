using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipWindowManager : MonoBehaviour
{
    public static TipWindowManager Instance;

    [SerializeField] private GameObject tipWindowPrefab;  // 提示框预制体
    [SerializeField] private GameObject tipWindowFather;  // 父容器，用于显示提示框

    private Color defaultTextColor = Color.black;  // 默认文本颜色
    private readonly Queue<GameObject> tipWindowPool = new();  // 对象池

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

    /// <summary>
    /// 从对象池获取一个提示框对象
    /// </summary>
    private GameObject GetTipWindow()
    {
        if (tipWindowPool.Count > 0)
        {
            return tipWindowPool.Dequeue();
        }
        else
        {
            // 如果池中没有对象，则实例化一个新的
            return Instantiate(tipWindowPrefab);
        }
    }

    /// <summary>
    /// 将提示框对象返回池中，并重置状态
    /// </summary>
    private void ReturnTipWindow(GameObject tipWindow)
    {
        ResetTipWindow(tipWindow);
        tipWindow.SetActive(false);
        tipWindowPool.Enqueue(tipWindow);
    }

    /// <summary>
    /// 重置提示框状态
    /// </summary>
    private void ResetTipWindow(GameObject tipWindow)
    {
        var textComponents = tipWindow.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textComponent in textComponents)
        {
            textComponent.text = string.Empty;
            textComponent.color = Color.black;
        }

        if (tipWindow.TryGetComponent<Button>(out var button))
        {
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 设置提示信息并显示窗口
    /// </summary>
    /// <param name="message">提示信息,默认文本 "这是一个提示框"</param>
    /// <param name="textColor">提示颜色，默认黑色</param>
    /// <param name="autoCV">自动复制 ？（复制文本为空 ？ 显示时复制提示信息 ：显示时复制复制文本） ： null</param>
    /// <param name="cvString">复制文本 = 复制文本 == "" ? 提示信息 ：复制文本</param>
    /// <param name="canCV">点击复制？[点击之后=>（复制文本为空 ？ 复制提示信息 ：复制复制文本）=>关闭]：关闭</param>
    /// <param name="isForever">永远存在？点击关闭：延迟关闭</param>
    /// <param name="closeTime">关闭时间（秒），默认为2f</param>
    /// <param name="changeMessage">是否更改原本提示信息。如果设置了永久存在，则提示用户操作。true时更改信息前缀进行提醒。false则生成提示框提醒，不改变原提示信息</param>
    /// <param name="addToErrorFile">如果字体为红色，会将信息写入报错文件</param>
    public void ShowTip(string message = "这是一个提示框", Color? textColor = null, bool autoCV = false, string cvString = "", bool canCV = false, bool isForever = false, float closeTime = 2f, bool changeMessage = true, bool addToErrorFile = true)
    {
        if (addToErrorFile && textColor == Color.red)
        {
            StackTrace stackTrace = new(true);

            // 获取当前调用堆栈的第一个调用函数
            StackFrame frame = stackTrace.GetFrame(1);

            if (frame != null)
            {
                // 获取文件路径
                string filePath = frame.GetFileName();

                // 获取文件名
                string scriptName = Path.GetFileNameWithoutExtension(filePath);
                ErrorLogger.Instance.HandleLog("提示/报错文件：" + scriptName + "\nInfo：\n" + message + "\n以下是该提示/报错提供的建议复制文字：\n" + cvString + "\n以上是该提示/报错提供的建议复制文字：\n", $"\n{frame}", LogType.Error);
            }
            else
            {
                ErrorLogger.Instance.HandleLog("提示/报错文件：无法获取文件名\nInfo：\n" + message + "\n以下是该提示/报错提供的建议复制文字：\n" + cvString + "\n以上是该提示/报错提供的建议复制文字：\n", $"\n{frame}", LogType.Error);
            }
        }
        // 从对象池获取提示窗口对象
        GameObject newTip = GetTipWindow();
        newTip.SetActive(true);
        newTip.transform.SetParent(tipWindowFather.transform, false);

        TextMeshProUGUI tipText = newTip.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tipTexts = newTip.GetComponentsInChildren<TextMeshProUGUI>()[1];
        Button button = newTip.GetComponent<Button>();

        // 清空按钮的点击事件
        button.onClick.RemoveAllListeners();

        if (autoCV)
        {
            TextEditor te = new()
            {
                text = cvString == "" ? message : cvString
            };
            te.SelectAll();
            te.Copy();
            ShowTip("已复制文本：" + te.text);
        }

        if (!isForever)
        {
            // 设置延迟自动关闭
            StartCoroutine(HideTipAfterDelay(newTip.transform, closeTime));
        }
        else
        {
            if (!canCV)
            {
                // 点击后关闭
                button.onClick.AddListener(() => ClickToClose(newTip.transform));
                if (changeMessage)
                {
                    message = "点击关闭以下弹窗文本：" + message;
                }
                else
                {
                    ShowTip("点击关闭以下弹窗文本：" + message);
                }
            }
            else
            {
                // 点击后关闭并复制
                button.onClick.AddListener(() => ClickToClose(newTip.transform, cvString == "" ? message : cvString));
                if (changeMessage)
                {
                    message = "点击关闭并复制以下弹窗文本：" + message;
                }
                else
                {
                    ShowTip("点击关闭并复制以下弹窗文本：" + message);
                }
            }
        }

        // 设置文本和颜色
        tipText.text = message;
        tipTexts.text = message;
        tipText.color = textColor ?? defaultTextColor;
        tipTexts.color = textColor ?? defaultTextColor;

        // 强制刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(tipWindowFather.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 延迟关闭提示框
    /// </summary>
    private IEnumerator HideTipAfterDelay(Transform _newTip, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnTipWindow(_newTip.gameObject);
    }

    /// <summary>
    /// 点击关闭提示框，并且复制文本（如果需要）
    /// </summary>
    public void ClickToClose(Transform _Tip, string _cvString = "")
    {
        if (_cvString != "")
        {
            TextEditor te = new()
            {
                text = _cvString
            };
            te.SelectAll();
            te.Copy();
            ShowTip("已复制文本：" + te.text);
        }
        ReturnTipWindow(_Tip.gameObject);
    }
}
