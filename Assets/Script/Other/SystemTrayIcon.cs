using System;
//using System.Windows.Forms;
using UnityEngine;
using System.Drawing;  // 用于设置托盘图标的图片

public class SystemTrayIcon : MonoBehaviour
{
    //private NotifyIcon trayIcon;
    //private ContextMenu trayMenu;
    //private MenuItem exitMenuItem;
    //private MenuItem actionMenuItem;

    //// 初始化系统托盘图标
    //void Start()
    //{
    //    // 创建菜单项
    //    trayMenu = new ContextMenu();
    //    actionMenuItem = new MenuItem("Execute Function", OnExecuteFunction);
    //    exitMenuItem = new MenuItem("Exit", OnExit);

    //    // 添加菜单项
    //    trayMenu.MenuItems.Add(actionMenuItem);
    //    trayMenu.MenuItems.Add(exitMenuItem);

    //    // 创建托盘图标
    //    trayIcon = new NotifyIcon();
    //    trayIcon.Text = "My Unity Application";
    //    trayIcon.Icon = new Icon("path_to_icon.ico"); // 设置托盘图标
    //    trayIcon.ContextMenu = trayMenu;
    //    trayIcon.Visible = true;
    //}

    //// 执行自定义函数
    //private void OnExecuteFunction(object sender, EventArgs e)
    //{
    //    Debug.Log("Executing custom function from tray menu");
    //    // 在这里执行你希望的函数
    //}

    //// 退出应用
    //private void OnExit(object sender, EventArgs e)
    //{
    //    trayIcon.Visible = false;
    //    Application.Exit();
    //}

    //// 清理托盘图标
    //private void OnApplicationQuit()
    //{
    //    trayIcon.Visible = false;
    //}
}
