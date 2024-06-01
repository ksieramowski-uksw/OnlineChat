using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class DeleteConfirmationPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        public object? Target { get; set; }

        [ObservableProperty]
        public string _targetName;

        public DeleteConfirmationPageViewModel(ChatContext context, object? target) {
            Context = context;

            Target = target;
            TargetName = string.Empty;
            if (Target is Guild guild) {
                TargetName = $"server '{guild.Name}'?";
                return;
            }
            if (Target is Category category) {
                TargetName = $"category '{category.Name}'?";
                return;
            }
            if (Target is TextChannel textChannel) {
                TargetName = $"channel '{textChannel.Name}'?";
                return;
            }
        }

        [RelayCommand]
        private void Delete() {
            if (Target is Guild guild) {
                Context.Client.DeleteGuild(guild.ID);
                return;
            }
            if (Target is Category category) {
                Context.Client.DeleteCategory(category.ID);
                return;
            }
            if (Target is TextChannel textChannel) {
                Context.Client.DeleteTextChannel(textChannel.ID);
            }
        }
    }
}
