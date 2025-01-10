using TMPro;
using UnityEngine;

public class MatchHeightWithTMP : MonoBehaviour
{
    [SerializeField] private bool isTmp;
    [SerializeField] private TextMeshProUGUI targetTMP;
    [SerializeField] private bool isInputTmp;
    [SerializeField] private TMP_InputField newTMP;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isTmp)
        {
            if (targetTMP != null && rectTransform != null)
            {
                float targetHeight = targetTMP.rectTransform.rect.height;

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            }
        }
        else
        {
            if (isInputTmp)
            {
                if (newTMP != null && rectTransform != null)
                {
                    float targetHeight = newTMP.transform.GetComponent<RectTransform>().rect.height;

                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
                }
            }
        }
    }
}
