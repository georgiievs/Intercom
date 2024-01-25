using Intercom.Application;
using Intercom.Models;
using Microsoft.EntityFrameworkCore;
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

namespace Intercom.Windows
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly IntercomController intercom;

        public SettingsWindow(IntercomController intercom)
        {
            this.intercom = intercom;

            InitializeComponent();          
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            var apartment = ApartmentsTable.SelectedItem as Apartment;

            if(apartment == null)
            {
                MessageBox.Show("Квартира не выбрана!");
            }
            else
            {
               IndorWindow IndorPanelWindow = new IndorWindow(intercom, apartment.Code);
            
               IndorPanelWindow.Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApartmentsTable.ItemsSource = intercom.Database.Apartments.OrderBy(x => x.Code).ToList();
            KeysTable.ItemsSource = intercom.Database.Keys.OrderBy(x => x.Id).ToList();
            ApartmentsTable.SelectedIndex = 0;
            KeysTable.SelectedIndex = 0;    
        }
    }
}
