using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//管理每个待办的脚本
public enum ClearFX
{
    /// <summary>
    /// 删除线和字体变灰
    /// </summary>
    StrikethroughAndFontAreGrayedOut,
}

public class TodoManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("待办的字体组件")]
    [SerializeField] private TextMeshProUGUI todoText;
    [SerializeField] private Image bg;
    [Header("渐变的shader")]
    [SerializeField] private Shader useShader;
    [Header("渐变的开始颜色")]
    [SerializeField] private Color startColor = new Color(1f, 0.435f, 0.376f, 1f);
    [Header("渐变的结束颜色")]
    [SerializeField] private Color endColor = new Color(143f / 255f, 0f / 255f, 16f / 255f, 1f);
    [Header("完成效果")]
    [SerializeField] private ClearFX clearFX;
    //是否在按住
    private bool isPointer = false;
    private Material newMaterial;
    //是否完成
    private bool isTodo = false;
    void Start()
    {
        if (bg.TryGetComponent<Image>(out var image))
        {
            newMaterial = new Material(useShader);
            newMaterial.renderQueue = 5000;
            if (newMaterial.shader != null)
            {
                newMaterial.SetColor("_GradientStartColor", startColor);
                newMaterial.SetColor("_GradientEndColor", endColor);
                newMaterial.SetVector("_GradientDirection", new Vector4(-1, 0, 0, 0));
                newMaterial.SetVector("_GradientStartUV", new Vector4(0, 0.5f, 0, 0));
                image.material = newMaterial;
            }
            else
            {
                Debug.LogError("未找到指定的Shader: " + useShader.name);
            }
        }
        else
        {
            Debug.LogError("当前物体上没有找到Renderer组件！");
        }
    }
    private float _GradientStartUVNumb;
    private void Update()
    {
        if (isPointer && !isTodo)
        {
            if (_GradientStartUVNumb >= 2f)
            {
                isTodo = true;
                switch (clearFX)
                {
                    case ClearFX.StrikethroughAndFontAreGrayedOut:
                        todoText.color = Color.gray;
                        todoText.text = $"<s>{todoText.text}</s>";
                        break;

                }
            }
            else
            {
                _GradientStartUVNumb = newMaterial.GetVector("_GradientStartUV").x + Time.deltaTime * 2f;
                _GradientStartUVNumb = Mathf.Clamp(_GradientStartUVNumb, 0f, 2f);
                newMaterial.SetVector("_GradientStartUV", new Vector4(_GradientStartUVNumb, 0.5f, 0, 0));
            }
        }
        else if (!isPointer && !isTodo)
        {
            {
                _GradientStartUVNumb = newMaterial.GetVector("_GradientStartUV").x - Time.deltaTime * 4f;
                _GradientStartUVNumb = Mathf.Clamp(_GradientStartUVNumb, 0f, 2f);
                newMaterial.SetVector("_GradientStartUV", new Vector4(_GradientStartUVNumb, 0.5f, 0, 0));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == 0)
        {
            isPointer = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == 0)
        {
            isPointer = false;
        }
    }
}
