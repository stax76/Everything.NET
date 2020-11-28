
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EverythingFrontend
{
    public partial class View : Window
    {
        public View()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            
            if (grid.SelectedItem != null)
            {
                Item item = grid.SelectedItem as Item;
                Process.Start("explorer.exe", "/n, /select, \"" + Path.Combine(item.Directory, item.Name) + "\"");
            }
        }
    }
}
