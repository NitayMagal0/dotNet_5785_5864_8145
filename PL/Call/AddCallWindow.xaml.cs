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
    public partial class AddCallWindow : Window, INotifyPropertyChanged
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private BO.Call _newCall;
        public BO.Call NewCall
        {
            get => _newCall;
            set
            {
                _newCall = value;
                OnPropertyChanged(nameof(NewCall));
            }
        }

        public ObservableCollection<BO.CallType> CallTypes { get; set; }
        public ObservableCollection<BO.CallStatus> StatusTypes { get; set; }

        public AddCallWindow()
        {
            InitializeComponent();
            DataContext = this;

            NewCall = new BO.Call();
            LoadCallTypes();
            LoadCallStatuses();
        }

        private void LoadCallTypes()
        {
            CallTypes = new ObservableCollection<BO.CallType>((BO.CallType[])Enum.GetValues(typeof(BO.CallType)));
            OnPropertyChanged(nameof(CallTypes));
        }

        private void LoadCallStatuses()
        {
            StatusTypes = new ObservableCollection<BO.CallStatus>((BO.CallStatus[])Enum.GetValues(typeof(BO.CallStatus)));
            OnPropertyChanged(nameof(StatusTypes));
        }

        private void BtnAddCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the Add function (all validations happen inside MinAddCall)
                s_bl.Call.MinAddCall(NewCall.CallType, NewCall.Description, NewCall.FullAddress, NewCall.MaxCompletionTime);

                MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (BlInvalidFormatException ex)
            {
                MessageBox.Show($"Input Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
