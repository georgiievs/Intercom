using Intercom.Application;
using Intercom.Constants;
using Intercom.Models;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Intercom.Windows
{
    /// <summary>
    /// Логика взаимодействия для IndorWindow.xaml
    /// </summary>
    public partial class IndorWindow : Window
    {
        private readonly IntercomController intercom;
        private Apartment apartment;

        public IndorWindow(IntercomController intercom, int apartmentCode)
        {
            this.intercom = intercom;

            InitializeComponent();

            intercom.AddCallHandlers(
                onStart: StartCalling,
                everySecond: UpdateCalling,
                atTheEnd: StopCalling);

            intercom.AddLockHandlers(
                onStart: OpenDoor,
                everySecond: UpdateOpenDoor,
                atTheEnd: CloseDoor);

            intercom.AddCommunicationHandlers(
                onStart: SubmitComminication,
                everySecond: UpdateCommunication,
                atTheEnd: StopCommunication);

            apartment = intercom.Database.Apartments
                .First(x => x.Code == apartmentCode);

            if(apartment.IsBlocked)
            {
                SetUiState("Заблокировано", Colors.Red, Colors.Red);           
            }
            else
            {
                SetUiState(Constants.StatusMessages.Wait, Colors.DarkGray, Colors.DarkGray);
            }
        }

        private void StopCommunication()
        {
            if(apartment.IsBlocked || intercom.CurrentTarget != apartment.Code)
            {
                return;
            }
            
            SetUiState(Constants.StatusMessages.Wait, Colors.DarkGray, Colors.DarkGray);
        }

        private void UpdateCommunication(int tick)
        {
            if(apartment.IsBlocked || intercom.CurrentTarget != apartment.Code)
            {
                return;
            }

            SetUiState($"{StatusMessages.Speaking}. Осталось {Timeouts.Communication - tick} секунд...", Colors.Green, Colors.Green);
        }

        private void SubmitComminication()
        {
            if(apartment.IsBlocked || intercom.CurrentTarget != apartment.Code)
            {
                return;
            }

            SetUiState($"{StatusMessages.Speaking}. Осталось {Timeouts.Communication + 1} секунд...", Colors.Green, Colors.Green);
        }

        private void CloseDoor()
        {
            if(apartment.IsBlocked)
            {
                return;
            }

            SetUiState(Constants.StatusMessages.Wait, Colors.DarkGray, Colors.DarkGray);
        }

        private void UpdateOpenDoor(int tick)
        {
            if(apartment.IsBlocked)
            {
                return;
            }

            SetUiState($"{StatusMessages.Unlocked}. Осталось {Timeouts.Unlocked - tick} секунд...", Colors.Green, Colors.DarkGray);
        }

        private void OpenDoor()
        {
            if(apartment.IsBlocked)
            {
                return;
            }

            SetUiState($"{StatusMessages.Unlocked}. Осталось {Timeouts.Unlocked + 1} секунд...", Colors.Green, Colors.DarkGray);
        }

        private void StopCalling()
        {
            if(!CanAcceptCalls())
            {
                return;
            }

            SetUiState(Constants.StatusMessages.Wait, Colors.DarkGray, Colors.DarkGray);
        }

        private void UpdateCalling(int tick)
        {
            if(!CanAcceptCalls())
            {
                return;
            }

            SetUiState($"{StatusMessages.Calling}. Осталось {Timeouts.Connection - tick} секунд...", Colors.Yellow, Colors.DarkGray);
        }

        private void StartCalling()
        {
            if (!CanAcceptCalls())
            {
                return;
            }

            SetUiState($"{StatusMessages.Calling}. Осталось {Timeouts.Connection + 1} секунд...", Colors.Yellow, Colors.DarkGray);
        }

        private bool CanAcceptCalls()
        {
            return !(apartment.IsBlocked || intercom.CurrentTarget != apartment.Code);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Code.Text = $"{apartment.Code}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SetUiState(string statusMessage, Color bulbColor, Color indicatorColor)
        {
            StatusMessage.Content = statusMessage;
            Bulb.Fill = new SolidColorBrush(bulbColor);
            Indicator.Background = new SolidColorBrush(indicatorColor);
        }

        private void OpenDoorButton_Click(object sender, RoutedEventArgs e)
        {
            intercom.UnlockDoor();
        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            if(apartment.IsBlocked)
            {
                intercom.AccessService.UnblockApartment(apartment.Code);
                SetUiState(Constants.StatusMessages.Wait, Colors.DarkGray, Colors.DarkGray);
            }
            else
            {
                intercom.AccessService.BlockApartment(apartment.Code);
                SetUiState("Заблокировано", Colors.Red, Colors.Red);

                if(intercom.IsRunning && intercom.CurrentTarget == apartment.Code)
                {
                    intercom.StopCallToApartment();
                }
            }
        }

        private void SubmitCallButton_Click(object sender, RoutedEventArgs e)
        {
            intercom.SubmitCommunication(apartment.Code);
        }
    }
}
