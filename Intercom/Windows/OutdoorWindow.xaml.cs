using Intercom.Application;
using Intercom.Constants;
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
    /// Логика взаимодействия для OutdoorWindow.xaml
    /// </summary>
    public partial class OutdoorWindow : Window
    {
        private readonly IntercomController intercom;
        private readonly AudioSpeaker audioSpeaker;
        
        public OutdoorWindow(IntercomController intercom, AudioSpeaker audioSpeaker)
        {
            this.intercom = intercom;
            this.audioSpeaker = audioSpeaker;
            
            InitializeComponent();

            intercom.AddCallHandlers(
                onStart: StartCallToApartment,
                everySecond: UpdateCallToApartmentMessage,
                atTheEnd: StopCallToApartment);

            intercom.AddLockHandlers(
                onStart: UnlockDoor,
                everySecond: UpdateUnlockDoorMessage,
                atTheEnd: CloseDoor);

            intercom.AddErrorHandlers(
                onStart: SetErrorState,
                everySecond: DoNothing,
                atTheEnd: SetNotErrorState);

            intercom.AddCommunicationHandlers(
                onStart: StartCommunication,
                everySecond: UpdateCommunication,
                atTheEnd: StopCommunication);
        }

        private void SetUiState(
            string inputMessage,
            string statusMessage,
            Color color)
        {
            CodeInputTxt.Text = inputMessage;
            StatusMessage.Content = statusMessage;
            Bulb.Fill = new SolidColorBrush(color);
        }

        private bool IsInputFull()
        {
            return CodeInputTxt.Text.Length >= 4;
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var key = (sender as Button)!.Content as string;

            if(!IsInputFull() && !intercom.IsRunning)
            {
                CodeInputTxt.Text += key;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if(!intercom.IsRunning)
            {
                CodeInputTxt.Text = string.Empty;
            }
            else
            {
                intercom.StopAll();
            }
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            if(intercom.IsRunning)
            {
                return;
            }

            _ = int.TryParse(CodeInputTxt.Text, out int code);

            var connection = intercom.CallToApartment(code);

            if(!connection.IsSuccess)
            {
                intercom.SetError(connection.Message);
            }
        }

        private void UseKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if(intercom.IsRunning)
            {
                return;
            }

            KeyInputWindow keyInput = new KeyInputWindow(intercom);

            keyInput.ShowDialog();

            if(keyInput.IsSuccess)
            {
                _ = int.TryParse(keyInput.Key, out var key);

                if(intercom.AccessService.IsKeyValid(key))
                {
                    intercom.UnlockDoor();
                }
                else
                {
                    intercom.SetError();
                }
            }
        }

        #region Timer Handlers

        private void DoNothing(int tick) { }

        private void StartCallToApartment()
        {
            SetUiState(InputMessages.Calling, $"{StatusMessages.Calling}. Осталось {Timeouts.Connection + 1} секунд...", Colors.Yellow);
            audioSpeaker.PlayCallApartmentSound();
        }

        private void UpdateCallToApartmentMessage(int second)
        {
            SetUiState(InputMessages.Calling, $"{StatusMessages.Calling}. Осталось {Timeouts.Connection - second} секунд...", Colors.Yellow);
        }

        private void UpdateUnlockDoorMessage(int second)
        {
            SetUiState(InputMessages.Open, $"{StatusMessages.Unlocked}. Осталось {Timeouts.Unlocked - second} секунд...", Colors.Green);
        }

        private void StopCallToApartment()
        {
            audioSpeaker.StopPlayCallApartmentSound();
            SetUiState(InputMessages.Empty, StatusMessages.Wait, Colors.DarkGray);
        }

        private void SetErrorState()
        {
            SetUiState(InputMessages.Error, intercom.CurrentStatus?.Message ?? StatusMessages.Error, Colors.Red);
        }

        private void SetNotErrorState()
        {
            SetUiState(InputMessages.Empty, StatusMessages.Wait, Colors.DarkGray);
        }

        private void UnlockDoor()
        {
            SetUiState(InputMessages.Open, $"{StatusMessages.Unlocked}. Осталось {Timeouts.Unlocked + 1} секунд...", Colors.Green);
            audioSpeaker.PlayOpenDoorSound();
        }

        private void CloseDoor()
        {
            audioSpeaker.StopPlayOpenDoorSound();
            SetUiState(InputMessages.Empty, StatusMessages.Wait, Colors.DarkGray);
        }

        private void StopCommunication()
        {
            SetUiState(InputMessages.Empty, StatusMessages.Wait, Colors.DarkGray);
        }

        private void UpdateCommunication(int tick)
        {
            SetUiState(InputMessages.Speak, $"{StatusMessages.Speaking}. Осталось {Timeouts.Communication - tick} секунд...", Colors.Green);
        }

        private void StartCommunication()
        {
            SetUiState(InputMessages.Speak, $"{StatusMessages.Speaking}. Осталось {Timeouts.Communication + 1} секунд..." , Colors.Green);
        }

        #endregion

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow(intercom);

            settings.ShowDialog();
        }
    }
}
