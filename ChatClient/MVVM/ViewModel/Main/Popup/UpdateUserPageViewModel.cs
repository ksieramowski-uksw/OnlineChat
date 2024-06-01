using ChatClient.MVVM.View.Main;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.IO;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class UpdateUserPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private string _pronoun;

        [ObservableProperty]
        private string _profilePictureFilePath;

        [ObservableProperty]
        private string _profilePictureFilePathText;

        [ObservableProperty]
        private string _feedback;

        public UpdateUserPageViewModel(ChatContext context, User user) {
            Context = context;

            User = user;
            Nickname = user.Nickname;
            Pronoun = user.Pronoun;

            Feedback = string.Empty;

            ProfilePictureFilePath = ResourceHelper.DefaultImage;
            ProfilePictureFilePathText = "Default";
        }

        [RelayCommand]
        private void UpdateUser() {
            if (!string.IsNullOrWhiteSpace(Nickname)
                && !string.IsNullOrWhiteSpace(Pronoun)) {
                if (ProfilePictureFilePathText == "Default") {
                    Context.Client.UpdateUser(User.ID, Nickname, Pronoun, ProfilePictureFilePathText);
                }
                else {
                    Context.Client.UpdateUser(User.ID, Nickname, Pronoun, ProfilePictureFilePath);
                }
            }
            else {
                Feedback = "Please, fill required fields.";
            }
        }

        [RelayCommand]
        private void SelectProfilePictureFile() {
            OpenFileDialog openFileDialog = new() {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff|All files (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (openFileDialog.ShowDialog() == true) {
                ProfilePictureFilePath = openFileDialog.FileName;
                ProfilePictureFilePathText = openFileDialog.FileName;
            }
        }
    }
}
