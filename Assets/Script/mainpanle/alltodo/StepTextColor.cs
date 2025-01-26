using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 文字的背景颜色
/// </summary>
public enum TextBGType
{
    black = 0,
    white = 1
}

public class StepTextColor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private TextBGType textBGType = TextBGType.white;
    private static readonly List<Color> gradientColorsWhiteBackground = new()
    {
    new Color(0f / 255, 0f / 255, 0f / 255), // 黑色
    new Color(255f / 255, 20f / 255, 147f / 255), // 深粉色
    new Color(128f / 255, 0f / 255, 128f / 255), // 紫罗兰色
    new Color(255f / 255, 99f / 255, 71f / 255), // 番茄红
    new Color(255f / 255, 165f / 255, 0), // 橙色
    new Color(0f / 255, 255f / 255, 255f / 255), // 青色
    new Color(255f / 255, 69f / 255, 0), // 火砖红
    new Color(0f / 255, 0f / 255, 255f / 255), // 蓝色
    new Color(240f / 255, 230f / 255, 140f / 255), // 黄土色
    new Color(255f / 255, 248f / 255, 220f / 255), // 杏色
    new Color(255f / 255, 215f / 255, 0), // 金色
    new Color(106f / 255, 90f / 255, 205f / 255), // 紫藤色
    new Color(255f / 255, 128f / 255, 64f / 255), // 橙红色
    new Color(255f / 255, 105f / 255, 180f / 255), // 粉色
    new Color(255f / 255, 182f / 255, 193f / 255), // 淡粉色
    new Color(106f / 255, 90f / 255, 205f / 255), // 紫色
    new Color(255f / 255, 250f / 255, 205f / 255), // 玉米黄
    new Color(0f / 255, 191f / 255, 255f / 255), // 深天蓝
    new Color(255f / 255, 160f / 255, 122f / 255), // 盐油果
    new Color(186f / 255, 85f / 255, 211f / 255), // 紫红色
    new Color(255f / 255, 218f / 255, 185f / 255), // 桃色
    new Color(250f / 255, 128f / 255, 114f / 255), // 鲑鱼红
    new Color(139f / 255, 69f / 255, 19f / 255), // 秘鲁色
    new Color(255f / 255, 182f / 255, 193f / 255), // 浅粉色
    new Color(255f / 255, 140f / 255, 0), // 亮金色
    new Color(0f / 255, 255f / 255, 127f / 255), // 春绿色
    new Color(255f / 255, 99f / 255, 77f / 255), // 红宝石色
    new Color(128f / 255, 128f / 255, 0f / 255), // 橄榄色
    new Color(255f / 255, 250f / 255, 205f / 255), // 玉米色
    new Color(95f / 255, 158f / 255, 160f / 255), // 钢蓝色
    new Color(255f / 255, 69f / 255, 130f / 255), // 红紫色
    new Color(240f / 255, 128f / 255, 128f / 255), // 淡红色
    new Color(0f / 255, 255f / 255, 255f / 255), // 青绿色
    new Color(106f / 255, 90f / 255, 205f / 255), // 薰衣草紫
    new Color(255f / 255, 228f / 255, 181f / 255), // 莲花色
    new Color(160f / 255, 82f / 255, 45f / 255), // 栗色
    new Color(255f / 255, 240f / 255, 245f / 255), // 薄荷白
    new Color(255f / 255, 215f / 255, 0), // 亮金色
    new Color(0f / 255, 255f / 255, 0f / 255), // 绿色
    new Color(72f / 255, 61f / 255, 139f / 255), // 海军蓝
    new Color(218f / 255, 165f / 255, 32f / 255), // 金色
    new Color(0f / 255, 0f / 255, 128f / 255), // 海军蓝
};
    private static readonly List<Color> gradientColorsBlackBackground = new()
    {
    new Color(255f / 255, 0f / 255, 0), // 红色
    new Color(255f / 255, 215f / 255, 0), // 金色
    new Color(255f / 255, 165f / 255, 0), // 橙色
    new Color(255f / 255, 105f / 255, 180f / 255), // 粉色
    new Color(255f / 255, 140f / 255, 0), // 金橙色
    new Color(0f / 255, 255f / 255, 255f / 255), // 青色
    new Color(255f / 255, 99f / 255, 71f / 255), // 番茄红
    new Color(0f / 255, 191f / 255, 255f / 255), // 深天蓝
    new Color(255f / 255, 250f / 255, 205f / 255), // 玉米色
    new Color(138f / 255, 43f / 255, 226f / 255), // 蓝紫色
    new Color(255f / 255, 69f / 255, 130f / 255), // 红紫色
    new Color(106f / 255, 90f / 255, 205f / 255), // 紫藤色
    new Color(255f / 255, 128f / 255, 64f / 255), // 橙红色
    new Color(95f / 255, 158f / 255, 160f / 255), // 钢蓝色
    new Color(255f / 255, 183f / 255, 77f / 255), // 鲜艳黄
    new Color(255f / 255, 69f / 255, 0), // 火砖红
    new Color(255f / 255, 255f / 255, 0), // 明黄色
    new Color(0f / 255, 255f / 255, 127f / 255), // 春绿色
    new Color(255f / 255, 240f / 255, 245f / 255), // 薄荷白
    new Color(255f / 255, 20f / 255, 147f / 255), // 深粉色
    new Color(255f / 255, 182f / 255, 193f / 255), // 淡粉色
    new Color(0f / 255, 0f / 255, 255f / 255), // 蓝色
    new Color(128f / 255, 0f / 255, 128f / 255), // 紫罗兰色
    new Color(250f / 255, 128f / 255, 114f / 255), // 鲑鱼红
    new Color(240f / 255, 230f / 255, 140f / 255), // 黄土色
    new Color(255f / 255, 218f / 255, 185f / 255), // 桃色
    new Color(255f / 255, 228f / 255, 181f / 255), // 莲花色
    new Color(255f / 255, 160f / 255, 122f / 255), // 盐油果
    new Color(34f / 255, 139f / 255, 34f / 255), // 森林绿
    new Color(255f / 255, 250f / 255, 205f / 255), // 玉米黄
    new Color(255f / 255, 215f / 255, 0), // 亮金色
    new Color(255f / 255, 99f / 255, 77f / 255), // 红宝石色
    new Color(255f / 255, 160f / 255, 122f / 255), // 亮盐油果
    new Color(72f / 255, 61f / 255, 139f / 255), // 海军蓝
    new Color(106f / 255, 90f / 255, 205f / 255), // 紫藤色
    new Color(240f / 255, 128f / 255, 128f / 255), // 淡红色
    new Color(255f / 255, 69f / 255, 0), // 火砖色
    new Color(255f / 255, 240f / 255, 245f / 255), // 薄荷白
    new Color(0f / 255, 0f / 255, 128f / 255), // 海军蓝
    new Color(32f / 255, 178f / 255, 170f / 255), // 海蓝色
    new Color(186f / 255, 85f / 255, 211f / 255), // 中紫红
    new Color(255f / 255, 105f / 255, 180f / 255), // 粉色
    new Color(0f / 255, 255f / 255, 255f / 255), // 青绿色
};

    private readonly List<List<Color>> allColorType = new()
    {
    gradientColorsBlackBackground,
    gradientColorsWhiteBackground
};


    private readonly int stepNeedNumb = 100;
    private int currentNumb = 0;
    private int currentStep = 0;

    private void Start()
    {
        StartCoroutine(SmoothTransition(0, currentNumb, 1f));
    }

    /// <summary>
    /// 初始化渐变
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SmoothTransition(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

            SetTextAndColor((int)currentValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetTextAndColor((int)endValue);
    }
    private int GetCurrentStep()
    {
        return Mathf.FloorToInt(currentNumb / stepNeedNumb);
    }

    private float GetCurrentProgress()
    {
        return ((float)currentNumb - ((float)currentStep * (float)stepNeedNumb)) / (float)stepNeedNumb;
    }

    /// <summary>
    /// 根据传入的数字设置对应的颜色和渐变位置
    /// </summary>
    /// <param name="value"></param>
    public void SetTextAndColor(int value)
    {
        currentNumb = value;
        textComponent.text = value.ToString();
        currentStep = GetCurrentStep();
        UpdateFontColorAndMove(currentStep);
    }

    private void UpdateFontColorAndMove(int step)
    {
        List<Color> useColors = allColorType[(int)textBGType];
        if (step >= useColors.Count - 1)
        {
            textComponent.color = useColors[useColors.Count - 1];
        }
        else
        {
            Color startColor = useColors[step];
            Color endColor = useColors[step + 1];
            float progress = GetCurrentProgress();
            TMP_TextInfo textInfo = textComponent.textInfo;

            textComponent.ForceMeshUpdate();

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = charInfo.vertexIndex;

                Color interpolatedColor = Color.Lerp(startColor, endColor, progress);

                Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                colors[vertexIndex] = interpolatedColor;
                colors[vertexIndex + 1] = interpolatedColor;
                colors[vertexIndex + 2] = interpolatedColor;
                colors[vertexIndex + 3] = interpolatedColor;
            }
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
