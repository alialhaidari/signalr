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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection connection;
        HubConnection hubConnection;
        string _messageRecived;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await InizializzaConnectioTuBlazorHub();
        }
        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            await InizializzaConnectioTuBlazorHub();
        }
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await Send();
        }
        async Task InizializzaConnectioTuBlazorHub()
        {
            try
            {
                Check(false);
                hubConnection = new HubConnectionBuilder()
                      .WithUrl(txtServer.Text)
                      .WithAutomaticReconnect()
                      .Build();

                hubConnection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };
                hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    _messageRecived = user + " - " + message;
                    messagesList.Items.Add(_messageRecived);
                });
                await hubConnection.StartAsync();
                Check(true);
            }
            catch (Exception ex)
            {
                IsEnabled = true;
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        void Check(bool status)
        {
            IsEnabled = status;
            txtUser.IsEnabled = status;
            txtMessage.IsEnabled = status;
            btnSend.IsEnabled = status;
            messagesList.IsEnabled = status;
        }
        async Task Send()
        {
            try
            {
                IsEnabled = false;
                if (hubConnection != null)
                    await hubConnection.SendAsync("SendMessage", txtUser.Text, txtMessage.Text);
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                IsEnabled = true;
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
