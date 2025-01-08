using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Slider;

public class SliderToIntegerTextUpdater : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Header("滑动条值的最大小值")]
    [SerializeField] private int minValue = 0;
    [SerializeField] private int maxValue = 100;
    private float sliderMinValue;
    private float sliderMaxValue;

    [Header("每次滑动条移动时增加的整数增量")]
    [SerializeField] private int incrementValue = 1;

    [Header("初始值设置")]
    [SerializeField] private int initialValue = 50; // 设置初始值，默认为50

    private int currentValue;
    private float sliderStepSize;
    public float Slidervalue { set => slider.value = value; get => slider.value; }
    public UnityEvent<float> OnvalueChange { get => slider.onValueChanged; }

    void Start()
    {
        if (slider == null || textDisplay == null)
        {
            return;
        }

        // 保存滑动条的实际最小值和最大值  
        sliderMinValue = slider.minValue;
        sliderMaxValue = slider.maxValue;

        // 调整滑动条的实际范围  
        AdjustSliderRange();

        // 设置初始值
        currentValue = Mathf.Clamp(initialValue, minValue, maxValue);
        slider.value = currentValue;
        textDisplay.text = currentValue.ToString();

        // 调用滑动条值变化事件
        OnSliderValueChanged(slider.value);

        // 监听滑动条的变化
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void AdjustSliderRange()
    {
        slider.wholeNumbers = true;
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = Mathf.Clamp(slider.value, minValue, maxValue);
    }

    void OnSliderValueChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value / incrementValue) * incrementValue; // 调整为增量
        currentValue = intValue;
        textDisplay.text = intValue.ToString();
    }

    public void ChangeValueBy(int changeAmount)
    {
        currentValue += changeAmount;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        slider.value = (float)currentValue / incrementValue;
        textDisplay.text = currentValue.ToString();
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

#if UNITY_EDITOR
        OnvalueChange?.RemoveAllListeners();
#endif
    }
}
