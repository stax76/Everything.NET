
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace EverythingFrontend
{
    class ViewModel : ViewModelBase
    {
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        string SearchTextValue;

        public string SearchText {
            get => SearchTextValue;
            set {
                SearchTextValue = value;
                Update();
            }
        }

        void Update()
        {
            Items.Clear();

            foreach (var item in Model.GetItems(SearchText))
                Items.Add(item);
        }
    }
}
