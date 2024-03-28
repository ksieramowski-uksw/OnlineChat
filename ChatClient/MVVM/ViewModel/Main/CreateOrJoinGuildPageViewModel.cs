using CommunityToolkit.Mvvm.Input;
using ChatShared.DataModels;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using ChatClient.Stores;
using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;


namespace ChatClient.MVVM.ViewModel {
    public partial class CreateOrJoinGuildPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        //[ObservableProperty]
        //private string _newGuildName;
        //
        //[ObservableProperty]
        //private string _newGuildPassword;
        //
        //[ObservableProperty]
        //private string _newGuildFeedback;
        //
        //
        //[ObservableProperty]
        //private string _existingGuildName;
        //
        //[ObservableProperty]
        //private string _existingGuildPassword;
        //
        //[ObservableProperty]
        //private string _existingGuildFeedback;

        public CreateOrJoinGuildPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            //NewGuildName = string.Empty;
            //NewGuildPassword = string.Empty;
            //NewGuildFeedback = string.Empty;
            //
            //ExistingGuildName = string.Empty;
            //ExistingGuildPassword = string.Empty;
            //ExistingGuildFeedback = string.Empty;
        }

        [RelayCommand]
        private void NavigateToCreateGuildPage() {
            if (NavigationStore.MainPage is MainPage mainPage) {
                NavigationStore.CreateGuildPage ??= new CreateGuildPage(NavigationStore);
                mainPage.MainPagePopupFrame.Navigate(NavigationStore.CreateGuildPage);
            }
        }

        [RelayCommand]
        private void NavigateToJoinGuildPage() {
            if (NavigationStore.MainPage is MainPage mainPage) {
                NavigationStore.JoinGuildPage ??= new JoinGuildPage(NavigationStore);
                mainPage.MainPagePopupFrame.Navigate(NavigationStore.JoinGuildPage);
            }
        }

        //[RelayCommand]
        //void CreateGuild() {
        //    string name = NewGuildName;
        //    string password = NewGuildPassword;
        //    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(password)) {
        //        string json = JsonSerializer.Serialize(new CreateGuildData(name, password));
        //        App.Current.Client.ServerConnection.Send(OperationCode.CreateGuild, json);
        //    }
        //    else {
        //        string errorMsg = "Please, fill required fields.";
        //        NewGuildFeedback = errorMsg;
        //    }
        //    
        //}
        //
        //[RelayCommand]
        //void JoinGuild() {
        //    string name = ExistingGuildName;
        //    string password = ExistingGuildPassword;
        //    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(password)) {
        //        string json = JsonSerializer.Serialize(new JoinGuildData(name, password));
        //        App.Current.Client.ServerConnection.Send(OperationCode.JoinGuild, json);
        //    }
        //    else {
        //        string errorMsg = "Please, fill required fields.";
        //        NewGuildFeedback = errorMsg;
        //    }
        //}



    }
}
