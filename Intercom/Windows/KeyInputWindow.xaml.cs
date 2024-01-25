using Intercom.Application;
using Intercom.Windows;
using System;
using System.Collections.Generic;
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

namespace Intercom
{
    /// <summary>
    /// Логика взаимодействия для KeyInputWindow.xaml
    /// </summary>
    public partial class KeyInputWindow : Window
    {
        private readonly IntercomController intercom;

        public string Key { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;

        public KeyInputWindow(IntercomController intercom)
        {
            this.intercom = intercom;

            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Key = KeyTxt.Text;
            IsSuccess = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSuccess = false;
            Close();
        }

        private void ShowKeys_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new(intercom);
            window.ShowDialog();
        }
    }
}
