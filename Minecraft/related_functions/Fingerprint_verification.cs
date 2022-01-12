using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.related_functions
{
    class Fingerprint_verification
    {
        //public static bool CopyFile(string soucrePath, string targetPath)
        //{


        //    //try
        //    //{
        //    //    //读取复制文件流
        //    //    using (FileStream fsRead = new FileStream(soucrePath, FileMode.Open, FileAccess.Read))
        //    //    {
        //    //        //写入文件复制流
        //    //        using (FileStream fsWrite = new FileStream(targetPath, FileMode.OpenOrCreate, FileAccess.Write))
        //    //        {
        //    //            byte[] buffer = new byte[1024 * 1024 * 2];                  //每次读取2M

        //    //            while (true)                                                //可能文件比较大，要循环读取，每次读取2M
        //    //            {
        //    //                int n = fsRead.Read(buffer, 0, buffer.Count());         //每次读取的数据    n：是每次读取到的实际数据大小
        //    //                if (n == 0)                                             //如果n=0说明读取的数据为空，已经读取到最后了，跳出循环
        //    //                    break;
        //    //                fsWrite.Write(buffer, 0, n);                            //写入每次读取的实际数据大小
        //    //            }
        //    //        }
        //    //    }
        //    //    return true;
        //    //}
        //    //catch
        //    //{
        //    //    return false;
        //    //}
        //}
        /// <summary>
        /// 检测自己身签名指纹是否匹配
        /// 有效防止软件被病毒或恶意软件篡改
        /// </summary>
        /// <returns>返回true为正常状态</returns>
        public static bool Document_verification()
        {
            try
            {
                X509Certificate cert = X509Certificate.CreateFromSignedFile(Process.GetCurrentProcess().MainModule.FileName);
                string Fingerprint = cert.GetCertHashString();
                if (Fingerprint == "36A888B9F2A505BF92AC6B2796C2188E639AB1D1")
                { return true; }
                else
                { return false; }
            }
            catch { return false; }
        }
    }
}
