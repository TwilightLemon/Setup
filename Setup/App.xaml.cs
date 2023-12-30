using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Setup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        private void AppendLog(Exception e)
        {
            if(e==null) return;
            string log= "\r\n" + e.Message
                + "\r\n 导致错误的对象名称:" + e.Source
                + "\r\n 引发异常的方法:" + e.TargetSite
                + "\r\n  帮助链接:" + e.HelpLink
                + "\r\n 调用堆:" + e.StackTrace;
            FileStream fs = new FileStream("Log.log", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(log);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            args.SetObserved();
            AppendLog(args.Exception);
        }

        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppendLog((Exception)e.ExceptionObject);
        }
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            AppendLog(e.Exception);
        }
    }
}
