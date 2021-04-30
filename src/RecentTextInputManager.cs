
using System.Collections.Generic;

namespace EverythingNET
{
    class RecentTextInputManager
    {
        public List<string> Items { get; set; }

        public RecentTextInputManager(IEnumerable<string> items)
        {
            Items = new List<string>(items);
        }

        public void Push(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (Items.Count > 0)
            {
                if (Items.Contains(value))
                    Items.Remove(value);

                string test = value.Substring(0, value.Length - 1);
                string first = Items[0];

                if (test == first)
                    Items.RemoveAt(0);

                if (first.StartsWith(value) || first.EndsWith(value))
                    return;
            }

            Items.Insert(0, value);
        }
    }
}
