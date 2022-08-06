using CL.IO.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Windows.Media;
using System.Threading.Tasks;

namespace Setup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static string AppName = "Lemon App";
        static string AppFileName = "LemonApp";
        static string BuildVersion = "1.2.4.1";
        static string Publisher = "Twilight./Lemon";
        static string SignText = "Powered by .NET6";
        public MainWindow()
        {
            InitializeComponent();
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == AppFileName)
                    process.Kill();
            }
            MouseLeftButtonDown += (sender,e)=>{
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            };
            (Resources["Start"] as Storyboard).Completed += delegate {
                Thread v = new Thread(Setup);
                v.Start(new {path=path.Text,istb= checkBox.IsChecked});
            };
            Title = AppName + "安装程序";
            TitleRun.Text = AppName;
            signe.Text = SignText;
            doingText.Text = "";
        }
        private void SetupDoing(string text) {
            Dispatcher.Invoke(() => {
                doingText.Text = text;
            });
        }
        private void Setup(object xxt) {
            //准备参数
            dynamic a = xxt;
            string xt = a.path;
            bool istb = a.istb;

            //释放数据文件
            if (!Directory.Exists(xt))
                Directory.CreateDirectory(xt);
            ZipHandler handler = ZipHandler.GetInstance();
            FileStream fss = new FileStream(xt + "\\Data.zip", FileMode.Create);
            byte[] datas = Properties.Resources.Data;
            fss.Write(datas, 0, datas.Length);
            fss.Flush();
            fss.Close();

            //解压缩包
            handler.UnpackAll(xt+"\\Data.zip", xt,
                (num) => { Dispatcher.Invoke(() => {
                    pro.Value = num-1;
                    doingText.Text = "installing... " +num+ "%";
                }); 
            });

            SetupDoing("Creating Shortcuts...");
            //创建桌面快捷方式
            if(istb)ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), AppName, xt + @"\"+AppFileName+".exe", null, xt + @"\"+AppFileName+".exe");
            //创建开始菜单快捷方式
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\"+AppName+@"\", AppName, xt + @"\" + AppFileName + ".exe", null, xt + @"\" + AppFileName + ".exe");
            //创建开始菜单卸载 快捷方式
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\"+AppName+@"\", "卸载"+AppName, xt + @"\Uninstall.exe", null, xt + @"\Uninstall.exe");
         
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"+AppFileName);
            hkSoftWare.SetValue("DisplayIcon", xt + @"\"+AppFileName+".exe", RegistryValueKind.String);
            hkSoftWare.SetValue("DisplayName", AppName, RegistryValueKind.String);
            hkSoftWare.SetValue("DisplayVersion", BuildVersion, RegistryValueKind.String);
            hkSoftWare.SetValue("InstallLocation", xt, RegistryValueKind.String);
            hkSoftWare.SetValue("UninstallString", xt + @"\Uninstall.exe", RegistryValueKind.String);
            hkSoftWare.SetValue("Publisher", Publisher, RegistryValueKind.String);
            hklm.Close();
            hkSoftWare.Close();
            Dispatcher.Invoke(() => { pro.Value =100; });
        }
        private void ChooseSetupPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                path.Text = fbd.SelectedPath;
                if (!path.Text.EndsWith(AppName))
                    path.Text = Path.Combine(path.Text,AppName);
            }
        }

        private void pro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 100)
                (Resources["OK"] as Storyboard).Begin();
        }

        private void FinishBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ps = path.Text + "\\"+AppFileName+".exe";
            var close = Resources["Finish"] as Storyboard;
            close.Completed += async delegate {
                await Task.Delay(100);
                Process.Start(ps);
                Environment.Exit(0);
            };
            close.Begin();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey hkSoftWare = hklm.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + AppFileName);
                string LastPath = hkSoftWare.GetValue("InstallLocation").ToString();
                hklm.Close();
                hkSoftWare.Close();
                path.Text = !string.IsNullOrEmpty(LastPath)?LastPath: "C:\\Program Files\\Lemon App";
            }
            catch { }
        }
        private void closeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var close = Resources["Finish"] as Storyboard;
            close.Completed += async delegate {
                await Task.Delay(100);
                Environment.Exit(0);
            };
            close.Begin();
        }
        public static string XtoYGetTo(string all, string r, string l, int t)
        {

            int A = all.IndexOf(r, t);
            int B = all.IndexOf(l, A + 1);
            if (A < 0 || B < 0)
            {
                return null;
            }
            else
            {
                A = A + r.Length;
                B = B - A;
                if (A < 0 || B < 0)
                {
                    return null;
                }
                return all.Substring(A, B);
            }
        }
    }
}
