using System.Collections;
using UnityEngine;
using TMPro;

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
    /// <param name="message"></param>
    /// <param name="textColor"></param>
    public void ShowTip(string message, Color? textColor = null)
    {
        GameObject newTip = Instantiate(tipWindow, tipWindowFather.transform);
        TextMeshProUGUI tipText = newTip.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tipTexts = newTip.GetComponentsInChildren<TextMeshProUGUI>()[1];

        tipText.text = message;
        tipTexts.text = message;
        tipText.color = textColor ?? defaultTextColor;
        tipTexts.color = textColor ?? defaultTextColor;
        StartCoroutine(HideTipAfterDelay(newTip.transform, 2f));
    }

    // 隐藏提示窗口
    private IEnumerator HideTipAfterDelay(Transform _newTip,float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(_newTip.gameObject);
    }
}
