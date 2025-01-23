using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;

    private List<ISaveManger> saveMangers;
    private FileDataHandler dataHandler;

    [ContextMenu("删除保存文件")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
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
        SaveGame();
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
}
