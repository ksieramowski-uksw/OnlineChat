using System.IO;
using System.Text.Json;
using System.Windows;


namespace ChatClient {
    public class Config {
        public ConfigData Current { get; private set; }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Config() {
            LoadConfig(ConfigData.DefaultFilePath);
        }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void LoadConfig(string filePath) {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) {
                var data = ConfigData.RestoreDefault();
                if (data == null) {
                    MessageBox.Show("Failed to restore default configration.");
                    return;
                }
                Current = data;
            }
            else {
                string json = File.ReadAllText(filePath);
                ConfigData? configData = JsonSerializer.Deserialize<ConfigData>(json);
                if (configData == null) {
                    MessageBox.Show("Failed to load configuration.");
                    return;
                }
                Current = configData;
            }
        }

        public class ConfigData {
            public const string DefaultFilePath = "Config.json";

            public string IPv4 { get; set; }
            public ushort Port { get; set; }
            public string EOM { get; set; }
            public int MessageBufferSize { get; set; }
            public bool DisposeMessagesOnGuildChanged { get; set; }

            #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public ConfigData() {}
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

            public static ConfigData? RestoreDefault() {
                ConfigData configData = new() {
                    IPv4 = "127.0.0.1",
                    Port = 21370,
                    EOM = "<|EOM|>",
                    MessageBufferSize = 10 * 1024 * 1024,
                    DisposeMessagesOnGuildChanged = false,
                };

                string json = JsonSerializer.Serialize(configData);

                string? directory = Path.GetDirectoryName(DefaultFilePath);
                if (directory == null) {
                    MessageBox.Show("Failed to create directory for configuration file.");
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(DefaultFilePath, json);

                return configData;
            }

        }
    }
}
