using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uninstall
{
    class Program
    {
        static string AppName = "Lemon App";
        static string AppFileName = "LemonApp";
        static string title = AppName + "卸载程序";
        static async Task Main(string[] args)
        {
            try
            {
                if (MessageBox.Show("您真的要忍心卸载"+AppName+"吗?/(ㄒoㄒ)/~~", title, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //首先结束掉正在运行的软件
                    Process[] processList = Process.GetProcesses();
                    foreach (Process process in processList)
                    {
                        if (process.ProcessName == AppFileName)
                            process.Kill();
                    }
                    await Task.Delay(2000);

                    //删除开始菜单项目
                    try { new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\"+AppName).Delete(true); } catch { }
                   //删除桌面快捷方式
                    try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\"+AppName+".lnk"); } catch { }

                    //删除软件目录
                    var a = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    var list = a.GetFiles();
                    foreach (var file in list)
                    {
                        try
                        {
                            //别傻了 不能自己删自己
                            if (file.Name != "uninstall.exe")
                                file.Delete();
                        }
                        catch { }
                    }

                    //删除注册表项目
                    try
                    {
                        RegistryKey hklm = Registry.LocalMachine;
                        hklm.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"+AppFileName+@"\", false);
                        hklm.Close();
                    }
                    catch { }
                    MessageBox.Show("卸载成功！o(*￣▽￣*)ブ", title);
                }
                else MessageBox.Show("感谢您！让我为您继续工作吧！o(*￣▽￣*)ブ", "开心");
            }
            catch { MessageBox.Show("卸载失败惹！o(*￣▽￣*)ブ",title); }
        }
    }
}