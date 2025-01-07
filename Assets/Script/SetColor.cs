using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetColorType
{
    Red, Green, Blue, Alpha
}
public enum SetThingType
{
    Image,
    Text,
    GradientStart,
    GradientEnd
}
public class SetColor : MonoBehaviour
{
    public static SetColor Instance;
    [SerializeField] private TextMeshProUGUI redtextDisplay;
    [SerializeField] private TextMeshProUGUI GreentextDisplay;
    [SerializeField] private TextMeshProUGUI bluetextDisplay;
    [SerializeField] private TextMeshProUGUI alphatextDisplay;

    [SerializeField] private GameObject redBox;
    [SerializeField] private GameObject greenBox;
    [SerializeField] private GameObject blueBox;
    [SerializeField] private GameObject alphaBox;
    [SerializeField] private GameObject allBox;

    // 存储各个颜色的数值
    private float redValue = 0f;
    private float greenValue = 0f;
    private float blueValue = 0f;
    private float alphaValue = 1f; // 默认完全不透明


    private Animator animator;
    [HideInInspector] public List<Transform> setTransforms;
    [HideInInspector] public SetThingType setType;

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
        UpdateAllBoxes(); // 初始化设置颜色
        animator = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SetColors(redtextDisplay, SetColorType.Red);
        SetColors(GreentextDisplay, SetColorType.Green);
        SetColors(bluetextDisplay, SetColorType.Blue);
        SetColors(alphatextDisplay, SetColorType.Alpha);

        UpdateAllBoxes(); // 每帧更新 allBox 颜色
    }

    private void SetColors(TextMeshProUGUI _colorNumb, SetColorType _setColorType)
    {
        switch (_setColorType)
        {
            case SetColorType.Red:
                if (int.TryParse(_colorNumb.text, out int tempRed))
                {
                    redValue = Mathf.Clamp(tempRed / 255f, 0f, 1f); // 将值归一化并限定在0-1之间
                    redBox.GetComponent<Image>().color = new Color(redValue, 0, 0, alphaValue);
                }
                else
                {
                    Debug.LogWarning("无法转换颜色值为整数: " + _colorNumb.text);
                }
                break;

            case SetColorType.Green:
                if (int.TryParse(_colorNumb.text, out int tempGreen))
                {
                    greenValue = Mathf.Clamp(tempGreen / 255f, 0f, 1f);
                    greenBox.GetComponent<Image>().color = new Color(0, greenValue, 0, alphaValue);
                }
                else
                {
                    Debug.LogWarning("无法转换颜色值为整数: " + _colorNumb.text);
                }
                break;

            case SetColorType.Blue:
                if (int.TryParse(_colorNumb.text, out int tempBlue))
                {
                    blueValue = Mathf.Clamp(tempBlue / 255f, 0f, 1f);
                    blueBox.GetComponent<Image>().color = new Color(0, 0, blueValue, alphaValue);
                }
                else
                {
                    Debug.LogWarning("无法转换颜色值为整数: " + _colorNumb.text);
                }
                break;

            case SetColorType.Alpha:
                if (int.TryParse(_colorNumb.text, out int tempAlpha))
                {
                    alphaValue = Mathf.Clamp(tempAlpha / 255f, 0f, 1f); // Alpha值也应该在0-1之间
                    alphaBox.GetComponent<Image>().color = new Color(0, 0, 0, alphaValue); // 只更新透明度
                }
                else
                {
                    Debug.LogWarning("无法转换颜色值为整数: " + _colorNumb.text);
                }
                break;
        }
    }

    // 更新 allBox 的颜色
    private void UpdateAllBoxes()
    {
        // 根据 redValue, greenValue, blueValue 和 alphaValue 设置 allBox 的颜色
        allBox.GetComponent<Image>().color = new Color(redValue, greenValue, blueValue, alphaValue);
    }

    /// <summary>
    /// 设置物体颜色，返回设置的颜色RGBA
    /// </summary>
    /// <param name="_toSetColorThing"></param>
    /// <returns></returns>
    public Color GetAllColor(List<Transform> _toSetColorThing, SetThingType _setType)
    {
        switch (_setType)
        {
            case SetThingType.Image:
                for (int i = 0; i < _toSetColorThing.Count; i++)
                {
                    _toSetColorThing[i].GetComponent<Image>().color = new Color(redValue, greenValue, blueValue, alphaValue);
                }
                break;
            case SetThingType.Text:
                for (int i = 0; i < _toSetColorThing.Count; i++)
                {
                    _toSetColorThing[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(redValue, greenValue, blueValue, alphaValue);
                }
                break;
            case SetThingType.GradientStart:
                for (int i = 0; i < _toSetColorThing.Count; i++)
                {
                    _toSetColorThing[i].GetComponent<TodoManager>().newMaterial.SetColor("_GradientStartColor", new Color(redValue, greenValue, blueValue, alphaValue));
                }
                break;
            case SetThingType.GradientEnd:
                for (int i = 0; i < _toSetColorThing.Count; i++)
                {
                    _toSetColorThing[i].GetComponent<TodoManager>().newMaterial.SetColor("_GradientEndColor", new Color(redValue, greenValue, blueValue, alphaValue));
                }
                break;
        }
        return new Color(redValue, greenValue, blueValue, alphaValue);

    }

    public void ToSetColor()
    {
        if (setTransforms != null)
        {
            GetAllColor(setTransforms, setType);
        }
        else
        {
            Debug.LogWarning("错误的打开了调节颜色盘;");
        }
        animator.SetBool("IsClose", true);
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
