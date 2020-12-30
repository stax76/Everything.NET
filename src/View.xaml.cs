
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Shell;

namespace EverythingNET
{
    public partial class View : Window
    {
        ViewModel ViewModel;

        public View()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            DataContext = ViewModel;
        }

        void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            
            if (grid.SelectedItem != null)
            {
                Item item = grid.SelectedItem as Item;
                Process.Start("explorer.exe", "/n, /select, \"" + Path.Combine(item.Directory, item.Name) + "\"");
            }
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
            if (DG.SelectedItem != null)
            {
                Item item = DG.SelectedItem as Item;
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
                Close();

            if (e.Key == Key.F1)
            {
                using (var proc = Process.GetCurrentProcess())
                {
                    string txt = "Everything.NET\n\nCopyright (C) 2020 Frank Skare (stax76)\n\nVersion " +
                        FileVersionInfo.GetVersionInfo(proc.MainModule.FileName).FileVersion.ToString() +
                        "\n\n" + "MIT License";

                    MessageBox.Show(txt);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!string.IsNullOrEmpty(ViewModel.SearchText))
                RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "LastText", ViewModel.SearchText);
        }

        void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && SearchTextBox.Text == "")
            {
                string last = RegistryHelp.GetString(RegistryHelp.ApplicationKey, "LastText");

                if (!string.IsNullOrEmpty(last))
                {
                    SearchTextBox.Text = last;
                    SearchTextBox.CaretIndex = 1000;
                }
            }

            if (DG.Items.Count > 0)
            {
                if (e.Key == Key.Up)
                {
                    int index = DG.SelectedIndex;
                    index--;

                    if (index < 0)
                        index = 0;

                    DG.SelectedIndex = index;
                }

                if (e.Key == Key.Down)
                {
                    int index = DG.SelectedIndex;
                    index++;

                    if (index > DG.Items.Count - 1)
                        index = DG.Items.Count - 1;

                    DG.SelectedIndex = index;
                }
            }

            if (e.Key == Key.Apps)
            {
                Application.Current.Dispatcher.InvokeAsync(() => {
                    ShowMenu(PointToScreen(new Point(0d, 0d)));
                });
            }
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x104 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                Application.Current.Dispatcher.InvokeAsync(() => {
                    ShowMenu(PointToScreen(new Point(0d, 0d)));
                });
            }

            return IntPtr.Zero;
        }

        void Window_Activated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.SearchText))
                ViewModel.Update();
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NameColumn.Width = ActualWidth * 0.25;
            DirectoryColumn.Width = ActualWidth * 0.5;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }
    }
}
