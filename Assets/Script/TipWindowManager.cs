using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEditor.VersionControl;
using UnityEditor;

public class TipWindowManager : MonoBehaviour
{
    public static TipWindowManager Instance;
    [SerializeField] private GameObject tipWindow;
    [SerializeField] private GameObject tipWindowFather;

    private Color defaultTextColor = Color.black;
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
    public void ShowTip(string message = "这是一个提示框", Color? textColor = null, bool autoCV = false, string cvString = "", bool canCV = false, bool isForever = false, float closeTime = 2f, bool changeMessage = true)
    {
        GameObject newTip = Instantiate(tipWindow, tipWindowFather.transform);
        TextMeshProUGUI tipText = newTip.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tipTexts = newTip.GetComponentsInChildren<TextMeshProUGUI>()[1];
        newTip.GetComponent<Button>().onClick.RemoveAllListeners();
        if (autoCV)
        {
            TextEditor te = new TextEditor();
            te.text = cvString == "" ? message : cvString;
            te.SelectAll();
            te.Copy();
            ShowTip("已复制文本：" + te.text);
        }
        if (!isForever)
        {
            StartCoroutine(HideTipAfterDelay(newTip.transform, closeTime));
        }
        else
        {
            if (!canCV)
            {
                newTip.GetComponent<Button>().onClick.AddListener(() => ClickToClose(newTip.transform));
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

                newTip.GetComponent<Button>().onClick.AddListener(() => ClickToClose(newTip.transform, cvString == "" ? message : cvString));
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
        tipText.text = message;
        tipTexts.text = message;
        tipText.color = textColor ?? defaultTextColor;
        tipTexts.color = textColor ?? defaultTextColor;
    }

    // 隐藏提示窗口
    private IEnumerator HideTipAfterDelay(Transform _newTip, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(_newTip.gameObject);
    }

    public void ClickToClose(Transform _Tip, string _cvString = "")
    {
        if (_cvString != "")
        {
            TextEditor te = new TextEditor();
            te.text = _cvString;
            te.SelectAll();
            te.Copy();
            ShowTip("已复制文本：" + te.text);
        }
        Destroy(_Tip.gameObject);
    }
}
