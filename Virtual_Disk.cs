using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    public class Virtual_Disk
    {
        static FileStream Disk;
        public static void Initialize_File(string name)
        {
            if (!File.Exists(name))
            {
                Disk = new FileStream(name, FileMode.Create);
                byte[] b = new byte[1024];
                Write_Cluster(0, b);//mini_fat->continue
                Mini_Fat.Prepare_Fat();
                Mini_Fat.Write_Fat();
            }
            else
            {
                Disk = new FileStream(name, FileMode.Open);//mini_fat->continue
                Mini_Fat.Read_Fat();

            }
        }
        public static void Write_Cluster(int v, byte[] b)
        {
            Disk.Seek(v * 1024, SeekOrigin.Begin);
            for (int i = 0; i < b.Length; i++)
            {
                Disk.WriteByte(b[i]);
            }
            Disk.Flush();
        }
        public static byte[] Read_Cluster(int v)
        {
            Disk.Seek(v * 1024, SeekOrigin.Begin);
            byte[] b = new byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                b[i] = (byte)Disk.ReadByte();
            }
            return b;
        }
        public static int Get_FreeSize()
        {
            return (Mini_Fat.Get_AvailableClusters() * 1024);
        }
    }
}
