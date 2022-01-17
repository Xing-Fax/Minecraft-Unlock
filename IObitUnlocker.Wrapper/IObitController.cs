using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace IObitUnlocker.Wrapper
{
    public static class IObitController
    {
        static bool init = false;

        static string SysPath = Path.Combine(Directory.GetCurrentDirectory(), "IObitUnlocker.sys");

        static string DLLPath = Path.Combine(Directory.GetCurrentDirectory(), "IObitUnlocker.dll");

        static string Original_file_1 = Path.Combine(Directory.GetCurrentDirectory(), @"Original\System32\Windows.ApplicationModel.Store.dll");

        static string Original_file_2 = Path.Combine(Directory.GetCurrentDirectory(), @"Original\SysWOW64\Windows.ApplicationModel.Store.dll");

        static string Revise_file_1 = Path.Combine(Directory.GetCurrentDirectory(), @"Revise\System32\Windows.ApplicationModel.Store.dll");

        static string Revise_file_2 = Path.Combine(Directory.GetCurrentDirectory(), @"Revise\SysWOW64\Windows.ApplicationModel.Store.dll");

        public static void Init()
        {
            //写入文件
            File.WriteAllBytes(DLLPath, Properties.Resources.IObitUnlocker);

            File.WriteAllBytes(SysPath, Properties.Resources.IObitUnlockerSyS);

            //设置隐藏属性
            File.SetAttributes(DLLPath, FileAttributes.Hidden);

            File.SetAttributes(SysPath, FileAttributes.Hidden);

            init = true;
        }

        public static bool Freed_file(string str)
        {
            switch (str)
            {
                case "Original":
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Original\SysWOW64\"));
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Original\System32\"));
                        ManagementClass System_Inf = new ManagementClass("Win32_OperatingSystem");
                        string system = string.Empty;
                        foreach (ManagementObject O in System_Inf.GetInstances())
                            system += O["Caption"];
                        if (system.Contains("Windows 11"))
                        {
                            File.WriteAllBytes(Original_file_1, Properties.Resources.Resources_System32_Win11);
                            File.WriteAllBytes(Original_file_2, Properties.Resources.Resources_SysWOW64_Win11);
                        }
                        else
                        {
                            File.WriteAllBytes(Original_file_1, Properties.Resources.Original_System32);
                            File.WriteAllBytes(Original_file_2, Properties.Resources.Original_SysWOW64);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                    if (File.Exists(Original_file_1) && File.Exists(Original_file_2))
                        return true;
                    return false;
                case "Revise":
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Revise\SysWOW64\"));
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Revise\System32\"));
                        File.WriteAllBytes(Revise_file_1, Properties.Resources.Revise_System32);
                        File.WriteAllBytes(Revise_file_2, Properties.Resources.Revise_SysWOW64);
                    }
                    catch
                    {
                        return false;
                    }
                    if (File.Exists(Revise_file_1) && File.Exists(Revise_file_2))
                        return true;
                    return false;
            }
            return false;
        }

        private static void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }

        public static void Delete_file_temp()
        {
            if(Directory.Exists(Path.Combine(Directory.GetCurrentDirectory() + @"\Revise")))
                DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Revise"));
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory() + @"\Original")))
                DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory() + @"\Original"));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IObitDriverBase();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        delegate int IObitDriverUnlockFile(string file, ref uint unk1, FileOperation deleteFile, int unk2, out int unk3);

        static int handle;                              //保存dll句柄
        public static bool DriverStart()
        {
            Init();
            handle = (int)Native.LoadLibraryEx(DLLPath, IntPtr.Zero, LoadLibraryFlags.None);
            var addr = Native.GetProcAddress((IntPtr)handle, "DriverStart");
            var DriverStart = Marshal.GetDelegateForFunctionPointer<IObitDriverBase>(addr);
            return DriverStart();
        }

        public static bool DriverStop()
        {
            var addr = Native.GetProcAddress((IntPtr)handle, "DriverStop");
            var DriverStop = Marshal.GetDelegateForFunctionPointer<IObitDriverBase>(addr);
            return DriverStop();
        }

        public static void DriverClose()
        {
            while (Native.FreeLibrary(handle) != 0);     //通过句柄释放dll
                                                         //删除文件
            if (File.Exists(SysPath))
                File.Delete(SysPath);
            if (File.Exists(DLLPath))
                File.Delete(DLLPath);
        }

        public static int UnlockFile(string file, FileOperation operation)
        {
            if (!init)
            {
                Init();
            }
            var dll = Native.LoadLibraryEx(DLLPath, IntPtr.Zero, LoadLibraryFlags.None);
            var addr = Native.GetProcAddress(dll, "DriverUnlockFile");
            var DriverUnlockFile = Marshal.GetDelegateForFunctionPointer<IObitDriverUnlockFile>(addr);
            uint flag = 0xc08b0000;
            return DriverUnlockFile(file, ref flag, operation, 0, out int unk3);
        }
    }
}
