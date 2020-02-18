﻿using CL.IO.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Windows.Media;

namespace Setup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BlurWindow : Window
    {
        public BlurWindow()
        {
            InitializeComponent();
            var wac = new WindowAccentCompositor(this);
            wac.Color = Color.FromArgb(180, 0, 0, 0);
            wac.IsEnabled = true;
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == "LemonApp")
                    process.Kill();
            }
            this.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e) {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            (Resources["OnMouseDown3"] as Storyboard).Completed += delegate {
                Thread v = new Thread(Setup);
                v.Start(new {path=path.Text,istb= checkBox.IsChecked});
            };
        }
        public void Setup(object xxt) {
            dynamic a = xxt;
            string xt = a.path;
            bool istb = a.istb;
            if (!Directory.Exists(xt))
                Directory.CreateDirectory(xt);
            ZipHandler handler = ZipHandler.GetInstance();
            FileStream fss = new FileStream(xt + "\\Data.zip", FileMode.Create);
            byte[] datas = Properties.Resources.Data;
            fss.Write(datas, 0, datas.Length);
            fss.Flush();
            fss.Close();
            handler.UnpackAll(xt+"\\Data.zip", xt, (num) => { Dispatcher.Invoke(() => { pro.Value = num; }); });
            if(istb)ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Lemon App", xt + @"\LemonApp.exe", null, xt + @"\LemonApp.exe");
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "Lemon App", xt + @"\LemonApp.exe", null, xt + @"\LemonApp.exe");
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "卸载", xt + @"\uninstall.exe", null, xt + @"\uninstall.exe");
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftWare = hklm.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\LemonApp");
            hkSoftWare.SetValue("DisplayIcon", xt + @"\LemonApp.exe", RegistryValueKind.String);
            hkSoftWare.SetValue("DisplayName", "Lemon App", RegistryValueKind.String);
            hkSoftWare.SetValue("DisplayVersion", "1.1.3.2", RegistryValueKind.String);
            hkSoftWare.SetValue("InstallLocation", xt, RegistryValueKind.String);
            hkSoftWare.SetValue("UninstallString", xt + @"\uninstall.exe", RegistryValueKind.String);
            hkSoftWare.SetValue("Publisher", "Twilight./Lemon", RegistryValueKind.String);
            hklm.Close();
            hkSoftWare.Close();
        }
        private void ChooseSetupPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                path.Text = fbd.SelectedPath;
        }

        private void pro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 100)
                (Resources["OK"] as Storyboard).Begin();
        }

        private void border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            var ps = path.Text + "\\LemonApp.exe";
            Process.Start(ps);
            Environment.Exit(0);
        }

        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            string str = "dotnet --version";
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            await p.StandardInput.WriteLineAsync(str + "&exit");
            p.StandardInput.AutoFlush = true;
            string output =await p.StandardOutput.ReadToEndAsync();
            p.WaitForExit();
            p.Close();
            string stre = MainWindow.XtoYGetTo(output+"}", "&exit", "}", 0);
            Version v = new Version(3, 1, 101);
            try
            {
                Version s = new Version(stre);
                if (s < v){
                    if (MessageBox.Show("Lemon App 需要安装.Net Core框架！", "Lemon App安装程序", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        Process.Start("https://dotnet.microsoft.com/download/dotnet-core/current/runtime");
                }
            }
            catch {
                if (MessageBox.Show("Lemon App 需要安装.Net Core框架！", "Lemon App安装程序", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                    Process.Start("https://dotnet.microsoft.com/download/dotnet-core/current/runtime");
            }
        }
        private void closeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}