using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel.Login {
    public partial class LoginWindowViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private object? _mainFrameContent;

        public LoginWindowViewModel(ChatContext context) {
            Context = context;
        }

        [RelayCommand]
        public void Navigate(object? content) {
            MainFrameContent = content;
        }
    }
}
