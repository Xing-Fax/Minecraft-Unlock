using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Minecraft
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string Absolute_path = AppDomain.CurrentDomain.BaseDirectory;
        string[] Catalog_file = new string[8];

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/xingchuanzhen/Minecraft_Bypass_the_program");
        }

        private void Log_Write(string str)
        {
            Dispatcher.Invoke(new Action(delegate{ 日志.Text += "[" + DateTime.Now.ToLongTimeString().ToString() + "]: " + str + "\n"; }));
        }

        public MainWindow()
        {
            InitializeComponent();

            Catalog_file[0] = Absolute_path + @"IObit Unlocker\IObitUnlocker.dll";
            Catalog_file[1] = Absolute_path + @"IObit Unlocker\IObitUnlocker.exe";
            Catalog_file[2] = Absolute_path + @"IObit Unlocker\IObitUnlocker.sys";
            Catalog_file[3] = Absolute_path + @"IObit Unlocker\IObitUnlockerExtension.dll";
            Catalog_file[4] = Absolute_path + @"Original file\System32\Windows.ApplicationModel.Store.dll";
            Catalog_file[5] = Absolute_path + @"Original file\SysWOW64\Windows.ApplicationModel.Store.dll";
            Catalog_file[6] = Absolute_path + @"Replace file\System32\Windows.ApplicationModel.Store.dll";
            Catalog_file[7] = Absolute_path + @"Replace file\SysWOW64\Windows.ApplicationModel.Store.dll";

            if (related_functions.Fingerprint_verification.Document_verification() != true)
                Environment.Exit(0);

            Log_Write("程序启动...");
        }

        void Delete_file1(object sender, DoWorkEventArgs e)
        {
            related_functions.CMD.RunCmd2(e.Argument.ToString(), @" /Delete C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll /Advanced");
        }

        void Delete_file2(object sender, DoWorkEventArgs e)
        {
            related_functions.CMD.RunCmd2(e.Argument.ToString(), @" /Delete C:\Windows\System32\Windows.ApplicationModel.Store.dll /Advanced");
        }

        void Start_in_the_background(object sender, DoWorkEventArgs e)
        {
            Log_Write("开始替换文件...");
            Log_Write("检查文件完整性...");
            bool is_it_complete = true;
            for (int i = 0; i < Catalog_file.Length; i++)
                if (!File.Exists(Catalog_file[i]))
                    is_it_complete = false;

            if (is_it_complete == true)
            {
                Log_Write("开始删除文件...");

                using (BackgroundWorker bw = new BackgroundWorker())
                {
                    bw.DoWork += new DoWorkEventHandler(Delete_file1);
                    bw.RunWorkerAsync(Catalog_file[1]);
                }

                using (BackgroundWorker bw = new BackgroundWorker())
                {
                    bw.DoWork += new DoWorkEventHandler(Delete_file2);
                    bw.RunWorkerAsync(Catalog_file[1]);
                }

                while(true)
                {
                    Thread.Sleep(50);
                    if (!File.Exists(@"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll") && 
                        !File.Exists(@"C:\Windows\System32\Windows.ApplicationModel.Store.dll"))
                    {
                        related_functions.CMD.RunCmd("taskkill /IM IObitUnlocker.exe");
                        Log_Write("文件删除完毕!");
                        break;
                    }
                }

                Log_Write("开始复制新的文件...");

                if (e.Argument .ToString () == "0")
                {
                    File.Copy(Catalog_file[6], @"C:\Windows\System32\Windows.ApplicationModel.Store.dll", true);
                    File.Copy(Catalog_file[7], @"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll", true);
                }
                else 
                {
                    File.Copy(Catalog_file[4], @"C:\Windows\System32\Windows.ApplicationModel.Store.dll", true);
                    File.Copy(Catalog_file[5], @"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll", true);
                }


                if (File.Exists(@"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll") &&
                    File.Exists(@"C:\Windows\System32\Windows.ApplicationModel.Store.dll"))
                {
                    Log_Write("复制完毕!");
                }
                else
                {
                    Log_Write("复制失败!!!");
                }
            }
            else
            {
                Log_Write("文件检查失败,可能缺少文件...");
            }
        }

        private void 启动_Click(object sender, RoutedEventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += new DoWorkEventHandler(Start_in_the_background);
                bw.RunWorkerAsync("0");
            }
        }

        private void 关闭_Click(object sender, RoutedEventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += new DoWorkEventHandler(Start_in_the_background);
                bw.RunWorkerAsync("1");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            主窗体.Visibility = Visibility.Collapsed;
            Environment.Exit(0);
        }
    }
}
