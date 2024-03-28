using ChatClient.MVVM.View;
using ChatClient.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.MVVM.ViewModel {
    public partial class MainPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private Visibility _maskVisiblity;

        public MainPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;
        }

        [RelayCommand]
        private void AddGuild() {
            if (NavigationStore.MainPage is MainPage mainPage) {
                CreateOrJoinGuildPage createOrJoinGuildPage = new(NavigationStore);
                mainPage.MainPagePopupFrame.Content = createOrJoinGuildPage;
                mainPage.MainPageMaskGrid.Visibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        private void FriendList() {
            
        }
    }
}
