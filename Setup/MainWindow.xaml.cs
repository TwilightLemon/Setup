using CL.IO.Zip;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Setup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e) {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            (Resources["OnMouseDown2"] as Storyboard).Completed += delegate { Environment.Exit(0); };
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
            handler.UnpackAll(AppDomain.CurrentDomain.BaseDirectory+"Data.zip", xt, (num) => { Dispatcher.Invoke(() => { pro.Value = num; }); });
            if(istb)ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "小萌", xt + @"\bin.exe", null, xt + @"\bin.exe");
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "小萌", xt + @"\bin.exe", null, xt + @"\bin.exe");
            ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "卸载", xt + @"\uninstall.exe", null, xt + @"\uninstall.exe");
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
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
            Process.Start(path.Text+"\\Lemon App.exe");
            (Resources["OnMouseDown2"] as Storyboard).Begin();
        }
    }
}
