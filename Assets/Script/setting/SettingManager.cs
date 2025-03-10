using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingManager : MonoBehaviour, ISaveManger
{
    public static SettingManager Instance;
    [SerializeField] private List<ToggleButton> settingToggleButtons;

    private SerializableDictionary<string, bool> settingToggleButtonsIsOn = new();

    [HideInInspector]
    public SerializableDictionary<string, Action> settingToggleButtonsActions = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        settingToggleButtonsActions = new(){
            { "openErrorLogFile",OpenErrorLogFile},
            { "clearErrorLogs",ClearErrorLogs},
            { "showMiniAuto",ShowMinniAuto},
            { "autoOpen",AutoOpen },
            { "toggleUpDown",ToggleUpDown},
            {"windowOpaque",WindowDontOpaque },
            {"miniPanleDraggableTodo",MiniPanleDraggableTodo},
            {"autoCloseMiniPanleDraggableTodo",AutoCloseMiniPanleDraggableTodo},
            {"reTodoPos" ,ReTodoPos},
            {"clearTodoData",ClearTodoData },
            {"clearSaveData",ClearSaveData }
        };
    }

    private static void OpenErrorLogFile()
    {
        string logDirectory = Path.Combine(Application.persistentDataPath, "ErrorLogs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        System.Diagnostics.Process.Start(logDirectory);
    }

    private static void ClearErrorLogs()
    {
        ErrorLogger.Instance.autoClearLogs = Instance.settingToggleButtons.Find(x => x.ID == "clearErrorLogs").IsOn;
    }

    private static void ShowMinniAuto()
    {
        LoadAllData.Instance.showMini = Instance.settingToggleButtons.Find(x => x.ID == "showMiniAuto").IsOn;
    }

    private static void AutoOpen()
    {
        AutoLauncher.SetStartup(Instance.settingToggleButtons.Find(x => x.ID == "autoOpen").IsOn);
    }

    private static void ToggleUpDown()
    {
        WindowTransparent.Instance.isUp = Instance.settingToggleButtons.Find(x => x.ID == "toggleUpDown").IsOn;
        WindowTransparent.Instance.ToggleUpDown(Instance.settingToggleButtons.Find(x => x.ID == "toggleUpDown").IsOn);
    }

    private static void WindowDontOpaque()
    {
        WindowTransparent.Instance.isOpaque = Instance.settingToggleButtons.Find(x => x.ID == "windowOpaque").IsOn;
        //Todo:如果切换，则设置不透明，不切换则透明
        //WindowTransparent.Instance.SetWindowTransparencyMust(WindowTransparent.Instance.isOpaque);
        TipWindowManager.Instance.ShowTip("这个功能暂时不可用", Color.gray);
    }

    private static void MiniPanleDraggableTodo()
    {
        MiniPanleManager.Instance.miniIsDrag = Instance.settingToggleButtons.Find(x => x.ID == "miniPanleDraggableTodo").IsOn;
        MiniPanleManager.Instance.gameObject.GetComponent<DraggableObject>().notThis = Instance.settingToggleButtons.Find(x => x.ID == "miniPanleDraggableTodo").IsOn;
    }

    private static void AutoCloseMiniPanleDraggableTodo()
    {
        MiniPanleManager.Instance.autoClose = Instance.settingToggleButtons.Find(x => x.ID == "autoCloseMiniPanleDraggableTodo").IsOn;
    }

    private static void ReTodoPos()
    {
        TodoWindowManager.Instance.ResetPosition();
    }

    private static void ClearTodoData()
    {
        LoadAllData.Instance.DeleteTodoData();
        for (int i = 0; i < LoadAllData.Instance.classifyButtonContainer.childCount; i++)
        {
            Destroy(LoadAllData.Instance.classifyButtonContainer.GetChild(i).gameObject);
        }
        LoadAllData.Instance.LoadDataFromJson();
    }

    private static void ClearSaveData()
    {
        SaveManager.Instance.DeleteSavedData();
        SaveManager.Instance.dontSave = true;
        TipWindowManager.Instance.ShowTip("已清除所有保存数据，将在5秒后重启", Color.red, addToErrorFile: false);
#if !UNITY_EDITOR
        SaveManager.Instance.ReStart();
#endif
    }

    public void LoadData(GameData _data)
    {
        if (_data.settingToggleButtonsIsOn.Count != settingToggleButtons.Count)
        {
            _data.settingToggleButtonsIsOn.Clear();
            foreach (ToggleButton item in settingToggleButtons)
            {
                settingToggleButtonsIsOn.Add(item.ID, item.IsOn);
            }
        }
        else
        {
            settingToggleButtonsIsOn = _data.settingToggleButtonsIsOn;
        }
        for (int i = 0; i < settingToggleButtons.Count; i++)
        {
            settingToggleButtons[i].IsOn = settingToggleButtonsIsOn[settingToggleButtons[i].ID];
        }
    }

    public void SaveData(ref GameData _data)
    {
        for (int i = 0; i < settingToggleButtons.Count; i++)
        {
            settingToggleButtonsIsOn[settingToggleButtons[i].ID] = settingToggleButtons[i].IsOn;
        }
        if (_data.autoClose)
        {
            settingToggleButtonsIsOn["miniPanleDraggableTodo"] = false;
        }
        _data.settingToggleButtonsIsOn = settingToggleButtonsIsOn;
    }
}