

namespace ChatClient {
    public enum OperationCode : byte {
        InvalidOperation,

        LogIn,
        LogInSuccess,
        LogInFail,

        LogOut,
        LogOutSuccess,
        LogOutFail,

        Register,
        RegisterSuccess,
        RegisterFail,

        CreateGuild,
        CreateGuildSuccess,
        CreateGuildFail,

        JoinGuild,
        JoinGuildSuccess,
        JoinGuildFail,

        DeleteGuild,
        DeleteGuildSuccess,
        DeleteGuildFail,

        UpdateGuild,
        UpdateGuildSuccess,
        UpdateGuildFail,

        CreateCategory,
        CreateCategorySuccess,
        CreateCategoryFail,

        DeleteCategory,
        DeleteCategorySuccess,
        DeleteCategoryFail,

        UpdateCategory,
        UpdateCategorySuccess,
        UpdateCategoryFail,

        MoveCategory,
        MoveCategorySuccess,
        MoveCategoryFail,

        CreateTextChannel,
        CreateTextChannelSuccess,
        CreateTextChannelFail,

        DeleteTextChannel,
        DeleteTextChannelSuccess,
        DeleteTextChannelFail,

        UpdateTextChannel,
        UpdateTextChannelSuccess,
        UpdateTextChannelFail,

        MoveTextChannel, // change channel's category
        MoveTextChannelSuccess,
        MoveTextChannelFail,

        GetGuildsForUser,
        GetGuildsForUserSuccess,
        GetGuildsForUserFail,

        GetCategoriesInGuild,
        GetCategoriesInGuildSuccess,
        GetCategoriesInGuildFail,

        CreateGuildPrivilege,
        CreateGuildPrivilegeSuccess,
        CreateGuildPrivilegeFail,

        UpdateGuildPrivilege,
        UpdateGuildPrivilegeSuccess,
        UpdateGuildPrivilegeFail,

        CreateCategoryPrivilege,
        CreateCategoryPrivilegeSuccess,
        CreateCategoryPrivilegeFail,

        UpdateCategoryPrivilege,
        UpdateCategoryPrivilegeSuccess,
        UpdateCategoryPrivilegeFail,

        CreateTextChannelPrivilege,
        CreateTextChannelPrivilegeSuccess,
        CreateTextChannelPrivilegeFail,

        UpdateTextChannelPrivilege,
        UpdateTextChannelPrivilegeSuccess,
        UpdateTextChannelPrivilageFail,

        GetTextChannelsInCategory,
        GetTextChannelsInCategorySuccess,
        GetTextChannelsInCategoryFail,

        GetGuildPrivilegeForUser,
        GetGuildPrivilegeForUserSuccess,
        GetGuildPrivilegeForUserFail,

        GetCategoryPrivilegeForUser,
        GetCategoryPrivilegeForUserSuccess,
        GetCategoryPrivilegeForUserFail,

        GetTextChannelPrivilegeForUser,
        GetTextChannelPrivilegeForUserSuccess,
        GetTextChannelPrivilegeForUserFail,

        AddFriend,
        AddFriendSuccess,
        AddFriendFail,

        RemoveFriend,
        RemoveFriendSuccess,
        RemoveFriendFail,

        GetFriendList,
        GetFriendListSuccess,
        GetFriendListFail,

        SendMessage,
        SendMessageSuccess,
        SendMessageFail,




        SimpleTextMessage
    }
}
