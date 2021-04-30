
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Shell;

namespace EverythingNET
{
    public partial class View : Window
    {
        MainViewModel ViewModel;

        public View()
        {
            ShellDarkMode.BeforeWindowCreation();
            InitializeComponent();
            ShellDarkMode.AfterWindowCreation(new WindowInteropHelper(this).Handle);
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
        }

        void DataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) & !(dep is DataGridRow))
                dep = VisualTreeHelper.GetParent(dep);

            DataGridRow row = dep as DataGridRow;
          
            if (row != null)
                row.IsSelected = true;
        }

        void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowMenu(PointToScreen(Mouse.GetPosition(this)));
        }

        void ShowMenu(Point screenPos)
        {
            if (MainDataGrid.SelectedItem != null)
            {
                EverythingItem item = MainDataGrid.SelectedItem as EverythingItem;
                string file = Path.Combine(item.Directory, item.Name);

                if (File.Exists(file))
                {
                    ShellContextMenu menu = new ShellContextMenu();
                    FileInfo[] files = { new FileInfo(file) };
                    IntPtr handle = new WindowInteropHelper(this).Handle;
                    System.Drawing.Point screenPos2 = new System.Drawing.Point((int)screenPos.X, (int)screenPos.Y);
                    menu.ShowContextMenu(handle, files, screenPos2);
                    Task.Run(() => {
                        Thread.Sleep(2000);
                        ViewModel.Update();
                    });
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (SearchTextBox.Text != "")
                    SearchTextBox.Text = "";
                else
                    Close();
            }
        }

        void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (MainDataGrid.Items.Count > 0)
            {
                if (e.Key == Key.Up)
                {
                    int index = MainDataGrid.SelectedIndex;
                    index--;

                    if (index < 0)
                        index = 0;

                    MainDataGrid.SelectedIndex = index;
                }

                if (e.Key == Key.Down)
                {
                    int index = MainDataGrid.SelectedIndex;
                    index++;

                    if (index > MainDataGrid.Items.Count - 1)
                        index = MainDataGrid.Items.Count - 1;

                    MainDataGrid.SelectedIndex = index;
                }
            }

            if (e.Key == Key.Apps)
                ShowMenu(PointToScreen(new Point(0d, 0d)));
        }

        void Window_Activated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.SearchText))
                ViewModel.Update();
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NameColumn.Width = ActualWidth * 0.25;
            DirectoryColumn.Width = ActualWidth * 0.49;
        }

        void Window_Closed(object sender, EventArgs e)
        {
            App.Settings.RecentSearches = ViewModel.RecentSearchManager.Items.Take(10).ToArray();
            SettingsManager.Save(App.Settings);
        }

        void SearchTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            List<string> l = new List<string>();
            string txt = Clipboard.GetText();

            if (!string.IsNullOrEmpty(txt) && !txt.Contains("\n"))
                l.Add(txt);

            l.AddRange(ViewModel.RecentSearchManager.Items);
            InputContextMenu.ItemsSource = l.Take(10);
        }

        void MainMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainContextMenu.IsOpen = true;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var mod = e.KeyboardDevice.Modifiers;

            if (e.Key == Key.F2 && mod == ModifierKeys.None)
            {
                var cm = MainContextMenu;
                cm.Placement = PlacementMode.Bottom;
                cm.PlacementTarget = MainMenuTextBlock;
                cm.IsOpen = true;
            }
        }
    }
}
