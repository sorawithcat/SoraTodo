using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    /// <summary>
    /// 根据给定的颜色生成50个颜色
    /// </summary>
    /// <param name="baseColor"></param>
    /// <returns></returns>
    public static List<Color> GenerateContrastColors(Color baseColor)
    {
        List<Color> generatedColors = new();

        float brightness = baseColor.grayscale;

        for (int i = 0; i < 50; i++)
        {
            float hue = (baseColor.r + baseColor.g + baseColor.b + i * 0.01f) % 1f;
            float saturation = 0.8f;
            float newBrightness = Mathf.Clamp01(brightness + (i % 2 == 0 ? 0.3f : -0.3f));
            generatedColors.Add(Color.HSVToRGB(hue, saturation, newBrightness));
        }

        return generatedColors;
    }
}
