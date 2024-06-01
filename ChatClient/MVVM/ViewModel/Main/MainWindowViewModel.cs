using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class MainWindowViewModel : ObservableObject {
        public ChatContext Context { get; set; }

        [ObservableProperty]
        private Page? _mainPage;

        public MainWindowViewModel(ChatContext context) {
            Context = context;
        }


    }
}
