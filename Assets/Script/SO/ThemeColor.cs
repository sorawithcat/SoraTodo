using UnityEngine;

[CreateAssetMenu(fileName = "ThemeColor", menuName = "Theme/Color")]
public class ThemeColor : ScriptableObject
{
    [Header("主题")]
    public Theme Theme;
    [Header("顶部导航栏")]
    public Color topBGColor;
    [Header("顶部修饰物")]
    public Color topOtherColor;
    [Header("背景")]
    public Color backgroundColor;

}
