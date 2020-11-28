
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EverythingFrontend
{
    class Model
    {
        public static List<Item> GetItems(string searchText)
        {
            List<Item> items = new List<Item>();

            if (searchText.Length < 2)
                return items;

            Everything_SetSearch(searchText);
         
            Everything_SetRequestFlags(
                EVERYTHING_REQUEST_FILE_NAME |
                EVERYTHING_REQUEST_PATH | 
                EVERYTHING_REQUEST_DATE_MODIFIED | 
                EVERYTHING_REQUEST_SIZE);
       
            Everything_Query(true);
            uint count = Everything_GetNumResults();

            for (uint i = 0; i < count; i++)             
                items.Add(new Item(i));

            return items;
        }

        public static void Init(uint index, ref string name, ref string directory, ref long size, ref DateTime date)
        {
            StringBuilder sb = new StringBuilder(500);
            Everything_GetResultFullPathName(index, sb, (uint)sb.Capacity);
            string path = sb.ToString();
            Everything_GetResultSize(index, out var size2);
            Everything_GetResultDateModified(index, out var fileTime);
            directory = Path.GetDirectoryName(path);
            name = Path.GetFileName(path);
            size = size2;

            if (fileTime != -1)
                date = DateTime.FromFileTime(fileTime);
        }

        const int EVERYTHING_REQUEST_FILE_NAME     = 0x00000001;
        const int EVERYTHING_REQUEST_PATH          = 0x00000002;
        const int EVERYTHING_REQUEST_SIZE          = 0x00000010;
        const int EVERYTHING_REQUEST_DATE_MODIFIED = 0x00000040;

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        static extern int Everything_SetSearch(string lpSearchString);

        [DllImport("Everything.dll")]
        static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);

        [DllImport("Everything.dll")]
        static extern void Everything_SetSort(UInt32 dwSortType);

        [DllImport("Everything.dll")]
        static extern bool Everything_Query(bool bWait);

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        static extern void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);

        [DllImport("Everything.dll")]
        public static extern bool Everything_GetResultDateModified(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything.dll")]
        static extern bool Everything_GetResultSize(UInt32 nIndex, out long lpFileSize);

        [DllImport("Everything.dll")]
        static extern UInt32 Everything_GetNumResults();
    }
}
