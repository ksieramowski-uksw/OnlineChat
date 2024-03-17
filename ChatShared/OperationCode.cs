

namespace ChatClient {
    public enum OperationCode : byte {
        InvalidOperation = 0,

        LogIn = 1,
        LogInSuccess = 2,
        LogInFail = 3,

        LogOut = 4,
        LogOutSuccess = 5,
        LogOutFail = 6,

        Register = 7,
        RegisterSuccess = 8,
        RegisterFail = 9,



        SimpleTextMessage = 255
    }
}
