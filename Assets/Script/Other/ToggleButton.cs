using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Color offColor = new(0.22f, 0.22f, 0.22f); // RGB(55,55,55)
    public Color onColor = new(0.78f, 0.2f, 0.2f);    // RGB(200,50,50)

    [Header("ID")]
    public string ID;

    [Header("过渡时长")]
    [Range(0.1f, 2f)] public float transitionDuration = 0.3f;

    [Header("左右边距比例")]
    [Range(0f, 0.5f)] public float spacingProportion = 0f;

    public bool _isOn;

    private RectTransform _parentRect;
    [SerializeField] private RectTransform _toggleRect;
    private Image _toggleImage;
    private Coroutine _transitionCoroutine;

    [SerializeField] private bool isSwitch;

    [ConditionalHide(nameof(isSwitch))]
    [Range(0f, 100f)]
    public float delayTime = 1f;

    private Vector2 _leftPosition;

    private Vector2 _rightPosition;

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                // 只有当物体在层级中实际激活时才执行动画
                if (gameObject.activeInHierarchy)
                {
                    StartTransition();
                }
            }
        }
    }

    private void OnValidate()
    {
        gameObject.name = ID + "-ToggleButton";
    }

    private void Awake()
    {
        // 获取父物体和子物体组件
        _parentRect = GetComponent<RectTransform>();
        _toggleImage = _toggleRect.GetComponent<Image>();

        // 自动计算位置
        CalculatePositions();

        // 强制初始化位置
        _toggleRect.anchoredPosition = _isOn ? _rightPosition : _leftPosition;
        _toggleImage.color = _isOn ? onColor : offColor;
    }

    // 根据父物体尺寸计算开关位置
    private void CalculatePositions()
    {
        float parentWidth = _parentRect.rect.width - _toggleRect.rect.width;

        float margin = parentWidth * spacingProportion;
        float travelDistance = (parentWidth / 2) - margin;

        _leftPosition = new Vector2(-travelDistance, 0);
        _rightPosition = new Vector2(travelDistance, 0);
    }

    // 外部调用的切换方法
    public void Toggle()
    {
        IsOn = !IsOn;
        SettingManager.Instance.settingToggleButtonsActions[ID]();
        if (isSwitch)
        {
            Invoke(nameof(DelayToggle), delayTime);
        }
    }

    public void DelayToggle()
    {
        IsOn = !IsOn;
    }

    // 启动过渡协程
    private void StartTransition()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionRoutine());
    }

    // 平滑过渡协程
    private IEnumerator TransitionRoutine()
    {
        Vector2 startPos = _toggleRect.anchoredPosition;
        Vector2 targetPos = _isOn ? _rightPosition : _leftPosition;

        Color startColor = _toggleImage.color;
        Color targetColor = _isOn ? onColor : offColor;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            _toggleRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            _toggleImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        // 确保最终位置准确
        _toggleRect.anchoredPosition = targetPos;
        _toggleImage.color = targetColor;

        _transitionCoroutine = null;
    }
}