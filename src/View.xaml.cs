
using System.Windows;

namespace EverythingFrontend
{
    public partial class View : Window
    {
        public View()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }
}
