using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectOSclean
{
    public class Directory_Entry
    {
        public string Dir_Name;
        //public char[] Dir_Name = new char[11];
        public byte Dir_Attribute;
        public byte[] Dir_Empty = new byte[12];
        public int Dir_FirstCluster, Dir_FileSize;
        public Directory_Entry()
        {

        }
        public Directory_Entry(string name, byte attr, int FC, int fileSize = 0)
        {
            Dir_Name = name;
            Dir_Attribute = attr;
            Dir_FirstCluster = FC;
            Dir_FileSize = fileSize;
        }
    }
}
