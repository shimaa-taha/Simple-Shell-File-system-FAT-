using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    class Program
    {
        public static Directory current;
        public static string path;
        static void Main(string[] args)
        {
            Virtual_Disk.Initialize_File("os");
            Directory root = new Directory("S:", 0x10, 5, null);
            current = root;
            path = current.Dir_Name;
        start://علشان الشاشه تفضل شغاله
            current.Read_Dir();
            //Mini_Fat.printFat();
            Console.Write(path + ">>");
            List<string> list = new List<string>();
            list = DealWithInput();
            Command(list);
            goto start;
        }
        static List<string> DealWithInput()
        {
            string cmd = Console.ReadLine();//باخد الامر من المستخدم
            string[] part;
            List<string> list = new List<string>();
            part = cmd.Split(' ');
            foreach (string s in part)
            {
                if (!s.Equals(""))
                {
                    list.Add(s);
                }
            }
            return list;
        }
        static void Command(List<string> ls)
        {
            ls[0] = ls[0].ToLower();
            if (ls[0] == "help" && ls.Count() == 1)
            {
                Console.WriteLine("cd - Change the current default directory to . If the argument is not present, report the current directory. If the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls - Clear the screen.");
                Console.WriteLine("dir - List the contents of directory.");
                Console.WriteLine("quit - Quit the shell.");
                Console.WriteLine("copy - Copies one or more files to another location");
                Console.WriteLine("del - Deletes one or more files.");
                Console.WriteLine("help - Provides Help information for commands.");
                Console.WriteLine("md - Creates a directory.");
                Console.WriteLine("rd - Removes a directory.");
                Console.WriteLine("rename - Renames a file.");
                Console.WriteLine("type - Displays the contents of a text file.");
                Console.WriteLine("import – import text file(s) from your computer");
                Console.WriteLine("export – export text file(s) to your computer");
            }
            else if (ls[0] == "cd" && ls.Count() == 1)
            {
                Console.WriteLine(Program.path);
            }
            else if (ls[0] == "dir" && ls.Count() == 1)
            {
                Directory root = new Directory("S:", 0x10, 5, null);//how?? compare with dirname or what?
                int countFile = 0, countDir = 0, sumFileSizes = 0;
                if (current.Dir_Name == root.Dir_Name)//compare by dirname
                {
                    Console.WriteLine($"Directory {current.Dir_Name}");
                    Console.WriteLine("<DIR>\t.");
                    if (current.Dir_Name == "S:")
                    {

                    }
                    else
                    {
                        Console.WriteLine("<DIR>\t..");
                    }
                    for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                    {
                        if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                        {//empty or full
                            Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                            countDir++;
                        }
                        else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                            countFile++;
                            sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                        }
                    }
                    Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                    Console.WriteLine($"Number of dirs {countDir}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                }
                else
                {
                    Console.WriteLine($"Directory {current.Dir_Name}");
                    Console.WriteLine("<DIR>\t.");
                    Console.WriteLine("<DIR>\t..");
                    for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                    {
                        if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                        {//empty or full
                            Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                            countDir++;
                        }
                        else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                            countFile++;
                            sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                        }
                    }
                    Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                    Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize()}");//Is this right??
                }
            }
            else if (ls.Count() > 1)//check if it more than 2
            {
                if (ls[0] == "help")
                {
                    switch (ls[1])
                    {
                        case "cd":
                            Console.WriteLine("cd - Change the current default directory to . If the argument is not present, report the current directory. If the directory does not exist an appropriate error should be reported.");
                            break;

                        case "cls":
                            Console.WriteLine("cls - Clear the screen.");
                            break;
                        case "dir":
                            Console.WriteLine("dir - List the contents of directory.");
                            break;
                        case "quit":
                            Console.WriteLine("quit - Quit the shell.");
                            break;
                        case "copy":
                            Console.WriteLine("copy - Copies one or more files to another location");
                            break;
                        case "del":
                            Console.WriteLine("del - Deletes one or more files.");
                            break;
                        case "help":
                            Console.WriteLine("help - Provides Help information for commands.");
                            break;
                        case "md":
                            Console.WriteLine("md - Creates a directory.");
                            break;
                        case "rd":
                            Console.WriteLine("rd - Removes a directory.");
                            break;
                        case "rename":
                            Console.WriteLine("rename - Renames a file.");
                            break;
                        case "type":
                            Console.WriteLine("type - Displays the contents of a text file.");
                            break;
                        case "import":
                            Console.WriteLine("import – import text file(s) from your computer");
                            break;
                        case "export":
                            Console.WriteLine("export – export text file(s) to your computer");
                            break;
                        default:
                            Console.WriteLine($"error :{ls[1]} is not a command");
                            break;
                    }
                }
                else if (ls[0] == "cd" && ls[1] == ".")
                {

                }
                else if (ls[0] == "cd" && ls[1] == "..")
                {
                    if (current.Dir_Name == "S:")
                    {
                        Console.WriteLine("error: you are in the root");
                    }
                    else
                    {
                        if (current.Parent.Parent == null)
                        {
                            current = new Directory(current.Parent.Dir_Name, current.Parent.Dir_Attribute, current.Parent.Dir_FirstCluster, null);
                            path = path.Substring(0, path.LastIndexOf("\\"));
                        }
                        else
                        {
                            current = new Directory(current.Parent.Dir_Name, current.Parent.Dir_Attribute, current.Parent.Dir_FirstCluster, current.Parent.Parent);
                            path = path.Substring(0, path.LastIndexOf("\\"));
                        }
                    }
                }
                else if (ls[0] == "cd" && ls[1] == "S:\\")
                {
                    current = new Directory("S:", 0x10, 5, null);
                    path = ls[1];
                }
                else if (ls[0] == "cd")
                {
                    List<string> pathList;
                    pathList = createPathlist(ls[1]);
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute != 0x0)
                        {
                            current = new Directory(current.DirsOrFiles[index].Dir_Name.ToString(), current.DirsOrFiles[index].Dir_Attribute, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            path = path + '\\' + pathList[0];
                        }
                        else
                        {
                            Console.WriteLine($"error : {pathList[0]} No such a directory");
                        }
                    }
                    else if (pathList.Count > 1)
                    {
                        Directory directory = new Directory();
                        bool found = MoveToDir(ls[1], ref directory);
                        if (found)
                        {
                            current = new Directory(directory.Dir_Name.ToString(), directory.Dir_Attribute, directory.Dir_FirstCluster, directory.Parent);
                            //current = directory;
                            path = ls[1];
                        }
                        else
                        {
                            Console.WriteLine("error : No such a directory");
                        }
                    }

                }
                else if (ls[0] == "dir" && ls[1] == ".")
                {
                    Directory root = new Directory("S:", 0x10, 5, null);//how?? compare with dirname or what?
                    int countFile = 0, countDir = 0, sumFileSizes = 0;
                    if (current.Dir_Name == root.Dir_Name)//compare by dirname
                    {
                        for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                        {
                            if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                            {//empty or full
                                Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                                countDir++;
                            }
                            else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                            {
                                Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                                countFile++;
                                sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                            }
                        }
                        Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                        Console.WriteLine($"Number of dirs {countDir}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                    }
                    else
                    {
                        Console.WriteLine($"Directory {current.Dir_Name}");
                        Console.WriteLine("<DIR>\t.");
                        if (current.Dir_Name == "S:")
                        {

                        }
                        else
                        {
                            Console.WriteLine("<DIR>\t..");
                        }
                        for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                        {
                            if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                            {//empty or full
                                Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                                countDir++;
                            }
                            else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                            {
                                Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                                countFile++;
                                sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                            }
                        }
                        Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                        Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize()}");//Is this right??
                    }
                }
                else if (ls[0] == "dir" && ls[1] == "..")
                {
                    Directory root = new Directory("S:", 0x10, 5, null);//how?? compare with dirname or what?
                    int countFile = 0, countDir = 0, sumFileSizes = 0;
                    if (current.Dir_Name == root.Dir_Name)//compare by dirname
                    {
                        Console.WriteLine("error: you are in the root \n can not print content of parent directory");
                    }
                    else
                    {
                        Console.WriteLine($"Directory {current.Parent.Dir_Name}");
                        Console.WriteLine("<DIR>\t.");
                        if (current.Parent.Dir_Name == "S:")
                        {

                        }
                        else
                        {
                            Console.WriteLine("<DIR>\t..");
                        }
                        for (int i = 0; i < current.Parent.DirsOrFiles.Count(); i++)
                        {
                            if (current.Parent.DirsOrFiles[i].Dir_Attribute == 0x10)
                            {//empty or full
                                Console.WriteLine($"<DIR>\t{current.Parent.DirsOrFiles[i].Dir_Name}");
                                countDir++;
                            }
                            else if (current.Parent.DirsOrFiles[i].Dir_Attribute == 0x0)
                            {
                                Console.WriteLine($"{current.Parent.DirsOrFiles[i].Dir_FileSize}\t{current.Parent.DirsOrFiles[i].Dir_Name}");
                                countFile++;
                                sumFileSizes += current.Parent.DirsOrFiles[i].Dir_FileSize;
                            }
                        }
                        Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                        Console.WriteLine($"Number of dirs {countDir + 2}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                    }
                }
                else if (ls[0] == "dir")
                {
                    List<string> pathList;
                    pathList = createPathlist(ls[1]);
                    if (pathList.Count == 1)
                    {
                        Directory directory;
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)
                        {
                            directory = new Directory(current.DirsOrFiles[index].Dir_Name, 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            directory.Read_Dir();
                            int countFile = 0, countDir = 0, sumFileSizes = 0;
                            Console.WriteLine($"Directory {current.DirsOrFiles[index].Dir_Name}");
                            Console.WriteLine("<DIR>\t.");
                            //Console.WriteLine("<DIR>\t.");
                            Console.WriteLine("<DIR>\t..");
                            //Console.WriteLine($"<DIR>\t{directory.Dir_Name}");
                            //Console.WriteLine($"<DIR>\t{directory.Parent.Dir_Name}");
                            for (int i = 0; i < directory.DirsOrFiles.Count(); i++)
                            {
                                if (directory.DirsOrFiles[i].Dir_Attribute == 0x10)
                                {//empty or full
                                    Console.WriteLine($"<DIR>\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countDir++;
                                }
                                else if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                {
                                    Console.WriteLine($"{directory.DirsOrFiles[i].Dir_FileSize}\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countFile++;
                                    sumFileSizes += directory.DirsOrFiles[i].Dir_FileSize;
                                }
                            }
                            Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                            Console.WriteLine($"Number of dirs {countDir + 2}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                        }
                        else if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"Directory of {current.Dir_Name}");
                            Console.WriteLine($"File Size {current.DirsOrFiles[index].Dir_FileSize}\tFile Name {current.DirsOrFiles[index].Dir_Name}");
                            Console.WriteLine($"Number of files {1}\ttheir size {current.DirsOrFiles[index].Dir_FileSize}");
                            Console.WriteLine($"Number of dirs {0}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                        }
                        else
                        {
                            Console.WriteLine($"error : {pathList[0]} No such a file or directory");
                        }
                    }
                    else if (pathList.Count > 1)
                    {
                        if (pathList[pathList.Count - 1].Contains(".txt"))
                        {
                            File_entry file_Entry = new File_entry();
                            if(MoveToFile(ls[1], ref file_Entry))
                            {
                                file_Entry.ReadFileContent();
                                Console.WriteLine($"Directory of {file_Entry.Parent.Dir_Name}\n\n");
                                Console.WriteLine($"File Size {file_Entry.Dir_FileSize}\tFile Name {file_Entry.Dir_Name}");
                                Console.WriteLine($"Number of files {1}\ttheir size {file_Entry.Dir_FileSize}");
                                Console.WriteLine($"Number of dirs {0}\tFree space {Virtual_Disk.Get_FreeSize()}");//Is this right??
                                //Console.WriteLine($"size {file_Entry.Dir_FileSize}\tname {file_Entry.Dir_Name}");
                            }
                            else
                            {
                                Console.WriteLine("error: No such a file");
                            }

                        }
                        else
                        {
                            Directory directory = new Directory();
                            if(MoveToDir(ls[1], ref directory))
                            {
                                directory.Read_Dir();
                                int countFile = 0, countDir = 0, sumFileSizes = 0;
                                Console.WriteLine("<DIR>\t.");
                                Console.WriteLine("<DIR>\t..");
                                //Console.WriteLine($"<DIR>\t{directory.Dir_Name}");
                                //Console.WriteLine($"<DIR>\t{directory.Parent.Dir_Name}");
                                for (int i = 0; i < directory.DirsOrFiles.Count(); i++)
                                {
                                    if (directory.DirsOrFiles[i].Dir_Attribute == 0x10)
                                    {//empty or full
                                        Console.WriteLine($"<DIR>\t{directory.DirsOrFiles[i].Dir_Name}");
                                        countDir++;
                                    }
                                    else if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                    {
                                        Console.WriteLine($"{directory.DirsOrFiles[i].Dir_FileSize}\t{directory.DirsOrFiles[i].Dir_Name}");
                                        countFile++;
                                        sumFileSizes += directory.DirsOrFiles[i].Dir_FileSize;
                                    }
                                }
                                Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                                Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize()}");//Is this right??
                            }
                            else
                            {
                                Console.WriteLine("error: No such a directory");
                            }
                            
                        }
                    }
                }
                else if (ls[0] == "md")
                {
                    List<string> pathList;
                    pathList = createPathlist(ls[1]);
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index == -1)
                        {
                            Directory_Entry directory_Entry = new Directory_Entry(pathList[0], 0x10, 0);//dirfilesize=0
                            if (current.CanAddEntery(directory_Entry))
                            {
                                current.AddEntry(directory_Entry);
                            }
                            else
                            {
                                Console.WriteLine("error: can not create directory");
                            }
                        }
                        else
                        {
                            Console.WriteLine("error : the directory name is already exist");
                        }
                    }//////full path
                    else
                    {
                        Directory directory = new Directory();
                        string dirName = pathList[pathList.Count - 1];

                        ls[1] = ls[1].Substring(0, ls[1].LastIndexOf("\\"));/////////////////edit 


                        if (MoveToDir(ls[1], ref directory))
                        {
                            directory.Read_Dir();
                            int index = directory.SearchDirOrFiles(dirName);
                            if (index == -1)
                            {
                                Directory_Entry directory_Entry = new Directory_Entry(dirName, 0x10, 0);
                                if (directory.CanAddEntery(directory_Entry))
                                {
                                    directory.AddEntry(directory_Entry);
                                }
                                else
                                {
                                    Console.WriteLine("error : there is no space to create the directory");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Duplicate name of the directory");
                            }
                        }
                        else
                        {
                            Console.WriteLine("error");//
                        }
                    }
                }
                else if (ls[0] == "rd")
                {
                    for(int i = 1; i < ls.Count; i++)
                    {
                        List<string> pathList;
                        pathList = createPathlist(ls[i]);
                        if (pathList.Count == 1)
                        {
                            int index = current.SearchDirOrFiles(pathList[0]);
                            if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)
                            {
                                Directory directory = new Directory(current.DirsOrFiles[index].Dir_Name, 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                                if (directory.Dir_FirstCluster == 0)
                                {
                                    Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                    string s;
                                    do
                                    {
                                        s = Console.ReadLine().ToLower();
                                        if (s == "yes")
                                        {
                                            directory.DeleteDir();
                                            break;
                                        }
                                        else if (s == "no")
                                        {
                                            break;
                                        }
                                    } while (s != "yes" || s != "no");
                                }
                                else
                                {
                                    Console.WriteLine("error: can not delete a full directory");
                                }

                            }
                            else
                            {
                                Console.WriteLine("error: the directory does not exist");
                            }
                        }
                        else
                        {
                            Directory directory = new Directory();
                            if (MoveToDir(ls[i], ref directory))
                            {
                                if (directory.Dir_FirstCluster == 0)
                                {
                                    Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                    string s;
                                    do
                                    {
                                        s = Console.ReadLine().ToLower();
                                        if (s == "yes")
                                        {
                                            directory.DeleteDir();
                                            break;
                                        }
                                        else if (s == "no")
                                        {
                                            break;
                                        }
                                    } while (s != "yes" || s != "no");
                                }
                            }
                            else
                            {
                                Console.WriteLine("error : directory is not exist ");
                            }
                        }
                    }
                    
                }
                else if (ls[0] == "type")
                {
                    for (int i = 1; i < ls.Count; i++)
                    {
                        List<string> pathList;
                        pathList = createPathlist(ls[i]);
                        if (pathList.Count == 1)
                        {
                            int index = current.SearchDirOrFiles(pathList[0]);
                            if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                            {
                                File_entry file = new File_entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                                file.ReadFileContent();
                                file.PrinteContent();
                            }
                            else
                            {
                                Console.WriteLine($"error : {pathList[0]} does not exist");
                            }
                        }
                        else
                        {
                            File_entry file = new File_entry();
                            if (MoveToFile(ls[i], ref file))
                            {
                                file.ReadFileContent();
                                file.PrinteContent();
                            }
                            else
                            {
                                Console.WriteLine("error: no such a file");
                            }
                        }
                        Console.WriteLine();
                    }
                }
                else if (ls[0] == "rename")//////////////////////////change file
                {
                    List<string> pathList;
                    pathList = createPathlist(ls[1]);
                    if (pathList.Count == 1)//just the name of old file
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                        {
                            if (ls[2].Contains(".txt")&&ls[2].IndexOf("S:\\")==-1)
                            {
                                Directory_Entry OLD = new Directory_Entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current.DirsOrFiles[index].Dir_FileSize);
                                if (current.SearchDirOrFiles(ls[2]) == -1)
                                {
                                    Directory_Entry NEW = new Directory_Entry(ls[2], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current.DirsOrFiles[index].Dir_FileSize);
                                    current.changeContent(OLD, NEW);
                                    current.Write_Dir();
                                }
                                else
                                {
                                    Console.WriteLine($"{ls[2]} is already exist");
                                }
                            }
                            else
                            {
                                Console.WriteLine("error the new file name should be a file name only");
                            }
                        }
                        else
                        {
                          
                            Console.WriteLine($"{pathList[0]} is not exist");
                        }
                    }
                    else//full path
                    {
                        File_entry file = new File_entry();
                        if (MoveToFile(ls[1], ref file))
                        {
                            if (ls[2].Contains(".txt") && ls[2].IndexOf("S:\\") == -1)
                            {
                                Directory_Entry OLD = new Directory_Entry(file.Dir_Name, 0x0, file.Dir_FirstCluster,file.Dir_FileSize);
                                if (file.Parent.SearchDirOrFiles(ls[2]) == -1)
                                {
                                    Directory_Entry NEW = new Directory_Entry(ls[2], 0x0, file.Dir_FirstCluster, file.Dir_FileSize);
                                    file.Parent.changeContent(OLD, NEW);
                                    file.Parent.Write_Dir();
                                }
                                else
                                {
                                    Console.WriteLine($"{ls[2]} is already exist");
                                }
                            }
                            else
                            {
                                Console.WriteLine("error the new file name should be a file name only");
                            }
                        }
                        else
                        {
                            Console.WriteLine("the file is not exist");
                        }
                    }
                }
                else if (ls[0] == "del")
                {//for loop for every file or directory the user will enter
                    for(int j = 1; j < ls.Count; j++)
                    {
                        List<string> pathList;
                        pathList = createPathlist(ls[j]);
                        if (pathList.Count == 1)
                        {
                            int index = current.SearchDirOrFiles(pathList[0]);
                            if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)//name of file
                            {
                                File_entry file = new File_entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                                Console.WriteLine("Are you sure you want to delete this file? yes or no");
                                string s;
                                do
                                {
                                    s = Console.ReadLine().ToLower();
                                    if (s == "yes")
                                    {
                                        file.DeleteFile();
                                        break;
                                    }
                                    else if (s == "no")
                                    {
                                        break;
                                    }
                                } while (s != "yes" || s != "no");
                            }
                            else if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)//name of directory
                            {
                                Directory directory = new Directory(current.DirsOrFiles[index].Dir_Name, 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                                directory.Read_Dir();
                             
                                    Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                    string s;
                                    do
                                    {
                                        s = Console.ReadLine().ToLower();
                                        if (s == "yes")
                                        {
                                            for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                            {
                                                if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                                {
                                                    File_entry file1 = new File_entry(directory.DirsOrFiles[i].Dir_Name, directory.DirsOrFiles[i].Dir_Attribute, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                    file1.DeleteFile();

                                                }
                                            }
                                            break;
                                        }
                                        else if (s == "no")
                                        {
                                            break;
                                        }
                                    } while (s != "yes" || s != "no");
                                
                            }
                            else
                            {
                                Console.WriteLine($"{pathList[0]} is not exist");
                            }
                        }
                        else
                        {
                            File_entry file = new File_entry();//full path to file
                            if (MoveToFile(ls[j], ref file))
                            {
                                Console.WriteLine("Are you sure you want to delete this file? yes or no");
                                string s;
                                do
                                {
                                    s = Console.ReadLine().ToLower();
                                    if (s == "yes")
                                    {
                                        file.DeleteFile();
                                        break;
                                    }
                                    else if (s == "no")
                                    {
                                        break;
                                    }
                                } while (s != "yes" || s != "no");
                            }
                            else
                            {
                                Directory directory = new Directory();//full path to directory
                                if (MoveToDir(ls[j], ref directory))
                                {
                                    directory.Read_Dir();
                                        Console.WriteLine("Are you sure you want to delete files in this directory? yes or no");
                                        string s;
                                        do
                                        {
                                            s = Console.ReadLine().ToLower();
                                            if (s == "yes")
                                            {
                                                for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                                {
                                                    if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                                    {
                                                        File_entry file1 = new File_entry(directory.DirsOrFiles[i].Dir_Name, directory.DirsOrFiles[i].Dir_Attribute, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                        file1.DeleteFile();

                                                    }
                                                }
                                                break;
                                            }
                                            else if (s == "no")
                                            {
                                                break;
                                            }
                                        } while (s != "yes" || s != "no");
                                   
                                }
                                else
                                {
                                    Console.WriteLine("error :the file or directory is not exist");
                                }
                            }
                        }
                    }
                    
                }
                else if (ls[0] == "import")
                {
                    if (File.Exists(ls[1]))/////file name 
                    {
                        if (ls.Count == 2)//import in current directory
                        {
                            string content = File.ReadAllText(ls[1]);//get content
                            int index = current.SearchDirOrFiles(ls[1]);
                            if (index == -1)
                            {
                                CreateFile(ref current, ls[1], content);
                            }
                            else
                            {
                                Console.WriteLine("file is exist you can't add it");
                            }
                        }
                        else////import source destination
                        {
                            List<string> pathList;
                            pathList = createPathlist(ls[2]);
                            if (pathList.Count > 1)///full path directory
                            {
                                string content = File.ReadAllText(ls[1]);//get content
                                Directory directory = new Directory();
                                if (MoveToDir(ls[2], ref directory))
                                {
                                    int index = directory.SearchDirOrFiles(ls[1]);
                                    if (index == -1)//////
                                    {
                                        CreateFile(ref directory, ls[1], content);
                                    }
                                    else
                                    {
                                        Console.WriteLine("file is exist you can't add it");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"error: the path {ls[2]} does not exist");
                                }
                            }
                            else////destination directory
                            {
                                string content = File.ReadAllText(ls[1]);//get content
                                int index = current.SearchDirOrFiles(ls[2]);
                                if (index != -1)
                                {
                                    Directory directory = new Directory(ls[2], 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                                    int index1 = directory.SearchDirOrFiles(ls[1]);
                                    if (index1 == -1)
                                    {
                                        CreateFile(ref directory, ls[1], content);
                                    }
                                    else
                                    {
                                        Console.WriteLine("file is exist you can't add it");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"error: the destination directory {ls[2]} does not exist");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("the file does not exist");
                    }
                }
                else if (ls[0] == "copy")
                {
                    if (ls.Count == 2)//just source
                    {
                        List<string> pathList;
                        pathList = createPathlist(ls[1]);
                        if (pathList.Count == 1 && pathList[0].Contains(".txt"))//and the name extention is txt//file name
                        {
                            if (current.SearchDirOrFiles(pathList[0]) == -1)
                            {
                                Console.WriteLine("error: the file does not exist");
                            }
                            else
                            {
                                Console.WriteLine("error: the file can not copied to itself");//overwrite?
                            }
                        }
                        else if (pathList.Count == 1)//directory name
                        {
                            int index = current.SearchDirOrFiles(pathList[0]);
                            if (index != -1)
                            {
                                Directory directory = new Directory(pathList[0], 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                                directory.Read_Dir();
                                for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                {
                                    if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                    {
                                        File_entry file = new File_entry(directory.DirsOrFiles[i].Dir_Name, 0x0, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                        file.ReadFileContent();/////////////////////////////////////////////////////////////
                                        if (current.SearchDirOrFiles(file.Dir_Name) == -1)
                                        {
                                            CreateFile(ref current, file.Dir_Name, file.content);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Are you sure you want to overwrite this directory? yes or no");
                                            string s;
                                            do
                                            {
                                                s = Console.ReadLine().ToLower();
                                                if (s == "yes")
                                                {
                                                    OverWriteFile(ref current, file.Dir_Name, file.content);
                                                    break;
                                                }
                                                else if (s == "no")
                                                {
                                                    break;
                                                }
                                            } while (s != "yes" || s != "no");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("error: the directory does not exist");
                            }
                        }
                        else//full path
                        {
                            if (pathList.Count > 1 && pathList[pathList.Count - 1].Contains(".txt"))//full path for a file
                            {
                                File_entry file = new File_entry();
                                if (MoveToFile(ls[1], ref file))
                                {
                                    for (int i = 0; i < current.DirsOrFiles.Count; i++)
                                    {
                                        if (file.Dir_Name == current.DirsOrFiles[i].Dir_Name)
                                        {
                                            Console.WriteLine("error: the file can not copied to itself");////////break
                                            break;
                                        }
                                    }
                                    file.ReadFileContent();/////////////////////////////////////////////////////////////
                                    if (current.SearchDirOrFiles(file.Dir_Name) == -1)
                                    {
                                        CreateFile(ref current, file.Dir_Name, file.content);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Are you sure you want to overwrite this directory? yes or no");
                                        string s;
                                        do
                                        {
                                            s = Console.ReadLine().ToLower();
                                            if (s == "yes")
                                            {
                                                OverWriteFile(ref current, file.Dir_Name, file.content);
                                                break;
                                            }
                                            else if (s == "no")
                                            {
                                                break;
                                            }
                                        } while (s != "yes" || s != "no");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("error :no such a file");
                                }
                            }
                            else//full path for directory
                            {
                                Directory directory = new Directory();
                                if (MoveToDir(ls[1], ref directory))
                                {
                                    directory.Read_Dir();
                                    for (int i = 0; i < current.DirsOrFiles.Count; i++)
                                    {
                                        if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                                        {
                                            File_entry file = new File_entry(current.DirsOrFiles[i].Dir_Name, 0x0, current.DirsOrFiles[i].Dir_FirstCluster, current, current.DirsOrFiles[i].Dir_FileSize, null);
                                            file.ReadFileContent();/////////////////////////////////////////////////////////////
                                            if (current.SearchDirOrFiles(file.Dir_Name) == -1)
                                            {
                                                CreateFile(ref current, file.Dir_Name, file.content);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Are you sure you want to overwrite this directory? yes or no");
                                                string s;
                                                do
                                                {
                                                    s = Console.ReadLine().ToLower();
                                                    if (s == "yes")
                                                    {
                                                        OverWriteFile(ref current, file.Dir_Name, file.content);
                                                    }
                                                    else if (s == "no")
                                                    {
                                                        break;
                                                    }
                                                } while (s != "yes" || s != "no");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("error: the directory does not exist");
                                }
                            }
                        }
                    }
                    else//source and destination
                    {
                        List<string> pathList;
                        pathList = createPathlist(ls[1]);
                        if (pathList.Count == 1)////////////source -----> directory name 
                        {
                            int index = current.SearchDirOrFiles(ls[1]);
                            if (index != -1)
                            {
                                Directory directory = new Directory(ls[1], 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                                directory.Read_Dir();
                                List<string> destin_Path;
                                destin_Path = createPathlist(ls[2]);
                                if (destin_Path.Count == 1 && !destin_Path[0].Contains(".txt"))////destination--->directory name
                                {
                                    int index1 = current.SearchDirOrFiles(ls[2]);
                                    if (index1 != -1)
                                    {
                                        Directory directory1 = new Directory(ls[2], 0x10, current.DirsOrFiles[index1].Dir_FirstCluster, current);
                                        directory1.Read_Dir();
                                        for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                        {
                                            if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                            {
                                                File_entry file = new File_entry(directory.DirsOrFiles[i].Dir_Name, 0x0, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                file.ReadFileContent();/////////////////////////////////////////////////////////////
                                                if (directory1.SearchDirOrFiles(file.Dir_Name) == -1)
                                                {
                                                    CreateFile(ref directory1, file.Dir_Name, file.content);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Are you sure you want to overwrite this directory? yes or no");
                                                    string s;
                                                    do
                                                    {
                                                        s = Console.ReadLine().ToLower();
                                                        if (s == "yes")
                                                        {
                                                            OverWriteFile(ref directory1, file.Dir_Name, file.content);
                                                        }
                                                        else if (s == "no")
                                                        {
                                                            break;
                                                        }
                                                    } while (s != "yes" || s != "no");
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (destin_Path.Count > 1 && !destin_Path[destin_Path.Count - 1].Contains(".txt"))///full path -->dir
                                {
                                    Directory directory1 = new Directory();
                                    if (MoveToDir(ls[2], ref directory1))
                                    {
                                        directory1.Read_Dir();
                                        for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                        {
                                            if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                            {
                                                File_entry file = new File_entry(directory.DirsOrFiles[i].Dir_Name, 0x0, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                file.ReadFileContent();/////////////////////////////////////////////////////////////
                                                if (directory1.SearchDirOrFiles(file.Dir_Name) == -1)
                                                {
                                                    CreateFile(ref directory1, file.Dir_Name, file.content);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Are you sure you want to overwrite this directory? yes or no");
                                                    string s;
                                                    do
                                                    {
                                                        s = Console.ReadLine().ToLower();
                                                        if (s == "yes")
                                                        {
                                                            OverWriteFile(ref directory1, file.Dir_Name, file.content);
                                                        }
                                                        else if (s == "no")
                                                        {
                                                            break;
                                                        }
                                                    } while (s != "yes" || s != "no");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("error: directory does not exist");
                                    }
                                }
                                else if (destin_Path.Count == 1 && destin_Path[0].Contains(".txt"))////destination--->file name
                                {
                                    string contents = null;

                                    for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                    {
                                        if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                        {
                                            File_entry file = new File_entry(directory.DirsOrFiles[i].Dir_Name, 0x0, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                            file.ReadFileContent();/////////////////////////////////////////////////////////////
                                            contents += file.content;/////اناكدا استفدت اى؟
                                        }
                                    }
                                    int index1 = current.SearchDirOrFiles(destin_Path[0]);
                                    if (index1 != -1)
                                    {
                                        if (!CreateFile(ref current, destin_Path[0], contents))
                                        {
                                            Console.WriteLine("error: can not create the file");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Are you sure you want to overwrite this file? yes or no");
                                        string s;
                                        do
                                        {
                                            s = Console.ReadLine().ToLower();
                                            if (s == "yes")
                                            {
                                                if (OverWriteFile(ref current, destin_Path[0], contents) == 0)/////tests 0 -1 1
                                                {
                                                    Console.WriteLine("error:file does not exist");
                                                }
                                                else if (OverWriteFile(ref current, destin_Path[0], contents) == -1)
                                                {
                                                    Console.WriteLine("error:file can not be overwrited");
                                                }

                                            }
                                            else if (s == "no")
                                            {
                                                break;
                                            }
                                        } while (s != "yes" || s != "no");
                                    }
                                }
                                else if (destin_Path.Count > 1 && destin_Path[destin_Path.Count - 1].Contains(".txt"))////full path for file 
                                {
                                    string contents = null;

                                    for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                    {
                                        if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                        {
                                            File_entry file = new File_entry(directory.DirsOrFiles[i].Dir_Name, 0x0, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                            file.ReadFileContent();/////////////////////////////////////////////////////////////
                                            contents += file.content;/////اناكدا استفدت اى؟
                                        }
                                    }
                                    Directory directory1 = new Directory();
                                    if (MoveToDir(ls[2], ref directory1))
                                    {
                                        Console.WriteLine("Are you sure you want to overwrite this file? yes or no");
                                        string s;
                                        do
                                        {
                                            s = Console.ReadLine().ToLower();
                                            if (s == "yes")
                                            {
                                                if (OverWriteFile(ref directory1, destin_Path[0], contents) == 0)/////tests 0 -1 1
                                                {
                                                    Console.WriteLine("error:file does not exist");
                                                }
                                                else if (OverWriteFile(ref directory1, destin_Path[0], contents) == -1)
                                                {
                                                    Console.WriteLine("error:file can not be overwrited");
                                                }

                                            }
                                            else if (s == "no")
                                            {
                                                break;
                                            }
                                        } while (s != "yes" || s != "no");
                                    }
                                    else
                                    {
                                        if (!CreateFile(ref directory1, destin_Path[destin_Path.Count - 1], contents))
                                        {
                                            Console.WriteLine("error: can not create the file");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("error: the dirsctory does not exists");
                            }
                        }
                        else if (pathList.Count == 1 && pathList[pathList.Count - 1].Contains(".txt"))///source ---> file name
                        {

                        }

                    }
                }
                else if (ls[0] == "export")//export source destination
                {
                    List<string> pathList;
                    pathList = createPathlist(ls[1]);
                    if (pathList.Count > 1)///source full path to file
                    {
                        if (System.IO.Directory.Exists(ls[2]))
                        {
                            File_entry file = new File_entry();
                            if (MoveToFile(ls[1], ref file))
                            {
                                file.ReadFileContent();
                                StreamWriter f = new StreamWriter(ls[2] + "\\" + pathList[pathList.Count - 1]);
                                f.Write(file.content);
                                f.Flush();
                                f.Close();
                            }
                            else
                            {
                                Console.WriteLine($"error: the path {ls[1]} does not exist");
                            }
                        }
                        else
                        {
                            Console.WriteLine("the path of directory does not exist");
                        }
                    }
                    else////name of file 
                    {
                        int index = current.SearchDirOrFiles(ls[1]);//check if the file does exist
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                        {
                            if (System.IO.Directory.Exists(ls[2]))
                            {//check if the path of destination does exist

                                File_entry file = new File_entry(ls[1], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                                file.ReadFileContent();
                                StreamWriter f = new StreamWriter(ls[2] + "\\" + ls[1]);
                                f.Write(file.content);
                                f.Flush();
                                f.Close();
                            }
                            else
                            {
                                Console.WriteLine("the path of directory does not exist");
                            }
                        }
                        else
                        {
                            Console.WriteLine("the file does not exist");
                        }
                    }
                }
                else if (ls[0] == "cls") Console.Clear();
                else if (ls[0] == "quit") Environment.Exit(1);
                else Console.WriteLine("'" + ls[0] + ls[1] + "'" + "not command");//for loop 
            }
            else if (ls[0] == "cls") Console.Clear();
            else if (ls[0] == "quit") Environment.Exit(1);
            else Console.WriteLine("'" + ls[0] + "'" + "not command");
        }
        public static bool MoveToDir(string path, ref Directory directory)
        {
            bool found = false;
            Directory root = new Directory("S:", 0x10, 5, null);
            List<string> list = new List<string>();
            string[] part;
            part = path.Split('\\');
            foreach (string s in part)
            {
                list.Add(s);
            }
            if (list[0] == "S:")
            {
                for (int i = 1; i < list.Count(); i++)
                {
                    int index = root.SearchDirOrFiles(list[i]);
                    if (index != -1)
                    {
                        found = true;
                        Directory root1 = new Directory(root.DirsOrFiles[index].Dir_Name, root.DirsOrFiles[index].Dir_Attribute, root.DirsOrFiles[index].Dir_FirstCluster, root);
                        root = root1;
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }
            }
            else
            {
                found = false;
                return found;
            }
            directory = root;
            return found;
        }
        public static bool MoveToFile(string path, ref File_entry file_Entry)
        {
            bool found = false;
            List<string> pathList;
            pathList = createPathlist(path);
            string fileName = pathList[pathList.Count - 1];
            path = path.Substring(0, path.LastIndexOf("\\"));
            //path.Remove(path.LastIndexOf('\\'));
            Directory directory = new Directory();
            if (MoveToDir(path, ref directory))
            {
                directory.Read_Dir();
                int index = directory.SearchDirOrFiles(fileName);
                if (index != -1 && directory.DirsOrFiles[index].Dir_Attribute == 0x0)
                {
                    found = true;
                    file_Entry = new File_entry(fileName, directory.DirsOrFiles[index].Dir_Attribute, directory.DirsOrFiles[index].Dir_FirstCluster, directory, directory.DirsOrFiles[index].Dir_FileSize, null);
                }
                else
                {
                    //found = false;
                    return found;
                }
            }
            else
            {
                found = false;
                return found;
            }
            return found;
        }
        public static bool CreateFile(ref Directory directory, string fileName, string content)
        {
            File_entry file = new File_entry(fileName, 0x0, 0, directory, content.Length, content);
            Directory_Entry directory_Entry;
            directory_Entry = file.GetMyDirectory_Entry();//?return it where?
            if (directory.CanAddEntery(directory_Entry))
            {
                directory.AddEntry(directory_Entry);
                file.WriteFileContent();
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int OverWriteFile(ref Directory directory, string fileName, string content)
        {
            int[] errorType = { 0, -1, 1 };//0 -> file not exist -1 -> can not overwrite it 1 ->file exist and we can overWrite it
            int index = directory.SearchDirOrFiles(fileName);
            if (index != -1)
            {
                File_entry oldFile = new File_entry(fileName, 0x0, directory.DirsOrFiles[index].Dir_FirstCluster, directory, directory.DirsOrFiles[index].Dir_FileSize, content);
                oldFile.ReadFileContent();
                oldFile.DeleteFile();
                File_entry newFile = new File_entry(fileName, 0x0, 0, directory, content.Length, content);
                Directory_Entry directory_Entry;
                directory_Entry = newFile.GetMyDirectory_Entry();//?return it where?
                if (directory.CanAddEntery(directory_Entry))
                {
                    directory.AddEntry(directory_Entry);
                    newFile.WriteFileContent();
                    return errorType[2];
                }
                else
                {
                    Directory_Entry oldDirectory_Entry;
                    oldDirectory_Entry = oldFile.GetMyDirectory_Entry();
                    directory.AddEntry(oldDirectory_Entry);
                    oldFile.WriteFileContent();
                    return errorType[1];
                }
            }
            else
            {
                return errorType[0];
            }
        }
        public static List<string> createPathlist(string ls)
        {
            List<string> pathList = new List<string>();
            string[] part;
            part = ls.Split('\\');
            foreach (string s in part)
            {
                pathList.Add(s);
            }
            return pathList;
        }
    }
}
