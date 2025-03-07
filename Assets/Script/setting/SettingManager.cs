using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
            { "autoOpen",AutoOpen }
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
        Instance.settingToggleButtons.Find(x => x.ID == "openErrorLogFile").IsOn = false;
    }

    private static void ClearErrorLogs()
    {
        ErrorLogger.Instance.autoClearLogs = Instance.settingToggleButtons.Find(x => x.ID == "clearErrorLogs").IsOn;
    }

    private static void ShowMinniAuto()
    {
        Debug.Log("showMiniAuto");
    }

    private static void AutoOpen()
    {
        Debug.Log("AutoOpen");
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
        _data.settingToggleButtonsIsOn = settingToggleButtonsIsOn;
    }
}