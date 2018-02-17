using CL.IO.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
        }

        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                path.Text =fbd.SelectedPath;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            c.Completed += delegate { Environment.Exit(0); };
            this.BeginAnimation(OpacityProperty, c);
        }

        private async void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
                (Resources["START"] as Storyboard).Begin();
                await Task.Delay(500);
                new DirectoryInfo(path.Text).Delete(true);
                ZipHandler handler = ZipHandler.GetInstance();
                handler.UnpackAll(AppDomain.CurrentDomain.BaseDirectory + "Data.zip",path.Text, (num) => { });
                ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "小萌", path.Text + @"\Lemon App.exe", null, path.Text + @"\Lemon App.exe");
                ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "小萌", path.Text + @"\Lemon App.exe", null, path.Text + @"\Lemon App.exe");
                ShortcutCreator.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Lemon App\", "卸载", path.Text + @"\uninstall.exe", null, path.Text + @"\uninstall.exe");
                Process.Start(path.Text + @"\Lemon App.exe");
                Environment.Exit(0);
        }
        bool a = false;
        private void border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (!a)
            { (Resources["OnMouseDown1"] as Storyboard).Begin(); a = true; }
            else { (Resources["OnMouseDown2"] as Storyboard).Begin(); a = false; }
        }

        private void grid1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid.Clip = new RectangleGeometry(new Rect(0, 0, grid.ActualWidth, grid.ActualHeight), 210, 210);
        }
    }
}
