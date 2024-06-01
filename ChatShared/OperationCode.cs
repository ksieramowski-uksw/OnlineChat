

namespace ChatShared {
    public enum OperationCode : byte {
        InvalidOperation,

        // user
        Register,
        RegisterSuccess,
        RegisterFail,

        LogIn,
        LogInSuccess,
        LogInFail,

        UpdateUser,
        UpdateUserSuccess,
        UpdateUserFail,

        UserStatusChanged,

        // guild
        JoinGuild,
        JoinGuildSuccess,
        JoinGuildFail,

        CreateGuild,
        CreateGuildSuccess,
        CreateGuildFail,

        UpdateGuild,
        UpdateGuildSuccess,
        UpdateGuildFail,

        DeleteGuild,
        DeleteGuildSuccess,
        DeleteGuildFail,

        GetGuildsForCurrentUser,
        GetGuildsForCurrentUserSuccess,
        GetGuildsForCurrentUserFail,

        GetGuildDetails,
        GetGuildDetailsSuccess,
        GetGuildDetailsFail,

        // category
        CreateCategory,
        CreateCategorySuccess,
        CreateCategoryFail,

        UpdateCategory,
        UpdateCategorySuccess,
        UpdateCategoryFail,

        DeleteCategory,
        DeleteCategorySuccess,
        DeleteCategoryFail,

        // text channel
        CreateTextChannel,
        CreateTextChannelSuccess,
        CreateTextChannelFail,

        UpdateTextChannel,
        UpdateTextChannelSuccess,
        UpdateTextChannelFail,

        DeleteTextChannel,
        DeleteTextChannelSuccess,
        DeleteTextChannelFail,

        // message
        SendMessage,
        SendMessageSuccess,
        SendMessageFail,

        GetMessageRange,
        GetMessageRangeSuccess,
        GetMessageRangeFail,





        //MoveCategory,
        //MoveCategorySuccess,
        //MoveCategoryFail,

        //MoveTextChannel, // change channel's category
        //MoveTextChannelSuccess,
        //MoveTextChannelFail,

        //GetGuildsForUser,
        //GetGuildsForUserSuccess,
        //GetGuildsForUserFail,

        //GetCategoriesInGuild,
        //GetCategoriesInGuildSuccess,
        //GetCategoriesInGuildFail,

        //CreateGuildPrivilege,
        //CreateGuildPrivilegeSuccess,
        //CreateGuildPrivilegeFail,

        //UpdateGuildPrivilege,
        //UpdateGuildPrivilegeSuccess,
        //UpdateGuildPrivilegeFail,

        //CreateCategoryPrivilege,
        //CreateCategoryPrivilegeSuccess,
        //CreateCategoryPrivilegeFail,

        //UpdateCategoryPrivilege,
        //UpdateCategoryPrivilegeSuccess,
        //UpdateCategoryPrivilegeFail,

        //CreateTextChannelPrivilege,
        //CreateTextChannelPrivilegeSuccess,
        //CreateTextChannelPrivilegeFail,

        //UpdateTextChannelPrivilege,
        //UpdateTextChannelPrivilegeSuccess,
        //UpdateTextChannelPrivilageFail,

        //GetTextChannelsInCategory,
        //GetTextChannelsInCategorySuccess,
        //GetTextChannelsInCategoryFail,

        //GetGuildPrivilegeForUser,
        //GetGuildPrivilegeForUserSuccess,
        //GetGuildPrivilegeForUserFail,

        //GetCategoryPrivilegeForUser,
        //GetCategoryPrivilegeForUserSuccess,
        //GetCategoryPrivilegeForUserFail,

        //GetTextChannelPrivilegeForUser,
        //GetTextChannelPrivilegeForUserSuccess,
        //GetTextChannelPrivilegeForUserFail,

        //AddFriend,
        //AddFriendSuccess,
        //AddFriendFail,

        //RemoveFriend,
        //RemoveFriendSuccess,
        //RemoveFriendFail,

        //GetFriendList,
        //GetFriendListSuccess,
        //GetFriendListFail,



        SimpleTextMessage
    }
}
