using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models.Privileges {
    public partial class PrivilegedUser<PrivilegeType>
            : ObservableObject where PrivilegeType : class, IPrivilege {

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private PrivilegeType _privilege;

        public PrivilegedUser(User user, PrivilegeType privilege) {
            User = user;
            Privilege = privilege;
        }

    }
}
