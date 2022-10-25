using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    public class Directory : Directory_Entry
    {
        public List<Directory_Entry> DirsOrFiles;
        public Directory Parent;
        public Directory()
        {

        }
        public Directory(string name, byte attr, int FC, Directory pa) : base(name, attr, FC)//still not complete!
        {
            DirsOrFiles = new List<Directory_Entry>();
            Parent = pa;
        }
        public void Write_Dir()
        {
            Directory_Entry directory_Entry = GetMyDirectory_Entry();
            if (DirsOrFiles.Count != 0)
            {
                byte[] b = new byte[DirsOrFiles.Count * 32];
                byte[] c = new byte[1024];
                List<byte[]> listOfArrays = new List<byte[]>();
                for (int i = 0; i < DirsOrFiles.Count; i++)
                {
                    for (int j = 0; j < DirsOrFiles[i].Dir_Name.Length; j++)
                    {
                        b[j + (i * 32)] = (byte)DirsOrFiles[i].Dir_Name[j];
                    }
                    if (DirsOrFiles[i].Dir_Name.Length < 11)
                    {
                        for (int h = DirsOrFiles[i].Dir_Name.Length; h < 11; h++)
                            b[h + (i * 32)] = (byte)'\0';
                    }
                    b[11 + (i * 32)] = DirsOrFiles[i].Dir_Attribute;
                    System.Buffer.BlockCopy(DirsOrFiles[i].Dir_Empty, 0, b, 12 + (i * 32), DirsOrFiles[i].Dir_Empty.Length);
                    System.Buffer.BlockCopy(BitConverter.GetBytes(DirsOrFiles[i].Dir_FirstCluster), 0, b, 24 + (i * 32), 4);
                    System.Buffer.BlockCopy(BitConverter.GetBytes(DirsOrFiles[i].Dir_FileSize), 0, b, 28 + (i * 32), 4);
                    //////}
                }
                for (int i = 0, j = 0; i < b.Length; i++, j++)
                {
                    if (j % 1024 == 0 && j != 0)//if length less than 1024
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
                if (Parent != null)
                    Dir_FirstCluster = 0;
            }
            if (Parent != null)
            {
                Parent.changeContent(directory_Entry, GetMyDirectory_Entry());
                Parent.Write_Dir();
            }
            Mini_Fat.Write_Fat();
        }
        public void Read_Dir()
        {
            if (Dir_FirstCluster != 0)
            {
                DirsOrFiles = new List<Directory_Entry>();
                int clusterIndex = Dir_FirstCluster;
                int next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                if (clusterIndex == 5 && next == 0)////root
                    return;
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.Read_Cluster(clusterIndex));
                    byte[] c = Virtual_Disk.Read_Cluster(clusterIndex);
                    clusterIndex = next;
                    if (clusterIndex != -1)
                    {
                        next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                    }
                } while (clusterIndex != -1);
                for (int i = 0; i < ls.Count;)
                {
                    byte[] b = new byte[32];
                    for (int m = 0; m < b.Length; m++)
                    {
                        b[m] = ls[i];
                        i++;
                    }
                    if (b[0] == 0)
                        break;
                    Directory_Entry directory_Entry = new Directory_Entry();
                    for (int h = 0; h < 11; h++)
                    {
                        if (b[h] != '\0')
                            directory_Entry.Dir_Name += (char)b[h];
                    }
                    //System.Buffer.BlockCopy(b, 0, directory_Entry.Dir_Name.ToCharArray(), 0, 11);
                    directory_Entry.Dir_Attribute = b[11];
                    System.Buffer.BlockCopy(b, 12, directory_Entry.Dir_Empty, 0, 12);
                    int[] tt = new int[1];
                    System.Buffer.BlockCopy(b, 24, tt, 0, 4);
                    directory_Entry.Dir_FirstCluster = tt[0];
                    int[] yy = new int[1];
                    System.Buffer.BlockCopy(b, 28, yy, 0, 4);
                    directory_Entry.Dir_FileSize = yy[0];
                    DirsOrFiles.Add(directory_Entry);
                }
            }
        }
        public Directory_Entry GetMyDirectory_Entry()
        {
            Directory_Entry directory = new Directory_Entry(this.Dir_Name, this.Dir_Attribute, this.Dir_FirstCluster);
            directory.Dir_Empty = this.Dir_Empty;
            directory.Dir_FileSize = this.Dir_FileSize;
            return directory;
        }
        public int SearchDirOrFiles(string name)
        {
            Read_Dir();
            //name = validNameDirectory(name);
            for (int i = 0; i < DirsOrFiles.Count; i++)
            {
                if (DirsOrFiles[i].Dir_Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public void changeContent(Directory_Entry old, Directory_Entry New)//try,catch if i=-1
        {
            Read_Dir();
            if (SearchDirOrFiles(old.Dir_Name) != -1)
            {
                int index = SearchDirOrFiles(old.Dir_Name);
                DirsOrFiles.RemoveAt(index);
                DirsOrFiles.Add(New);
            }
            else
            {
                Console.WriteLine($"error :{old.Dir_Name} no such file or directory");
            }
        }
        public void RemoveEntry(Directory_Entry directory)
        {
            int index = SearchDirOrFiles(directory.Dir_Name);
            DirsOrFiles.RemoveAt(index);
            Write_Dir();
        }
        public void DeleteDir()
        {
            EmptyMyCluster();
            if (this.Parent != null)
            {
                this.Parent.RemoveEntry(GetMyDirectory_Entry());
            }
            if (Program.current == this)
            {
                if (this.Parent != null)
                {
                    Program.current = this.Parent;
                    Program.path = Program.path.Substring(0, Program.path.IndexOf("\\"));
                }
            }
        }
        public void AddEntry(Directory_Entry directory)
        {
            //directory.Dir_Name = validNameDirectory(Dir_Name);
            DirsOrFiles.Add(directory);
            Write_Dir();
        }
        public void EmptyMyCluster()
        {
            int cluster, next;
            if (this.Dir_FirstCluster != 0)
            {
                cluster = this.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                if (cluster == 5 && next == 0)
                    return;
                while (cluster != -1)
                {
                    Mini_Fat.Set_ClusterStatus(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                    {
                        next = Mini_Fat.Get_ClusterStatus(cluster);
                    }
                }
            }

        }
        public int Get_SizeOfDir()//return number of clusters the dir has
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
        public bool CanAddEntery(Directory_Entry directory)
        {
            int remainder, numOfClusters;
            numOfClusters = ((DirsOrFiles.Count + 1) * 32) / 1024;
            remainder = ((DirsOrFiles.Count + 1) * 32) % 1024;
            if (remainder > 0)
            {
                numOfClusters++;
            }
            numOfClusters += directory.Dir_FileSize / 1024;
            remainder = directory.Dir_FileSize % 1024;
            if (remainder > 0)
            {
                numOfClusters++;
            }
            if (Get_SizeOfDir() + Mini_Fat.Get_AvailableClusters() >= numOfClusters)//is that true?
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
