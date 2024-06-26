﻿global using ID = ulong;

using System.Reflection;
using System.Text;


namespace ChatServer {
    public static class Logger {
        public enum Severity {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Message = 4,
        }

        private static void SaveMessageToFile(string message, Severity severity) {
            StringBuilder builder = new();
            builder.Append('[');
            switch (severity) {
                case Severity.None: {
                    builder.Append("NONE");
                } break;
                case Severity.Info: {
                    builder.Append("INFO");
                } break;
                case Severity.Warning: {
                    builder.Append("WARNING");
                } break;
                case Severity.Error: {
                    builder.Append("ERROR");
                } break;
                case Severity.Message: {
                    builder.Append("MESSAGE");
                } break;
            }
            builder.Append(']');
            builder.Append($"[{DateTime.Now}] {message}\n");

            const string _logFilePath = "OnlineChat.log.txt";
            File.AppendAllText(_logFilePath, builder.ToString());
        }

        private static void PrintMessage(string message, Severity severity) {
            Console.Write($"[{DateTime.Now}][");
            switch (severity) {
                case Severity.None: {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("NONE");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]   ");
                } break;
                case Severity.Info: {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("INFO");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]   ");
                } break;
                case Severity.Warning: {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARNING");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]");
                } break;
                case Severity.Error: {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]  ");
                } break;
                case Severity.Message: {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("MESSAGE");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("]");
                } break;
            }
            Console.Write($" {message}\n");
        }

        public static void Info(string message) {
            const Severity severity = Severity.Info;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }

        public static void Warning(string message) {
            const Severity severity = Severity.Warning;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }

        public static void Error(string message) {
            const Severity severity = Severity.Error;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }

        public static void Error(string message, MethodBase? method, string indent = "\n\t\t\t           ") {
            message = message.Replace("\n", indent);
            if (method != null) {
                Error($"[{method.Name}]:{indent}{message}");
            }
            else {
                Error(message);
            }
        }

        public static void Error(Exception ex, MethodBase? method, string indent = "\n\t\t\t           ") {
            string message = ex.Message.Replace("\n", indent);
            if (method != null) {
                Error($"[{method.Name}] ({ex.GetType().Name}):{indent}{message}");
            }
            else {
                Error($"({ex.GetType().Name}):{indent}{message}");
            }
        }

        public static void Message(string message) {
            const Severity severity = Severity.Message;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }
    }
}
