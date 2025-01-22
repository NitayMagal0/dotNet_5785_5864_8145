using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BO;

namespace PL.Call
{
    public partial class CallAssignInListWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<CallAssignInList> _callAssignList;

        public ObservableCollection<CallAssignInList> CallAssignList
        {
            get => _callAssignList;
            set
            {
                _callAssignList = value;
                OnPropertyChanged(nameof(CallAssignList));
            }
        }

        private string _callIdTxt;
        public string CallIdTxt
        {
            get => _callIdTxt;
            set
            {
                _callIdTxt = value;
                OnPropertyChanged(nameof(CallIdTxt));
            }
        }

        public CallAssignInListWindow(int callId, List<CallAssignInList> callAssigns)
        {
            InitializeComponent();
            DataContext = this;

            CallIdTxt = $"Call ID: {callId}";
            CallAssignList = new ObservableCollection<CallAssignInList>(callAssigns);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}