

namespace ChatShared.Models.Privileges {
    public class PrivilegedUser<PrivilegeType> where PrivilegeType : class, IPrivilege {
        public User User { get; set; }
        public PrivilegeType Privilege { get; set; }
        public PrivilegeType FinalPrivilege { get; set; }

        public PrivilegedUser(User user, PrivilegeType privilege, PrivilegeType finalPrivilege) {
            User = user;
            Privilege = privilege;
            FinalPrivilege = finalPrivilege;
        }

    }
}
