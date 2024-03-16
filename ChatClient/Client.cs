using ChatClient.Network;
using ChatSharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient
{
    public static class Client
    {
        public readonly static Config.Config Config;

        public static readonly ServerConnection ServerConnection;

        static Client()
        {
            Config = new();
            ServerConnection = new();
        }

        public static void Connect()
        {
            ServerConnection.Connect();
        }

        public static void LogIn(string email, string password)
        {
            var loginData = new LoginData(email, password);
            string json = JsonSerializer.Serialize(loginData);
            ServerConnection.Send(OperationCode.LogIn, json);
        }

        public static void Register(string email, string password, string confirmPassword, string nickname)
        {
            var registerData = new RegisterData(email, password, confirmPassword, nickname);
            string json = JsonSerializer.Serialize(registerData);
            ServerConnection.Send(OperationCode.Register, json);
        }
    }
}
