using Microsoft.AspNetCore.SignalR.Client;
namespace SignalRClient
{
    public partial class Main : Form
    {
        private HubConnection connection;
        HubConnection hubConnection;
        string _messageRecived;
        public Main()
        {
            InitializeComponent();
        }
        private async void Main_Load(object sender, EventArgs e)
        {
            await InizializzaConnectioTuBlazorHub();
        }
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            await InizializzaConnectioTuBlazorHub();
        }
        private async void btnSend_Click(object sender, EventArgs e)
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
                Enabled = true;
                MessageBox.Show(ex.Message,"error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

        }
        void Check(bool status)
        {
            Enabled = status;
            txtUser.Enabled = status;
            txtMessage.Enabled = status;
            btnSend.Enabled = status;
            messagesList.Enabled = status;
            if (status)
                messagesList.BackColor = Color.White;
            else
                messagesList.BackColor = SystemColors.Control;
        }
        async Task Send()
        {
            try
            {
                Enabled = false;
                if (hubConnection != null)
                    await hubConnection.SendAsync("SendMessage", txtUser.Text, txtMessage.Text);
                Enabled = true;
            }
            catch (Exception ex)
            {
                Enabled = true;
                MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}