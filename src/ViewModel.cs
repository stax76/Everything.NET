
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace EverythingFrontend
{
    class ViewModel : ViewModelBase
    {
        TypeAssistant TypeAssistant;

        public ViewModel()
        {
            TypeAssistant = new TypeAssistant();
            TypeAssistant.Idled += TypeAssistant_Idled;
            Task.Run(() => Update(""));
        }

        private List<Item> ItemsValue;

        public List<Item> Items {
            get => ItemsValue;
            set {
                ItemsValue = value;
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

        void TypeAssistant_Idled(string text) => Update(text);

        void Update(string text)
        {
            List<Item> items = Model.GetItems(text);
            Application.Current.Dispatcher.Invoke(() => Items = items);
        }
    }
}
