using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.MVVM.ViewModel
{
    partial class MainViewModel
    {

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }


        public RelayCommand ConnectToServerCommand { get; set;  }
        public RelayCommand SendMessageCommand { get; set; }

        public string Username { get; set; }
        public string Message { get; set; }

        //private Server server;

        public MainViewModel()
        {

            //Server = new();
            Users = new();
            Messages = new();
            Server.connectedEvent += UserConnected;
            Server.msgReceivedEvent += MessageReceived;
            Server.userDisconnectedEvent += UserDisconnected;
            ConnectToServerCommand = new(x => Server.ConnectToServer(Username), x => !string.IsNullOrWhiteSpace(Username));
            SendMessageCommand = new(x => Server.SendMessageToServer(Message));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = Server.PacketReader.ReadMessage(),
                UserID = Server.PacketReader.ReadMessage(),
            };

            MessageBox.Show(user.Username + " " + user.UserID);

            if (!Users.Any(x => x.UserID == user.UserID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user)); 
            }
        }

        private void MessageReceived()
        {
            var msg = Server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserDisconnected()
        {
            var uid = Server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UserID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }
}
