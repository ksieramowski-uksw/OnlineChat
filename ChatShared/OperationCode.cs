

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

        CreateGuild = 10,
        JoinGuild = 11,
        DeleteGuild = 12,
        UpdateGuild = 13,

        CreateCategory = 15,
        DeleteCategory = 16,
        UpdateCategory = 17,
        MoveCategory = 18,

        CreateChannel = 20,
        DeleteChannel = 21,
        UpdateChannel = 22,
        MoveChannel = 23,

        AddFriend = 25,
        RemoveFriend = 26,

        SendMessage = 30,




        SimpleTextMessage = 255
    }
}
