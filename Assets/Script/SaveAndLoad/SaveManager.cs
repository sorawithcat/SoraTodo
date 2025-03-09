using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;

    private List<ISaveManger> saveMangers;
    private FileDataHandler dataHandler;

    [HideInInspector] public bool dontSave = false;

    [ContextMenu("删除保存文件")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        saveMangers = FindAllSaveMangers();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            NewGame();
        }

        foreach (ISaveManger saveManger in saveMangers)
        {
            saveManger.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManger saveManger in saveMangers)
        {
            saveManger.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }

    public void ExitGame()
    {
        SaveGame();

        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (!dontSave)
        {
            SaveGame();
        }
    }

    private List<ISaveManger> FindAllSaveMangers()
    {
        IEnumerable<ISaveManger> saveMangers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManger>();

        return new List<ISaveManger>(saveMangers);
    }

    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }

    public void ReStart()
    {
        //延迟5秒启动

        string[] strs = new string[]
         {
            "@echo off",
            "echo wscript.sleep 5000 > sleep.vbs",
            "start /wait sleep.vbs",
            "start /d \"{0}\" {1}",
            "del /f /s /q sleep.vbs",
            "exit"
         };
        string path = Application.dataPath + "/../SoraTodo";

        List<string> prefabs = new List<string>();
        prefabs = new List<string>(Directory.GetFiles(Application.dataPath + "/../SoraTodo", "*.exe", SearchOption.AllDirectories));
        if (prefabs.Count == 0)
        {
            path = Application.dataPath + "/../";
            prefabs = new List<string>(Directory.GetFiles(Application.dataPath + "/../", "*.exe", SearchOption.AllDirectories));
        }
        foreach (string keyx in prefabs)
        {
            string _path = Application.dataPath;
            _path = _path.Remove(_path.LastIndexOf("/")) + "/";
            Debug.LogError(_path);
            string _name = Path.GetFileName(keyx);
            strs[3] = string.Format(strs[3], _path, _name);
            Application.OpenURL(path);
        }

        string batPath = Application.dataPath + "/../restart.bat";
        if (File.Exists(batPath))
        {
            File.Delete(batPath);
        }
        using (FileStream fileStream = File.OpenWrite(batPath))
        {
            using (StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.GetEncoding("UTF-8")))
            {
                foreach (string s in strs)
                {
                    writer.WriteLine(s);
                }
                writer.Close();
            }
        }
        Application.Quit();
        Application.OpenURL(batPath);
    }
}