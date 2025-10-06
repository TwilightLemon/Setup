using CL.IO.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace Setup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 安装信息
        static string AppName = "Lemon App";
        static string AppFileName = "LemonApp";
        static string BuildVersion = "1.0.2";
        static string Publisher = "TwilightLemon";
        static string SignText = "Powered by .NET8";
        static string DefaultPath = "C:\\Program Files\\"+AppFileName;
        #endregion

        private Storyboard StartAni,CloseAni;

        #region Window基本实现
        public MainWindow()
        {
            #region 初始化窗口
            InitializeComponent();
            MouseLeftButtonDown += (sender, e) => {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            };
            Title = AppName + "安装程序";
            TitleRun.Text = AppName;
            signe.Text = SignText;
            doingText.Text = "";
            #endregion
            #region 关闭正在运行的程序
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == AppFileName)
                    process.Kill();
            }
            #endregion

            //载入动画资源
            CloseAni = Resources["Finish"] as Storyboard;
            StartAni = Resources["Start"] as Storyboard;
            StartAni.Completed += delegate {
                Thread v = new Thread(Setup);
                v.Start(new {path=path.Text,istb= checkBox.IsChecked});
            };
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            string LastPath = "";
            try
            {
                //查找已经安装的信息
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey hkSoftWare = hklm.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + AppFileName);
                if(hkSoftWare.ValueCount>2)
                    LastPath = hkSoftWare.GetValue("InstallLocation").ToString();
                hklm.Close();
                hkSoftWare.Close();
            }
            finally
            {
                path.Text = !string.IsNullOrEmpty(LastPath) ? LastPath : DefaultPath;
            }
        }

        private void FinishBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ps = path.Text + "\\" + AppFileName + ".exe";
            CloseAni.Completed += async delegate {
                await Task.Delay(100);
                Process.Start(ps);
                Environment.Exit(0);
            };
            CloseAni.Begin();
        }

        private void closeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseAni.Completed += async delegate {
                await Task.Delay(100);
                Environment.Exit(0);
            };
            CloseAni.Begin();
        }
        #endregion
        #region 安装实现
        private void pro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //安装进度条
            if (e.NewValue == 100)
                //安装完成
                (Resources["OK"] as Storyboard).Begin();
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
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)+ @"\Programs" + @"\"+AppName+@"\", AppName, xt + @"\" + AppFileName + ".exe", null, xt + @"\" + AppFileName + ".exe");
            //创建开始菜单卸载 快捷方式
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs" + @"\"+AppName+@"\", "卸载"+AppName, xt + @"\Uninstall.exe", null, xt + @"\Uninstall.exe");
         
            //创建注册表
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
            //选择安装路径
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                path.Text = fbd.SelectedPath;
                if (!path.Text.EndsWith(AppName))
                    path.Text = Path.Combine(path.Text,AppName);
            }
        }
        #endregion
    }
}
