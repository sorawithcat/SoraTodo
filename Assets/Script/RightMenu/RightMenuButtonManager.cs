using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightMenuButtonManager : MonoBehaviour
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
}
