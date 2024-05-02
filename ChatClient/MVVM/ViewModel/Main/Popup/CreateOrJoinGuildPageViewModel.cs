﻿using CommunityToolkit.Mvvm.Input;
using ChatShared.DataModels;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using ChatClient.Stores;
using System.Windows;


namespace ChatClient.MVVM.ViewModel {
    public partial class CreateOrJoinGuildPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _newGuildName;

        [ObservableProperty]
        private string _newGuildPassword;

        [ObservableProperty]
        private string _newGuildFeedback;


        [ObservableProperty]
        private string _existingGuildId;

        [ObservableProperty]
        private string _existingGuildPassword;

        [ObservableProperty]
        private string _existingGuildFeedback;


        public CreateOrJoinGuildPageViewModel(ChatContext context) {
            Context = context;

            NewGuildName = string.Empty;
            NewGuildPassword = string.Empty;
            NewGuildFeedback = string.Empty;
            
            ExistingGuildId = string.Empty;
            ExistingGuildPassword = string.Empty;
            ExistingGuildFeedback = string.Empty;
        }


        [RelayCommand]
        void CreateGuild() {
            if (!string.IsNullOrWhiteSpace(NewGuildName) 
                && !string.IsNullOrWhiteSpace(NewGuildPassword)) {
                Context.Client.CreateGuild(NewGuildName, NewGuildPassword);
            }
            else {
                NewGuildFeedback = "Please, fill required fields.";
            }
        }

        [RelayCommand]
        private void JoinGuild() {
            if (!string.IsNullOrWhiteSpace(ExistingGuildId)
                && !string.IsNullOrWhiteSpace(ExistingGuildPassword)) {
                Context.Client.JoinGuild(ExistingGuildId, ExistingGuildPassword);
            }
            else {
                ExistingGuildFeedback = "Please, fill required fields.";
            }
        }


    }
}