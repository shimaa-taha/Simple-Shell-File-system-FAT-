using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    public class Mini_Fat
    {
        static int[] Fat = new int[1024];
        public static void Prepare_Fat()
        {
            for (int i = 0; i < 1024; i++)
            {
                if (i == 0 || i == 4)
                {
                    Fat[i] = -1;
                }
                else if (i > 0 && i < 4)
                {
                    Fat[i] = i + 1;
                }
                else
                    Fat[i] = 0;
            }
        }
        public static void Write_Fat()
        {
            byte[] Fat_Convert = new byte[4096];
            System.Buffer.BlockCopy(Fat, 0, Fat_Convert, 0, Fat_Convert.Length);
            for (int i = 0; i < 4; i++)
            {
                byte[] c = new byte[1024];//continue
                for (int j = i * 1024, k = 0; j < (i + 1) * 1024; j++, k++)
                {
                    c[k] = Fat_Convert[j];
                }
                Virtual_Disk.Write_Cluster(i + 1, c);
            }
        }
        public static void Read_Fat()
        {
            byte[] Fat_Convert = new byte[4096];
            for (int i = 0; i < 4; i++)
            {
                byte[] c = Virtual_Disk.Read_Cluster(i + 1); //continue
                for (int j = i * 1024, k = 0; j < (i + 1) * 1024; j++, k++)
                {
                    Fat_Convert[j] = c[k];
                }
            }
            int[] arr = new int[1024];
            for (int i = 0; i < 1024; i++)
            {
                arr[i] = BitConverter.ToInt32(Fat_Convert, i * 4);
            }
            Fat = arr;
        }
        public static int Get_EmptyCluster()
        {
            for (int i = 5; i < 1024; i++)
            {
                if (Fat[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public static void Set_ClusterStatus(int v, int status)
        {
            Fat[v] = status;
        }
        public static int Get_ClusterStatus(int v)
        {
            return Fat[v];
        }
        public static int Get_AvailableClusters()
        {
            int counter = 0;
            for (int i = 5; i < 1024; i++)
            {
                if (Fat[i] == 0)
                {
                    counter++;
                }
            }
            return counter;
        }
        public static void printFat()
        {
            for (int i = 0; i < Fat.Length; i++)
            {
                Console.WriteLine($"FAT[{i}] = {Fat[i]}");
            }
        }
    }
}
