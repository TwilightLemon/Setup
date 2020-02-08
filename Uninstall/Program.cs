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
        static async Task Main(string[] args)
        {
            try
            {
                if (MessageBox.Show("您真的要忍心卸载Lemon App吗?/(ㄒoㄒ)/~~", "Lemon App卸载程序", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Process[] processList = Process.GetProcesses();
                    foreach (Process process in processList)
                    {
                        if (process.ProcessName == "LemonApp")
                            process.Kill();
                    }
                    await Task.Delay(2000);
                    try { new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App").Delete(true); } catch { }
                    try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Lemon App.lnk"); } catch { }

                    var a = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    var list = a.GetFiles();
                    foreach (var file in list)
                    {
                        try
                        {
                            if (file.Name != "uninstall.exe")
                                file.Delete();
                        }
                        catch { }
                    }
                    try
                    {
                        RegistryKey hklm = Registry.LocalMachine;
                        hklm.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\LemonApp\", false);
                        hklm.Close();
                    }
                    catch { }
                    MessageBox.Show("卸载成功！o(*￣▽￣*)ブ", "Lemon App卸载程序");
                }
                else MessageBox.Show("感谢您！让我为您继续服务吧！o(*￣▽￣*)ブ", "开心");
            }
            catch { MessageBox.Show("卸载失败惹！o(*￣▽￣*)ブ", "Lemon App卸载程序"); }
        }
    }
}