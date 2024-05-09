using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace ChatServer.Database {
    public partial class DatabaseCommands {

        public User? GetUserByID(ulong userID) {
            var reader = ExecuteReader($@"
                SELECT
                    Users.ID,
                    Users.PublicID,
                    Users.Nickname,
                    Users.Pronoun,
                    Users.CreationTime,
                    Users.ProfilePicture,
                    Users.Status
                FROM Users
                WHERE Users.ID = {userID}");
            if (reader == null) {
                return null;
            }
            if (reader.Read()) {
                return GetUser(reader);
            }
            return null;
        }
        public User GetUser(SqliteDataReader reader) {
            byte field = 0;
            ulong id = reader.GetFieldValue<ulong>(field++);
            string publicID = reader.GetFieldValue<string>(field++);
            string nickname = reader.GetFieldValue<string>(field++);
            string pronoun = reader.GetFieldValue<string>(field++);
            DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(field++));
            byte[] profilePicture = reader.GetFieldValue<byte[]>(field++);
            UserStatus status = reader.GetFieldValue<UserStatus>(field++);

            return new User(id, publicID, nickname, pronoun, creationTime, profilePicture, status);
        }


        public Guild? GetGuildByID(ulong guildID) {
            var reader = ExecuteReader($@"
                SELECT
                    Guilds.ID,
                    Guilds.PublicID,
                    Guilds.Name,
                    Guilds.Password,
                    Guilds.OwnerID,
                    Guilds.CreationTime,
                    Guilds.Icon
                FROM Guilds
                WHERE Guilds.ID = '{guildID}'");
            if (reader == null) {
                return null;
            }
            if (reader.Read()) {
                return GetGuild(reader);
            }
            return null;
        }
        public Guild GetGuild(SqliteDataReader reader) {
            byte field = 0;
            ulong guildID = reader.GetFieldValue<ulong>(field++);
            string publicID = reader.GetFieldValue<string>(field++);
            string name = reader.GetFieldValue<string>(field++);
            string password = reader.GetFieldValue<string>(field++);
            ulong ownerID = reader.GetFieldValue<ulong>(field++);
            DateTime CreationTime = reader.GetFieldValue<DateTime>(field++);
            byte[] icon = reader.GetFieldValue<byte[]>(field++);

            return new Guild(guildID, publicID, name, password, ownerID, CreationTime, icon);
        }

        public Category? GetCategoryByID(ulong categoryID) {
            var reader = ExecuteReader($@"
                SELECT
                    Categories.ID,
                    Categories.GuildID,
                    Categories.Name,
                    Categories.CreationTime
                FROM Categories
                WHERE Categories.ID = '{categoryID}'");
            if (reader == null) {
                return null;
            }
            if (reader.Read()) {
                return GetCategory(reader);
            }
            return null;
        }
        public Category GetCategory(SqliteDataReader reader) {
            byte field = 0;
            ulong id = reader.GetFieldValue<ulong>(field++);
            ulong guildID = reader.GetFieldValue<ulong>(field++);
            string name = reader.GetFieldValue<string>(field++);
            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

            return new Category(id, guildID, name, creationTime);
        }


        public TextChannel? GetTextChannelByID(ulong textChannelID) {
            var reader = ExecuteReader($@"
                SELECT
                    TextChannels.ID,
                    TextChannels.CategoryID,
                    TextChannels.Name,
                    TextChannels.CreationTime
                FROM TextChannels
                WHERE TextChannels.ID = '{textChannelID}'");
            if (reader == null) {
                return null;
            }
            if (reader.Read()) {
                return GetTextChannel(reader);
            }
            return null;
        }
        public TextChannel GetTextChannel(SqliteDataReader reader) {
            byte field = 0;
            ulong textChannelID = reader.GetFieldValue<ulong>(field++);
            ulong categoryID = reader.GetFieldValue<ulong>(field++);
            string name = reader.GetFieldValue<string>(field++);
            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

            return new TextChannel(textChannelID, categoryID, name, creationTime);
        }



        #region Privileges

        public GuildPrivilege? GetGuildPrivilege(ulong userID, ulong guildID) {
            var reader = ExecuteReader($@"
                SELECT
                    GuildPrivileges.ID,
                    GuildPrivileges.ManageGuild,
                    GuildPrivileges.ManagePrivileges,
                    GuildPrivileges.CreateCategory,
                    GuildPrivileges.UpdateCategory,
                    GuildPrivileges.DeleteCategory,
                    GuildPrivileges.CreateChannel,
                    GuildPrivileges.UpdateChannel,
                    GuildPrivileges.DeleteChannel,
                    GuildPrivileges.Read,
                    GuildPrivileges.Write
                FROM GuildPrivileges
                WHERE GuildPrivileges.UserID = '{userID}' AND GuildPrivileges.GuildID = '{guildID}'
                ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue manageGuild = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue managePrivileges = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);      
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new GuildPrivilege(privilegeID, userID, guildID) {
                    ManageGuild = manageGuild,
                    ManagePrivileges = managePrivileges,
                    CreateCategory = createCategory,
                    UpdateCategory = updateCategory,
                    DeleteCategory = deleteCategory,
                    CreateChannel = createChannel,
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        public CategoryPrivilege? GetCategoryPrivilege(ulong userID, ulong categoryID) {
            var reader = ExecuteReader($@"
                SELECT 
                    CategoryPrivileges.ID,
                    CategoryPrivileges.UpdateCategory,
                    CategoryPrivileges.DeleteCategory,
                    CategoryPrivileges.ViewCategory,
                    CategoryPrivileges.CreateChannel,
                    CategoryPrivileges.UpdateChannel,
                    CategoryPrivileges.DeleteChannel,
                    CategoryPrivileges.Read,
                    CategoryPrivileges.Write
                FROM CategoryPrivileges
                WHERE CategoryPrivileges.UserID = '{userID}' AND CategoryPrivileges.CategoryID = '{categoryID}'
            ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue viewCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new CategoryPrivilege(privilegeID, userID, categoryID) {
                    UpdateCategory = updateCategory,
                    DeleteCategory = deleteCategory,
                    ViewCategory = viewCategory,
                    CreateChannel = createChannel,
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        public TextChannelPrivilege? GetTextChannelPrivilege(ulong userID, ulong textChannelID) {
            var reader = ExecuteReader($@"
                SELECT
                    TextChannelPrivileges.ID,
                    TextChannelPrivileges.UpdateChannel,
                    TextChannelPrivileges.DeleteChannel,
                    TextChannelPrivileges.ViewChannel,
                    TextChannelPrivileges.Read,
                    TextChannelPrivileges.Write
                FROM TextChannelPrivileges
                WHERE TextChannelPrivileges.UserID = '{userID}' AND TextChannelPrivileges.ChannelID = '{textChannelID}'
                ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue viewChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new TextChannelPrivilege(privilegeID, userID, textChannelID) {
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    ViewChannel = viewChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        #endregion Privileges

        #region Defeault Privileges

        public GuildPrivilege? GetDefaultGuildPrivilege(ulong guildID) {
            var reader = ExecuteReader($@"
                SELECT
                    DefaultGuildPrivileges.ID,
                    DefaultGuildPrivileges.ManageGuild,
                    DefaultGuildPrivileges.ManagePrivileges,
                    DefaultGuildPrivileges.CreateCategory,
                    DefaultGuildPrivileges.UpdateCategory,
                    DefaultGuildPrivileges.DeleteCategory,
                    DefaultGuildPrivileges.CreateChannel,
                    DefaultGuildPrivileges.UpdateChannel,
                    DefaultGuildPrivileges.DeleteChannel,
                    DefaultGuildPrivileges.Read,
                    DefaultGuildPrivileges.Write
                FROM DefaultGuildPrivileges
                WHERE DefaultGuildPrivileges.GuildID = '{guildID}'
                ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue manageGuild = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue managePrivileges = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new GuildPrivilege(privilegeID, 0, guildID) {
                    ManageGuild = manageGuild,
                    ManagePrivileges = managePrivileges,
                    CreateCategory = createCategory,
                    UpdateCategory = updateCategory,
                    DeleteCategory = deleteCategory,
                    CreateChannel = createChannel,
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }


        public CategoryPrivilege? GetDefaultCategoryPrivilege(ulong categoryID) {
            var reader = ExecuteReader($@"
                SELECT 
                    DefaultCategoryPrivileges.ID,
                    DefaultCategoryPrivileges.UpdateCategory,
                    DefaultCategoryPrivileges.DeleteCategory,
                    DefaultCategoryPrivileges.ViewCategory,
                    DefaultCategoryPrivileges.CreateChannel,
                    DefaultCategoryPrivileges.UpdateChannel,
                    DefaultCategoryPrivileges.DeleteChannel,
                    DefaultCategoryPrivileges.Read,
                    DefaultCategoryPrivileges.Write
                FROM DefaultCategoryPrivileges
                WHERE DefaultCategoryPrivileges.CategoryID = '{categoryID}'
            ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue viewCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new CategoryPrivilege(privilegeID, 0, categoryID) {
                    UpdateCategory = updateCategory,
                    DeleteCategory = deleteCategory,
                    ViewCategory = viewCategory,
                    CreateChannel = createChannel,
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        public TextChannelPrivilege? GetDefaultTextChannelPrivilege(ulong textChannelID) {
            var reader = ExecuteReader($@"
                SELECT
                    DefaultTextChannelPrivileges.ID,
                    DefaultTextChannelPrivileges.UpdateChannel,
                    DefaultTextChannelPrivileges.DeleteChannel,
                    DefaultTextChannelPrivileges.ViewChannel,
                    DefaultTextChannelPrivileges.Read,
                    DefaultTextChannelPrivileges.Write
                FROM DefaultTextChannelPrivileges
                WHERE DefaultTextChannelPrivileges.ChannelID = '{textChannelID}'
                ;");
            if (reader == null) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - reader is null");
                return null;
            }
            if (reader.Read()) {
                byte field = 0;
                ulong privilegeID = reader.GetFieldValue<ulong>(field++);
                PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue viewChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                return new TextChannelPrivilege(privilegeID, 0, textChannelID) {
                    UpdateChannel = updateChannel,
                    DeleteChannel = deleteChannel,
                    ViewChannel = viewChannel,
                    Read = read,
                    Write = write,
                };
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        #endregion Default Privileges


        public GuildPrivilege? GetFinalGuildPrivilege(ulong userID, ulong guildID) {
            return GetGuildPrivilege(userID, guildID);
        }

        public CategoryPrivilege? GetFinalCategoryPrivilege(ulong userID, ulong categoryID) {
            var reader = ExecuteReader($@"
                SELECT Categories.GuildID
                FROM Categories
                WHERE Categories.ID = '{categoryID}'
            ;");
            if (reader == null) {
                Logger.Error($"[{MethodBase.GetCurrentMethod()}] - reader is null");
                return null;
            }
            if (reader.Read()) {
                ulong guildID = reader.GetFieldValue<ulong>(0);

                GuildPrivilege? guildPrivilege = GetGuildPrivilege(userID, guildID);
                if (guildPrivilege == null) {
                    Logger.Error($"{MethodBase.GetCurrentMethod()} - guild privilege is null");
                    return null;
                }

                CategoryPrivilege? categoryPrivilege = GetCategoryPrivilege(userID, categoryID);
                if (categoryPrivilege == null) {
                    Logger.Error($"[{MethodBase.GetCurrentMethod()}] - category privilege is null");
                    return null;
                }

                CategoryPrivilege finalPrivilege = categoryPrivilege.Merge(guildPrivilege);
                return finalPrivilege;
            }
            Logger.Error($"{MethodBase.GetCurrentMethod()} - nothing to read");
            return null;
        }

        public TextChannelPrivilege? GetFinalTextChannelPrivilege(ulong userID, ulong textChannelID) {
            var reader = ExecuteReader($@"
                SELECT Categories.ID, Categories.GuildID
                FROM TextChannels INNER JOIN Categories ON TextChannels.CategoryID = Categories.ID
                WHERE TextChannels.ID = '{textChannelID}'
            ;");
            if (reader == null) {
                Logger.Error($"[{MethodBase.GetCurrentMethod()}] - reader is null");
                return null;
            }
            if (reader.Read()) {
                ulong categoryID = reader.GetFieldValue<ulong>(0);
                ulong guildID = reader.GetFieldValue<ulong>(1);

                GuildPrivilege? guildPrivilege = GetGuildPrivilege(userID, guildID);
                if (guildPrivilege == null) {
                    Logger.Error($"[{MethodBase.GetCurrentMethod()}] - guild privilege is null");
                    return null;
                }

                CategoryPrivilege? categoryPrivilege = GetCategoryPrivilege(userID, categoryID);
                if (categoryPrivilege == null) {
                    Logger.Error($"[{MethodBase.GetCurrentMethod()}] - category privilege is null");
                    return null;
                }

                TextChannelPrivilege? textChannelPrivilege = GetTextChannelPrivilege(userID, textChannelID);
                if (textChannelPrivilege == null) {
                    Logger.Error($"[{MethodBase.GetCurrentMethod()}] - text channel privilege is null");
                    return null;
                }

                CategoryPrivilege mergedPrivilege = categoryPrivilege.Merge(guildPrivilege);
                TextChannelPrivilege finalPrivilege = textChannelPrivilege.Merge(mergedPrivilege);
                return finalPrivilege;
            }
            Logger.Error($"[{MethodBase.GetCurrentMethod()}] - nothing to read");
            return null;
        }

    }
}
