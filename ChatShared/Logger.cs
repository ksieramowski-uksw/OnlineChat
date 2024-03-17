using System.Text;


namespace ChatServer {
    public static class Logger {
        public enum Severity {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
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
            }
            builder.Append(']');
            builder.Append($"[{DateTime.Now}] {message}\n");

            const string _logFilePath = "OnlineChat.log.txt";
            File.AppendAllText(_logFilePath, builder.ToString());
        }

        private static void PrintMessage(string message, Severity severity)
        {
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
            }
            Console.Write($" {message}\n");
        }

        public static void Info(string message)
        {
            const Severity severity = Severity.Info;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }

        public static void Warning(string message)
        {
            const Severity severity = Severity.Warning;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }

        public static void Error(string message)
        {
            const Severity severity = Severity.Error;
            SaveMessageToFile(message, severity);
            PrintMessage(message, severity);
        }
    }
}
