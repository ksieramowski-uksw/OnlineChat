using ChatClient.MVVM.View.Main;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class NotificationPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _message;

        public NotificationPageViewModel(ChatContext context, string message) {
            Context = context;
            Message = message;
        }

        [RelayCommand]
        private void Confirm() {
            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                mainPage.ViewModel.HideMask();
            }
        }
    }
}
