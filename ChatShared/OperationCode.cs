using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public enum OperationCode : byte
    {
        LogIn = 1,
        LogOut = 2,
        Register = 3,

        SimpleTextMessage = 11,


    }
}
