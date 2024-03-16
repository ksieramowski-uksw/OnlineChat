using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSharedLib
{
    public class RegisterData
    {
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string ConfirmPassword { get; private set; }
        public string Nickname { get; private set; }

        public RegisterData(string email, string password, string confirmPassword, string nickname)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            Nickname = nickname;
        }
    }
}
