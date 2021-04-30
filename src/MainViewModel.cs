
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace EverythingNET
{
    class MainViewModel : ViewModelBase
    {
        public RecentTextInputManager RecentSearchManager { get; set; }
        TypeAssistant TypeAssistant;

        public Command ShowAboutCommand { get; }
        public Command ShowHelpCommand { get; }
        public Command ShowInExplorerCommand { get; }
        public Command SearchTextBoxMenuCommand { get; }

        public MainViewModel()
        {
            TypeAssistant = new TypeAssistant();
            TypeAssistant.Idled += TypeAssistant_Idled;

            RecentSearchManager = new RecentTextInputManager(App.Settings.RecentSearches);

            ShowAboutCommand = new Command(ShowAbout);
            ShowHelpCommand = new Command(ShowHelp);
            ShowInExplorerCommand = new Command(ShowInExplorer);
            SearchTextBoxMenuCommand = new Command(SearchTextBoxMenuCommandHandler);
        }

        private EverythingItem _SelectedItem;

        public EverythingItem SelectedItem {
            get => _SelectedItem;
            set {
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }

        private List<EverythingItem> _Items;

        public List<EverythingItem> Items {
            get => _Items;
            set {
                _Items = value;
                OnPropertyChanged();
            }
        }

        string _SearchText;

        public string SearchText {
            get => _SearchText;
            set {
                _SearchText = value;
                OnPropertyChanged();
                TypeAssistant.TextChanged(value);
                RecentSearchManager.Push(value);
            }
        }

        void TypeAssistant_Idled(string text)
        {
            Update();
        }

        public void Update()
        {
            List<EverythingItem> items = Everything.GetItems(SearchText);

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

        void SearchTextBoxMenuCommandHandler(object param)
        {
            SearchText = param.ToString();
        }

        void ShowHelp(object param)
        {
            var info = new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "https://github.com/stax76/Everything.NET#usage"
            };
            Process.Start(info).Dispose();
        }

        void ShowAbout(object param)
        {
            using var proc = Process.GetCurrentProcess();

            string txt = "Everything.NET\n\nCopyright (C) 2020-2021 Frank Skare (stax76)\n\nVersion " +
                FileVersionInfo.GetVersionInfo(proc.MainModule.FileName).FileVersion.ToString() +
                "\n\n" + "MIT License";

            MessageBox.Show(txt);
        }
    }
}
