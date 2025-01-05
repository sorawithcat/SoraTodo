using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightMenuButtonManager : MonoBehaviour, IPointerClickHandler
{
    [Header("按钮")]
    [SerializeField] private Button button;
    private TextMeshProUGUI buttonText;
    private Image buttonImage;
    public int buttonID;
    public List<Action> actions;
    private Action action = null;
    private void Start()
    {
        buttonText = button.GetComponent<TextMeshProUGUI>();
        buttonImage = button.GetComponent<Image>();
    }
    private void Update()
    {
        if (actions.Count > 0 && action == null)
        {
            action = (Action)actions[buttonID];
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action());
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerCurrentRaycast.gameObject.tag);
        if (eventData.pointerCurrentRaycast.gameObject.tag == "menuThing")
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {

            }
            //右键-右键菜单（？）
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RightMenuManager.Instance.GetMenuInfo(MenuTags.menuThing);
            }
        }
    }
}
