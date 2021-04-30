
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using static EverythingNET.EverythingNative;

namespace EverythingNET
{
    class Everything
    {
        public static List<EverythingItem> GetItems(string searchText)
        {
            List<EverythingItem> items = new List<EverythingItem>();

            if (searchText.Length < 2)
                return items;

            Everything_SetSearch(searchText);
         
            Everything_SetRequestFlags(
                EVERYTHING_REQUEST_FILE_NAME |
                EVERYTHING_REQUEST_PATH |
                EVERYTHING_REQUEST_DATE_MODIFIED | 
                EVERYTHING_REQUEST_SIZE);

            Everything_SetMatchPath(true);
          
            if (!Everything_Query(true))
            {
                string[] errorCodes = {
                    "EVERYTHING_OK",
                    "EVERYTHING_ERROR_MEMORY",
                    "EVERYTHING_ERROR_IPC",
                    "EVERYTHING_ERROR_REGISTERCLASSEX",
                    "EVERYTHING_ERROR_CREATEWINDOW",
                    "EVERYTHING_ERROR_CREATETHREAD",
                    "EVERYTHING_ERROR_INVALIDINDEX",
                    "EVERYTHING_ERROR_INVALIDCALL"
                };

                uint lastError = Everything_GetLastError();

                if (lastError > 0 && lastError < errorCodes.Length)
                    throw new Exception(errorCodes[lastError]);
                else
                    throw new Exception("Unknown Error");
            }

            uint count = Everything_GetNumResults();

            for (uint i = 0; i < count; i++)
            {
                Everything_GetResultSize(i, out var size);

                if (size > -1)
                    items.Add(new EverythingItem() { Index = i, Size = size });
            }

            return items;
        }

        public static void InitItem(EverythingItem item)
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
    }

    class EverythingItem
    {
        public uint Index { get; set; }
        public long Size { get; set; }
        public bool WasInitialized { get; set; }

        string _Name;

        public string Name {
            get {
                if (!WasInitialized)
                    Everything.InitItem(this);
                return _Name;
            }
            set => _Name = value;
        }

        string _Directory;

        public string Directory {
            get {
                if (!WasInitialized)
                    Everything.InitItem(this);
                return _Directory;
            }
            set => _Directory = value;
        }

        DateTime _Date;

        public DateTime Date {
            get {
                if (!WasInitialized)
                    Everything.InitItem(this);
                return _Date;
            }
            set => _Date = value;
        }
    }

    class EverythingNative
    {
        public const int EVERYTHING_REQUEST_FILE_NAME     = 0x00000001;
        public const int EVERYTHING_REQUEST_PATH          = 0x00000002;
        public const int EVERYTHING_REQUEST_SIZE          = 0x00000010;
        public const int EVERYTHING_REQUEST_DATE_MODIFIED = 0x00000040;

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_SetSearch(string lpSearchString);

        [DllImport("Everything.dll")]
        public static extern void Everything_SetRequestFlags(uint dwRequestFlags);

        [DllImport("Everything.dll")]
        public static extern void Everything_SetSort(uint dwSortType);

        [DllImport("Everything.dll")]
        public static extern bool Everything_Query(bool bWait);

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathName(uint nIndex, StringBuilder lpString, uint nMaxCount);

        [DllImport("Everything.dll")]
        public static extern bool Everything_GetResultDateModified(uint nIndex, out long lpFileTime);

        [DllImport("Everything.dll")]
        public static extern bool Everything_GetResultSize(uint nIndex, out long lpFileSize);

        [DllImport("Everything.dll")]
        public static extern uint Everything_GetNumResults();

        [DllImport("Everything.dll")]
        public static extern void Everything_SetMatchPath(bool bEnable);

        [DllImport("Everything.dll")]
        public static extern uint Everything_GetLastError();
    }
}
