using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Setup
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            new Task(new Action(delegate
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "CL.IO.Zip.dll", FileMode.Create);
                byte[] data = Setup.Properties.Resources.CL_IO_Zip;
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();

                FileStream fss = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Data.zip", FileMode.Create);
                byte[] datas = Setup.Properties.Resources.Data;
                fss.Write(datas, 0, datas.Length);
                fss.Flush();
                fss.Close();
            })).Start();
        }
    }
}
