
using System;

namespace EverythingNET
{
    class Item
    {
        public uint Index { get; set; }
        public long Size { get; set; }
        public bool WasInitialized { get; set; }

        string _Name;

        public string Name {
            get {
                if (!WasInitialized)
                    Model.Init(this);
                return _Name;
            }
            set => _Name = value;
        }

        string _Directory;

        public string Directory {
            get {
                if (!WasInitialized)
                    Model.Init(this);
                return _Directory;
            }
            set => _Directory = value;
        }


        DateTime _Date;

        public DateTime Date {
            get {
                if (!WasInitialized)
                    Model.Init(this);
                return _Date;
            }
            set => _Date = value;
        }
    }
}
