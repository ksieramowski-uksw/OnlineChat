using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class LoginData
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public LoginData(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
