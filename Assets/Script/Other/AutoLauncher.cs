using System;
using System.IO;
using IWshRuntimeLibrary;

public class AutoLauncher
{
    private static readonly string ShortcutName = "SoraWithCat.SoraTodo.lnk";

    public static bool SetStartup(bool enable)
    {
        try
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupPath, ShortcutName);

            if (enable)
            {
                if (!System.IO.File.Exists(shortcutPath))
                {
                    CreateShortcut(startupPath, ShortcutName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }
            }
            else
            {
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            TipWindowManager.Instance.ShowTip("设置启动时出错： " + ex.Message);
            return false;
        }
    }

    private static bool CreateShortcut(string directory, string shortcutName, string targetPath)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string shortcutPath = Path.Combine(directory, shortcutName);
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Save();

            return true;
        }
        catch (Exception ex)
        {
            TipWindowManager.Instance.ShowTip("创建快捷方式时出错： " + ex.Message);
            return false;
        }
    }
}