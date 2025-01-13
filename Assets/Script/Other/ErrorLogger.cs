using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class ErrorLogger : MonoBehaviour,ISaveManger
{
    public static ErrorLogger Instance;
    private string logDirectory;
    private Dictionary<string, DateTime> lastErrorTimes;
    private TimeSpan errorRepeatThreshold = TimeSpan.FromMinutes(1); // 如果错误重复发生的间隔超过1分钟，认为是新的错误
    private TimeSpan logRetentionThreshold = TimeSpan.FromDays(7); // 设置日志保留期限，默认7天

    private bool autoClearLogs = false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        logDirectory = Application.dataPath + "/ErrorLogs";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 用来记录错误类型和发生的时间
        lastErrorTimes = new Dictionary<string, DateTime>();
        if (autoClearLogs)
        {
            CleanLogsByRetention();
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            // 使用错误信息生成唯一的错误键（可以根据需要更改生成规则）
            string errorKey = GenerateErrorKey(logString, stackTrace);

            // 检查该错误是否在短时间内重复出现
            if (lastErrorTimes.ContainsKey(errorKey))
            {
                TimeSpan timeSinceLastError = DateTime.Now - lastErrorTimes[errorKey];
                if (timeSinceLastError < errorRepeatThreshold)
                {
                    // 如果错误在指定时间内重复发生，跳过日志写入，只标记为持续报错
                    return;
                }
            }

            // 获取当前日期，格式化为文件名（例如：2025-01-10）
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logFilePath = Path.Combine(logDirectory, date + "_error_log.txt");

            // 写入新错误信息
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("Timestamp: " + DateTime.Now.ToString("HH:mm:ss"));
                writer.WriteLine("Log Type: " + type.ToString());
                writer.WriteLine("Message: " + logString);
                writer.WriteLine("Stack Trace: " + stackTrace);
                writer.WriteLine("Error Occurred: First time or after threshold");
                writer.WriteLine("-----------------------------");
            }

            // 更新该错误的最后发生时间
            lastErrorTimes[errorKey] = DateTime.Now;

            Debug.Log("错误日志已保存至: " + logFilePath);
        }
    }

    // 生成一个唯一的错误标识符
    private string GenerateErrorKey(string logString, string stackTrace)
    {
        // 可以根据错误的日志内容、堆栈信息等生成唯一标识符
        // 这里使用消息和堆栈的简化形式生成一个字符串作为唯一标识符
        return logString.GetHashCode() + "-" + stackTrace.GetHashCode();
    }

    /// <summary>
    /// 清理指定日期之前的日志文件
    /// </summary>
    /// <param name="dateThreshold">删除日期之前的日志文件</param>
    public void CleanOldLogs(DateTime dateThreshold)
    {
        try
        {
            var logFiles = Directory.GetFiles(logDirectory, "*_error_log.txt");
            foreach (var file in logFiles)
            {
                DateTime fileDate = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file).Split('_')[0], "yyyy-MM-dd", null);
                if (fileDate < dateThreshold)
                {
                    File.Delete(file);
                    Debug.Log("删除过期日志: " + file);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("清理日志时发生错误: " + e.Message);
        }
    }

    /// <summary>
    /// 清理超过保留期限的日志文件
    /// </summary>
    public void CleanLogsByRetention()
    {
        DateTime retentionDate = DateTime.Now.Subtract(logRetentionThreshold);
        CleanOldLogs(retentionDate);
    }

    public void LoadData(GameData _data)
    {
        autoClearLogs = _data.autoClearLogs;
    }

    public void SaveData(ref GameData _data)
    {
        _data.autoClearLogs = autoClearLogs;
    }
}
