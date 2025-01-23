using System;
using System.IO;
using UnityEngine;
public class FileDataHandler
{
    private readonly string dataDirPath = "";
    private readonly string dataFileName = "";

    private readonly bool encryptData = false;
    private readonly string codeWord = "sorawithcat";
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStroe = JsonUtility.ToJson(_data, true);
            if (encryptData)
            {
                dataToStroe = EncryptDecrpyt(dataToStroe);
            }

            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(dataToStroe);
        }
        catch (Exception e)
        {
            TipWindowManager.Instance.ShowTip("在尝试保存数据到文件时出错。" + fullPath + "\n" + e, Color.red, true, canCV: true, isForever: true);

        }
    }


    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }
                if (encryptData)
                {
                    dataToLoad = EncryptDecrpyt(dataToLoad);
                }
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                TipWindowManager.Instance.ShowTip($"在文件读取时错误{fullPath}\n{e}", Color.red, true, canCV: true, isForever: true);
            }
        }
        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrpyt(string _data)
    {
        string modifiodData = "";

        for (int i = 0; i < _data.Length; i++)
        {
            modifiodData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiodData;
    }
}
