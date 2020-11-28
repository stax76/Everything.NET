
using System;

namespace EverythingFrontend
{
    class Item
    {
        public Item(uint index)
        {
            Index = index;
        }

        string NameValue;

        public string Name {
            get {
                if (!WasInitialized)
                    Init();
                return NameValue;
            }
        }

        string DirectoryValue;

        public string Directory {
            get {
                if (!WasInitialized)
                    Init();
                return DirectoryValue;
            }
        }

        long SizeValue;

        public long Size {
            get {
                if (!WasInitialized)
                    Init();
                return SizeValue;
            }
        }

        DateTime DateValue;

        public DateTime Date {
            get {
                if (!WasInitialized)
                    Init();
                return DateValue;
            }
        }

        public uint Index { get; }

        bool WasInitialized;

        void Init()
        {
            Model.Init(Index, ref NameValue, ref DirectoryValue, ref SizeValue, ref DateValue);
            WasInitialized = true;
        }
    }
}
