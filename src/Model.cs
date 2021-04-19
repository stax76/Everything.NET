
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EverythingNET
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
                EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME |
                EVERYTHING_REQUEST_DATE_MODIFIED | 
                EVERYTHING_REQUEST_SIZE);

            Everything_SetMatchPath(true);
            Everything_Query(true);
            uint count = Everything_GetNumResults();

            for (uint i = 0; i < count; i++)
            {
                Everything_GetResultSize(i, out var size);

                if (size > -1)
                    items.Add(new Item() { Index = i, Size = size });
            }

            return items;
        }

        public static void Init(Item item)
        {
            StringBuilder sb = new StringBuilder(500);
            Everything_GetResultFullPathName(item.Index, sb, (uint)sb.Capacity);
            string path = sb.ToString();
            Everything_GetResultDateModified(item.Index, out var fileTime);
            item.Directory = Path.GetDirectoryName(path);
            item.Name = Path.GetFileName(path);

            if (fileTime != -1)
                item.Date = DateTime.FromFileTime(fileTime);

            item.WasInitialized = true;
        }

        const int EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME = 0x00000004;
        const int EVERYTHING_REQUEST_SIZE                    = 0x00000010;
        const int EVERYTHING_REQUEST_DATE_MODIFIED           = 0x00000040;

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        static extern int Everything_SetSearch(string lpSearchString);

        [DllImport("Everything.dll")]
        static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);

        [DllImport("Everything.dll")]
        static extern void Everything_SetSort(UInt32 dwSortType);

        [DllImport("Everything.dll")]
        static extern bool Everything_IsFileResult(UInt32 index);

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

        [DllImport("Everything.dll")]
        static extern void Everything_SetMatchPath(bool bEnable);
    }
}
