using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;


namespace ChatServer.Database {
    public partial class DatabaseCommands {

        public User? GetUser(ID userID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                    SELECT
                            Users.PublicID,
                            Users.Nickname,
                            Users.Pronoun,
                            Users.CreationTime,
                            Users.ProfilePicture,
                            Users.Status
                        FROM Users
                        WHERE Users.ID = @UserID
                    ;";
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            string publicID = reader.GetFieldValue<string>(field++);
                            string nickname = reader.GetFieldValue<string>(field++);
                            string pronoun = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(field++));
                            byte[] profilePicture = reader.GetFieldValue<byte[]>(field++);
                            UserStatus status = reader.GetFieldValue<UserStatus>(field++);

                            return new User(userID, publicID, nickname, pronoun, creationTime, profilePicture, status);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public User? GetUserByLoginDetails(string email, string password) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                    SELECT
                            Users.ID,
                            Users.PublicID,
                            Users.Nickname,
                            Users.Pronoun,
                            Users.CreationTime,
                            Users.ProfilePicture,
                            Users.Status
                        FROM Users
                        WHERE Users.Email = @Email
                            AND Users.Password = @Password
                    ;";
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID id = reader.GetFieldValue<ID>(field++);
                            string publicID = reader.GetFieldValue<string>(field++);
                            string nickname = reader.GetFieldValue<string>(field++);
                            string pronoun = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(field++));
                            byte[] profilePicture = reader.GetFieldValue<byte[]>(field++);
                            UserStatus status = reader.GetFieldValue<UserStatus>(field++);

