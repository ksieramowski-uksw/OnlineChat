using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Database {

    public enum DatabaseCommandResult {
        Success,
        Fail,
        UnknownError,
        DatabaseError,
        UserExists,
        UserDoesntExist,
    }
}
