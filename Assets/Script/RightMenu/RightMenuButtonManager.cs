using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightMenuButtonManager : MonoBehaviour, IPointerClickHandler
{
    [Header("按钮")]
    [SerializeField] private Button button;
    public int buttonID;
    public List<Action> actions;
    private Action action = null;

    private void Update()
    {
        if (actions.Count > 0 && action == null)
        {
            action = (Action)actions[buttonID];
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                RightMenuManager.Instance.HideRightMenu();
                action();
            });
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("menuThing"))
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                RightMenuManager.Instance.HideRightMenu();
            }
            //右键-右键菜单（？）
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RightMenuManager.Instance.GetMenuInfo(MenuTags.menuThing, transform);
            }
        }
    }
}