                            return new User(id, publicID, nickname, pronoun, creationTime, profilePicture, status);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public User? GetUserByEmail(string email) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                    SELECT
                            Users.ID,
                            Users.PublicID,
                            Users.Nickname,
                            Users.Pronoun,
                            Users.CreationTime,
                            Users.ProfilePicture,
                            Users.Status
                        FROM Users
                        WHERE Users.Email = @Email
                    ;";
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID id = reader.GetFieldValue<ID>(field++);
                            string publicID = reader.GetFieldValue<string>(field++);
                            string nickname = reader.GetFieldValue<string>(field++);
                            string pronoun = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(field++));
                            byte[] profilePicture = reader.GetFieldValue<byte[]>(field++);
                            UserStatus status = reader.GetFieldValue<UserStatus>(field++);

                            return new User(id, publicID, nickname, pronoun, creationTime, profilePicture, status);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public string? GetUniquePublicID(string tableName) {
            try {
                while (true) {
                    string publicID = Guid.NewGuid().ToString();
                    using (var command = Connection.CreateCommand()) {
                        command.CommandText = $@"
                            SELECT ID
                                FROM [{tableName}]
                                WHERE [{tableName}].PublicID LIKE @PublicID
                            ;";
                        command.Parameters.AddWithValue("@PublicID", publicID);

                        using (var reader = command.ExecuteReader()) {
                            if (!reader.HasRows) {
                                return publicID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ID GetGuildOwnerID(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                            WHERE GuildAffiliations.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            return reader.GetFieldValue<ID>(0);
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return 0;
            }
        }



        public ID GetCategoryOwnerID(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                                INNER JOIN Categories ON Categories.GuildID = GuildAffiliations.GuildID
                            WHERE Categories.ID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            return reader.GetFieldValue<ID>(0);
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return 0;
            }
        }



        public ID GetGuildAffiliationID(ID userID, ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.ID
                            FROM GuildAffiliations
                            WHERE GuildAffiliations.UserID = @UserID
                                AND GuildAffiliations.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            return reader.GetFieldValue<ID>(0);
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return 0;
            }
        }



        public Guild? GetGuild(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT
                            Guilds.PublicID,
                            Guilds.Name,
                            Guilds.OwnerID,
                            Guilds.Icon,
                            Guilds.CreationTime
                        FROM Guilds
                        WHERE Guilds.ID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            string publicID = reader.GetFieldValue<string>(field++);
                            string name = reader.GetFieldValue<string>(field++);
                            ID ownerID = reader.GetFieldValue<ID>(field++);
                            byte[] icon = reader.GetFieldValue<byte[]>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

                            return new Guild(guildID, publicID, name, ownerID, icon, creationTime);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public Guild? GetGuildByJoinDetails(string publicID, string password) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                                Guilds.ID,
                                Guilds.Name,
                                Guilds.OwnerID,
                                Guilds.Icon,
                                Guilds.CreationTime
                            FROM Guilds
                            WHERE Guilds.PublicID = @GuildPublicID
                                AND Guilds.Password = @GuildPassword
                        ;";
                    command.Parameters.AddWithValue("@GuildPublicID", publicID);
                    command.Parameters.AddWithValue("@GuildPassword", password);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID guildID = reader.GetFieldValue<ID>(field++);
                            string name = reader.GetFieldValue<string>(field++);
                            ID ownerID = reader.GetFieldValue<ID>(field++);
                            byte[] icon = reader.GetFieldValue<byte[]>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

                            return new Guild(guildID, publicID, name, ownerID, icon, creationTime);
                        }
                    }
                }
                Logger.Error($"Failed to get guild by join details with publicID '{publicID}'.", MethodBase.GetCurrentMethod());
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public Category? GetCategory(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT
                            Categories.GuildID,
                            Categories.Name,
                            Categories.CreationTime
                        FROM Categories
                        WHERE Categories.ID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID guildID = reader.GetFieldValue<ID>(field++);
                            string categoryName = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

                            return new Category(categoryID, guildID, categoryName, creationTime);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public TextChannel? GetTextChannel(ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                            TextChannels.CategoryID,
                            TextChannels.Name,
                            TextChannels.CreationTime
                        FROM TextChannels
                        WHERE TextChannels.ID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID categoryID = reader.GetFieldValue<ID>(field++);
                            string name = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

                            return new TextChannel(textChannelID, categoryID, name, creationTime);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        #region Privileges

        public GuildPrivilege? GetGuildPrivilege(ID userID, ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
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
                        WHERE GuildPrivileges.UserID = @UserID AND GuildPrivileges.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public CategoryPrivilege? GetCategoryPrivilege(ID userID, ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
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
                        WHERE CategoryPrivileges.UserID = @UserID AND CategoryPrivileges.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ObservableCollection<CategoryPrivilege>? GetAllPrivilegesInCategory(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                                CategoryPrivileges.ID,
                                CategoryPrivileges.UserID,
                                CategoryPrivileges.UpdateCategory,
                                CategoryPrivileges.DeleteCategory,
                                CategoryPrivileges.ViewCategory,
                                CategoryPrivileges.CreateChannel,
                                CategoryPrivileges.UpdateChannel,
                                CategoryPrivileges.DeleteChannel,
                                CategoryPrivileges.Read,
                                CategoryPrivileges.Write
                            FROM CategoryPrivileges
                            WHERE CategoryPrivileges.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    ObservableCollection<CategoryPrivilege> privileges = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
                            ID userID = reader.GetFieldValue<ID>(field++);
                            PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue viewCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                            privileges.Add(new CategoryPrivilege(privilegeID, userID, categoryID) {
                                UpdateCategory = updateCategory,
                                DeleteCategory = deleteCategory,
                                ViewCategory = viewCategory,
                                CreateChannel = createChannel,
                                UpdateChannel = updateChannel,
                                DeleteChannel = deleteChannel,
                                Read = read,
                                Write = write,
                            });
                        }
                        return privileges;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public TextChannelPrivilege? GetTextChannelPrivilege(ID userID, ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                            TextChannelPrivileges.ID,
                            TextChannelPrivileges.UpdateChannel,
                            TextChannelPrivileges.DeleteChannel,
                            TextChannelPrivileges.ViewChannel,
                            TextChannelPrivileges.Read,
                            TextChannelPrivileges.Write
                        FROM TextChannelPrivileges
                        WHERE TextChannelPrivileges.UserID = @UserID
                            AND TextChannelPrivileges.ChannelID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ObservableCollection<TextChannelPrivilege>? GetAllPrivilegesInTextChannel(ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                                TextChannelPrivileges.ID,
                                TextChannelPrivileges.UserID,
                                TextChannelPrivileges.UpdateChannel,
                                TextChannelPrivileges.DeleteChannel,
                                TextChannelPrivileges.ViewChannel,
                                TextChannelPrivileges.Read,
                                TextChannelPrivileges.Write
                            FROM TextChannelPrivileges
                            WHERE TextChannelPrivileges.ChannelID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);

                    ObservableCollection<TextChannelPrivilege> privileges = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
                            ID userID = reader.GetFieldValue<ID>(field++);
                            PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue viewChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                            privileges.Add(new TextChannelPrivilege(privilegeID, userID, textChannelID) {
                                UpdateChannel = updateChannel,
                                DeleteChannel = deleteChannel,
                                ViewChannel = viewChannel,
                                Read = read,
                                Write = write,
                            });
                        }
                        return privileges;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }


        #endregion Privileges

        #region Defeault Privileges

        public GuildPrivilege? GetDefaultGuildPrivilege(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
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
                        WHERE DefaultGuildPrivileges.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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

                            GuildPrivilege privilege = new(privilegeID, 0, guildID) {
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
                            return privilege;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }


        public CategoryPrivilege? GetDefaultCategoryPrivilege(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
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
                        WHERE DefaultCategoryPrivileges.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public TextChannelPrivilege? GetDefaultTextChannelPrivilege(ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                            DefaultTextChannelPrivileges.ID,
                            DefaultTextChannelPrivileges.UpdateChannel,
                            DefaultTextChannelPrivileges.DeleteChannel,
                            DefaultTextChannelPrivileges.ViewChannel,
                            DefaultTextChannelPrivileges.Read,
                            DefaultTextChannelPrivileges.Write
                        FROM DefaultTextChannelPrivileges
                        WHERE DefaultTextChannelPrivileges.ChannelID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            byte field = 0;
                            ID privilegeID = reader.GetFieldValue<ID>(field++);
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
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }

        #endregion Default Privileges



        public List<ID>? GetKnownUsers(ID userID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                            WHERE GuildAffiliations.GuildID = @UserID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);

                    List<ID>? users = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            ID id = reader.GetFieldValue<ID>(0);
                            users.Add(id);
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }




        public List<ID>? GetUsersInGuild(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                            WHERE GuildAffiliations.GuildID = @GuildID";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    List<ID>? users = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            users.Add(reader.GetFieldValue<ID>(0));
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ObservableCollection<User>? GetUsersInGuild<T>(ID guildID) where T : User {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT Users.ID, Users.PublicID, Users.Nickname, Users.Pronoun, Users.CreationTime, Users.ProfilePicture, Users.Status
                            FROM GuildAffiliations
                                INNER JOIN Users ON Users.ID = GuildAffiliations.UserID
                            WHERE GuildAffiliations.GuildID = @GuildID";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    ObservableCollection<User>? users = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID userID = reader.GetFieldValue<ID>(field++);
                            string publicID = reader.GetFieldValue<string>(field++);
                            string nickname = reader.GetFieldValue<string>(field++);
                            string pronoun = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);
                            byte[] profilePicture = reader.GetFieldValue<byte[]>(field++);
                            UserStatus status = reader.GetFieldValue<UserStatus>(field++);

                            users.Add(new User(userID, publicID, nickname, pronoun, creationTime, profilePicture, status));
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public List<ID>? GetUsersInCategory(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                                INNER JOIN Guilds on GuildAffiliations.GuildID = Guilds.ID
                                INNER JOIN Categories on Guilds.ID = Categories.GuildID
                            WHERE Categories.ID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    List<ID> users = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            users.Add(reader.GetFieldValue<ID>(0));
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public List<ID>? GetUsersInTextChannel(ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT GuildAffiliations.UserID
                            FROM GuildAffiliations
                                INNER JOIN Guilds ON GuildAffiliations.GuildID = Guilds.ID
                                INNER JOIN Categories ON Guilds.ID = Categories.GuildID
                                INNER JOIN TextChannels ON TextChannels.CategoryID = Categories.ID
                            WHERE TextChannels.ID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);

                    List<ID> users = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            users.Add(reader.GetFieldValue<ID>(0));
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }


        public ObservableCollection<Category>? GetCategoriesInGuild(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT Categories.ID, Categories.GuildID, Categories.Name, Categories.CreationTime
                            FROM Categories
                            WHERE Categories.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    ObservableCollection<Category> categories = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID categoryID = reader.GetFieldValue<ID>(field++);
                            ID categoryGuildID = reader.GetFieldValue<ID>(field++);
                            string categoryName = reader.GetFieldValue<string>(field++);
                            DateTime categoryCreationTime = DateTime.Parse(reader.GetFieldValue<string>(field++));

                            Category category = new(categoryID, categoryGuildID, categoryName, categoryCreationTime);

                            category.Privileges = GetAllPrivilegesInCategory(categoryID);
                            if (category.Privileges == null) {
                                Logger.Error($"Failed to get privileges in category '{category.ID}'.", MethodBase.GetCurrentMethod());
                                return null;
                            }
                            category.DefaultPrivilege = GetDefaultCategoryPrivilege(categoryID);
                            if (category.DefaultPrivilege == null) {
                                Logger.Error($"Failed to get privileges in category '{category.ID}'.", MethodBase.GetCurrentMethod());
                                return null;
                            }
                            category.TextChannels = GetTextChannelsInCategory(categoryID);
                            if (category.TextChannels == null) {
                                Logger.Error($"Failed to get text channels in category '{category.ID}'.", MethodBase.GetCurrentMethod());
                                return null;
                            }

                            categories.Add(category);
                        }
                        return categories;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }


        public ObservableCollection<TextChannel>? GetTextChannelsInCategory(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT TextChannels.ID, TextChannels.Name, TextChannels.CreationTime
                            FROM TextChannels
                            WHERE TextChannels.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    ObservableCollection<TextChannel> textChannels = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID textChannelID = reader.GetFieldValue<ID>(field++);
                            string textChannelName = reader.GetFieldValue<string>(field++);
                            DateTime creationTime = reader.GetFieldValue<DateTime>(field++);

                            TextChannel textChannel = new(textChannelID, categoryID, textChannelName, creationTime);

                            textChannel.Privileges = GetAllPrivilegesInTextChannel(textChannelID);
                            if (textChannel.Privileges == null) {
                                Logger.Error($"Failed to get privileges in text channel '{textChannel.ID}'.", MethodBase.GetCurrentMethod());
                                return null;
                            }
                            textChannel.DefaultPrivilege = GetDefaultTextChannelPrivilege(textChannelID);
                            if (textChannel.DefaultPrivilege == null) {
                                Logger.Error($"Failed to get default privilege in text channel '{textChannel.ID}'.", MethodBase.GetCurrentMethod());
                                return null;
                            }

                            textChannels.Add(textChannel);
                        }
                        return textChannels;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }

    }
}
