
using Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EverythingNET
{
    public partial class View : Window
    {
        public View()
        {
            InitializeComponent();
            DataContext = new ViewModel();
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

        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid.SelectedItem != null)
            {
                Item item = grid.SelectedItem as Item;
                string file = Path.Combine(item.Directory, item.Name);

                if (File.Exists(file))
                {
                    ShellContextMenu menu = new ShellContextMenu();
                    FileInfo[] files = { new FileInfo(file) };
                    IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                    Point screenPos = PointToScreen(Mouse.GetPosition(this));
                    System.Drawing.Point screenPos2 = new System.Drawing.Point((int)screenPos.X, (int)screenPos.Y);
                    menu.ShowContextMenu(handle, files, screenPos2);
                }
            }
        }
    }
}
