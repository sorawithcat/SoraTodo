using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public static class FolderBrowserHelper
{
    #region Window
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenDialogFile ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenDialogFile ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
    #endregion

    public const string IMAGEFILTER = "图片文件(*.jpg;*.png;*.jpeg;*.bmp;*.gif)\0*.jpg;*.png;*.jpeg;*.bmp;*.gif";
    public const string TEXTFILTER = "文本文件(*.txt;*.docx;*.pdf)\0*.txt;*.docx;*.pdf";
    public const string AUDIOFILTER = "音频文件(*.mp3;*.wav;*.flac;*.ogg)\0*.mp3;*.wav;*.flac;*.ogg";
    public const string VIDEOFILTER = "视频文件(*.mp4;*.avi;*.mkv;*.mov)\0*.mp4;*.avi;*.mkv;*.mov";
    public const string ALLFILTER = "所有文件(*.*)\0*.*";
    public const string CODEFILTER = "代码文件(*.cs;*.cpp;*.js;*.html;*.py)\0*.cs;*.cpp;*.js;*.html;*.py";
    public const string COMPRESSIONFILTER = "压缩文件(*.zip;*.rar;*.7z)\0*.zip;*.rar;*.7z";
    public const string SPREADSHEETFILTER = "表格文件(*.xls;*.xlsx;*.csv)\0*.xls;*.xlsx;*.csv";
    public const string PRESENTATIONFILTER = "演示文件(*.ppt;*.pptx)\0*.ppt;*.pptx";
    public const string PDFILTER = "PDF文件(*.pdf)\0*.pdf";



    /// <summary>
    /// 添加一个函数来加载音频文件并设置为 AudioClip
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="audioSource"></param>
    public static void SetAudioClip(string filePath, AudioSource audioSource)
    {
        TipWindowManager.Instance.ShowTip("音频加载中: " + filePath);
        if (File.Exists(filePath))
        {
            // 创建 UnityWebRequest 来加载音频文件
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.UNKNOWN);
            www.SendWebRequest().completed += (operation) =>
            {
                if (www.result == UnityWebRequest.Result.Success)
                {
                    // 加载成功，获取 AudioClip
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = audioClip;
                    if (audioSource.clip != null)
                    {
                        TipWindowManager.Instance.ShowTip("音频加载成功: " + filePath);
                    }
                    else
                    {
                        TipWindowManager.Instance.ShowTip("音频加载失败: " + www.error, Color.red);

                    }
                }
                else
                {
                    TipWindowManager.Instance.ShowTip("音频加载失败: " + www.error, Color.red);
                }
            };
        }
        else
        {
            TipWindowManager.Instance.ShowTip("错误的文件路径" + filePath, Color.red);
        }
    }

    /// <summary>
    /// 选择文件
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="filter"></param>
    public static void SelectFile(Action<string> callback, string filter = ALLFILTER)
    {
        try
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = filter;
            openFileName.file = new string(new char[256]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.title = "选择文件";
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if (GetSaveFileName(openFileName))
            {
                string filepath = openFileName.file;
                if (File.Exists(filepath))
                {
                    callback?.Invoke(filepath);
                }
            }
        }
        catch (Exception e)
        {
            TipWindowManager.Instance.ShowTip(e.Message, Color.red);
        }

        // callback?.Invoke(string.Empty);
    }

    // 获取文件夹路径
    public static string GetPathFromWindowsExplorer(string dialogtitle = "请选择下载路径")
    {
        try
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string(new char[2048]);
            ofn2.lpszTitle = dialogtitle;
            ofn2.ulFlags = 0x00000040;
            IntPtr pidlPtr = SHBrowseForFolder(ofn2);

            char[] charArray = new char[2048];
            Array.Fill(charArray, '\0');

            SHGetPathFromIDList(pidlPtr, charArray);
            string res = new string(charArray);
            return res.Substring(0, res.IndexOf('\0'));
        }
        catch (Exception e)
        {
            TipWindowManager.Instance.ShowTip(e.Message, Color.red);

        }

        return string.Empty;
    }

    // 打开目录
    public static void OpenFolder(string path)
    {
        System.Diagnostics.Process.Start("explorer.exe", path);
    }
}
