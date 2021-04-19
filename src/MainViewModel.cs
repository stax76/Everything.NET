
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace EverythingNET
{
    class MainViewModel : ViewModelBase
    {
        public Command ShowInExplorerCommand { get; }
        public Command ShowHelpCommand { get; }
        public Command ShowContextMenuCommand { get; }

        TypeAssistant TypeAssistant;

        public MainViewModel()
        {
            TypeAssistant = new TypeAssistant();
            TypeAssistant.Idled += TypeAssistant_Idled;

            ShowInExplorerCommand = new Command(ShowInExplorer);
            ShowHelpCommand = new Command(ShowHelp);
            ShowContextMenuCommand = new Command(ShowContextMenu);
        }

        private Item _SelectedItem;

        public Item SelectedItem {
            get => _SelectedItem;
            set {
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }

        private List<Item> _Items;

        public List<Item> Items {
            get => _Items;
            set {
                _Items = value;
                OnPropertyChanged();
            }
        }

        string SearchTextValue;

        public string SearchText {
            get => SearchTextValue;
            set {
                SearchTextValue = value;
                TypeAssistant.TextChanged(value);
            }
        }

        void TypeAssistant_Idled(string text)
        {
            Update();
        }

        public void Update()
        {
            List<Item> items = Model.GetItems(SearchText);

            Application.Current.Dispatcher.Invoke(() =>
            {
                string lastName = SelectedItem?.Name;
                string lastDirectory = SelectedItem?.Directory;

                Items = items;

                int count = Items.Count;

                if (count > 50)
                    count = 50;

                for (int i = 0; i < count; i++)
                    if (lastName == Items[i].Name && lastDirectory == Items[i].Directory)
                        SelectedItem = Items[i];
            });
        }

        void ShowInExplorer(object param)
        {
            if (SelectedItem == null)
                return;

            string path = Path.Combine(SelectedItem.Directory, SelectedItem.Name);
            Process.Start("explorer.exe", "/n, /select, \"" + path + "\"");
        }

        void ShowHelp(object param)
        {
            var info = new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "https://github.com/stax76/Everything.NET#usage"
            };
            Process.Start(info).Dispose();
        }

        void ShowContextMenu(object param)
        {
            var cm = (param as FrameworkElement).ContextMenu;
            cm.Placement = PlacementMode.Bottom;
            cm.PlacementTarget = param as UIElement;
            cm.IsOpen = true;
        }
    }
}
