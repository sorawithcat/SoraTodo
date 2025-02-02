using UnityEngine;

[CreateAssetMenu(fileName = "ThemeColor", menuName = "Theme/Color")]
public class ThemeColor : ScriptableObject
{
    [Header("主题")]
    public Theme Theme;
    [Space]
    [Header("mainPanle")]
    [Header("顶部导航栏")]
    public Color MAINPANLE_TOPBGCOLOR;
    [Header("顶部修饰物")]
    public Color MAINPANLE_TOPOTHERCOLOR;
    [Header("背景")]
    public Color MAINPANLE_BACKGROUNDCOLOR;
    [Header("logo")]
    public Sprite MAINPANLE_LOGO;
    [Header("基础字体颜色")]
    public Color MAINPANLE_BASEFONTSCOLOR;
    [Header("侧边线")]
    public Color MAINPALE_SIDEBARSLINE;
    [Header("选择项底部线")]
    public Color MAINPALE_DOWNLINE;
    [Header("框题字体颜色")]
    public Color MAINPANLE_BOXFONTSCOLOR;
}
