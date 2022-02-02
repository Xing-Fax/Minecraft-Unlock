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
using IObitUnlocker.Wrapper;

namespace Minecraft
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 关闭64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        // 开启64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/xingchuanzhen/Minecraft-Unlock");
        }

        private void Log_Write(string str)
        {
            Dispatcher.Invoke(new Action(delegate{ 日志.Text += "[" + DateTime.Now.ToLongTimeString().ToString() + "]: " + str + "\n"; }));
        }

        public MainWindow()
        {
            InitializeComponent();

            if (related_functions.Fingerprint_verification.Document_verification() != true)
               Environment.Exit(0);

            Log_Write("程序启动...");
        }

        void Replace_file(object sender, DoWorkEventArgs e)
        {
            string Parameter = e.Argument.ToString();
            string Path_file1 = @"C:\Windows\System32\Windows.ApplicationModel.Store.dll";
            string Path_file2 = @"C:\Windows\SysWOW64\Windows.ApplicationModel.Store.dll";

            string file_1 = string.Empty;
            string file_2 = string.Empty;

            if (Parameter == "Revise")
            {
                Log_Write("开始替换文件...");
                file_1 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Revise\System32\Windows.ApplicationModel.Store.dll");
                file_2 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Revise\SysWOW64\Windows.ApplicationModel.Store.dll");
                Log_Write("释放资源...");
                if(!IObitController.Freed_file("Revise"))
                {
                    Log_Write("资源释放失败...");
                    return;
                }
            }
            else
            {
                Log_Write("开始还原文件...");
                file_1 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Original\System32\Windows.ApplicationModel.Store.dll");
                file_2 = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Original\SysWOW64\Windows.ApplicationModel.Store.dll");
                Log_Write("释放资源...");
                if (!IObitController.Freed_file("Original"))
                {
                    Log_Write("资源释放失败...");
                    return;
                }
            }

            Log_Write("加载驱动...");
            IObitController.DriverStart();

            Log_Write("删除原始文件...");

            IObitController.UnlockFile(Path_file1, FileOperation.UnlockAndDelete);

            IObitController.UnlockFile(Path_file2, FileOperation.UnlockAndDelete);

            IObitController.DriverStop();
            IObitController.DriverClose(); //释放资源

            Log_Write("开始移动文件...");

            IntPtr oldWOW64State = new IntPtr();
            Wow64DisableWow64FsRedirection(ref oldWOW64State);

            Log_Write(related_functions.CMD.RunCmd("Copy \"" + file_1 + "\" \"" + Path_file1 + "\"").Replace("\n","").Replace(" ", ""));
            Log_Write(related_functions.CMD.RunCmd("Copy \"" + file_2 + "\" \"" + Path_file2 + "\"").Replace("\n", "").Replace(" ",""));

            Wow64RevertWow64FsRedirection(oldWOW64State);

            if (File.Exists(Path_file1) && File.Exists(Path_file2))
            {
                Log_Write("执行成功!!!");
            }
            else
            {
                Log_Write("执行失败!!!");
            }
            IObitController.Delete_file_temp();
        }

        private void 启动_Click(object sender, RoutedEventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += new DoWorkEventHandler(Replace_file);
                bw.RunWorkerAsync("Revise");
            }
        }

        private void 关闭_Click(object sender, RoutedEventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += new DoWorkEventHandler(Replace_file);
                bw.RunWorkerAsync("Original");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            主窗体.Visibility = Visibility.Collapsed;
            IObitController.Delete_file_temp();
            Environment.Exit(0);
        }
    }
}
