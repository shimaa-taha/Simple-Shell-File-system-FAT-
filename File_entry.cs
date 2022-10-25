using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    public class File_entry : Directory_Entry
    {
        public string content;
        public Directory Parent;
        public File_entry()
        {

        }
        public File_entry(string name, byte attr, int FC, Directory Parent, int fileSize, string content) : base(name, attr, FC)
        {
            this.Parent = Parent;
            Dir_FileSize = fileSize;
            this.content = content;
        }
        public Directory_Entry GetMyDirectory_Entry()
        {
            Directory_Entry directory = new Directory_Entry(this.Dir_Name, this.Dir_Attribute, this.Dir_FirstCluster);
            directory.Dir_Empty = this.Dir_Empty;
            directory.Dir_FileSize = this.Dir_FileSize;
            return directory;
        }
        public void EmptyMyCluster()
        {
            int cluster, next;
            if (this.Dir_FirstCluster != 0)
            {
                cluster = this.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                while (next != -1)
                {
                    Mini_Fat.Set_ClusterStatus(cluster, 0);
                    cluster = next;
                    next = Mini_Fat.Get_ClusterStatus(cluster);
                }
            }

        }
        public int Get_SizeOfFile()//return number of clusters the dir has
        {
            int cluster, next, counter = 0;
            if (this.Dir_FirstCluster != 0)
            {
                cluster = this.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                while (next != -1)
                {
                    counter++;
                    cluster = next;
                    next = Mini_Fat.Get_ClusterStatus(cluster);
                }
            }
            return counter;
        }
        public void DeleteFile()
        {
            EmptyMyCluster();
            if (this.Parent != null)
            {
                this.Parent.RemoveEntry(GetMyDirectory_Entry());
            }
        }
        public void PrinteContent()
        {
            Console.WriteLine(content);
        }
        public void WriteFileContent()
        {
            Directory_Entry directory_Entry = GetMyDirectory_Entry();
            if (content.Length != 0)
            {
                byte[] b = new byte[content.Length];
                byte[] c = new byte[1024];
                List<byte[]> listOfArrays = new List<byte[]>();
                for (int i = 0; i < content.Length; i++)
                {
                    b[i] = (byte)content[i];
                }
                for (int i = 0, j = 0; i < b.Length; i++, j++)
                {
                    if (j % 1024 == 0 && j != 0)
                    {
                        listOfArrays.Add(c);
                        c = new byte[1024];
                        j = 0;
                    }
                    c[j] = b[i];
                    if (i == b.Length - 1)
                    {
                        listOfArrays.Add(c);
                    }

                }
                int lastCluster = -1, clusterIndex;
                if (Dir_FirstCluster == 0)
                {
                    clusterIndex = Mini_Fat.Get_EmptyCluster();
                    Dir_FirstCluster = clusterIndex;
                }
                else
                {
                    EmptyMyCluster();
                    clusterIndex = Mini_Fat.Get_EmptyCluster();
                    Dir_FirstCluster = clusterIndex;
                }
                for (int i = 0; i < listOfArrays.Count; i++)
                {
                    Virtual_Disk.Write_Cluster(clusterIndex, listOfArrays[i]);
                    Mini_Fat.Set_ClusterStatus(clusterIndex, -1);
                    if (lastCluster != -1)
                    {
                        Mini_Fat.Set_ClusterStatus(lastCluster, clusterIndex);
                    }
                    lastCluster = clusterIndex;
                    clusterIndex = Mini_Fat.Get_EmptyCluster();
                }
            }
            else
            {
                if (Dir_FirstCluster != 0)
                    EmptyMyCluster();
                Dir_FirstCluster = 0;
            }
            if (Parent != null)
            {
                Parent.changeContent(directory_Entry, GetMyDirectory_Entry());
                Parent.Write_Dir();
            }
            Mini_Fat.Write_Fat();
        }
        public void ReadFileContent()
        {
            if (Dir_FirstCluster != 0)
            {
                // DirsOrFiles = new List<Directory_Entry>();
                int clusterIndex = Dir_FirstCluster;
                int next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                if (clusterIndex == 5 && next == 0)
                    return;
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.Read_Cluster(clusterIndex));
                    //  byte[] c = Virtual_Disk.Read_Cluster(clusterIndex);
                    clusterIndex = next;
                    if (clusterIndex != -1)
                    {
                        next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                    }
                } while (clusterIndex != -1);
                content = string.Empty;
                for (int i = 0; i < ls.Count; i++)///??
                {
                    if ((char)ls[i] != '\0')
                    {
                        content += (char)ls[i];
                    }
                }

            }
        }
    }
}
