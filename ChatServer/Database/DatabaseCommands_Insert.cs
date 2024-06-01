using ChatShared;
using ChatShared.Models.Privileges;
using System.Reflection;
using System.Text.Json;


namespace ChatServer.Database {
    public partial class DatabaseCommands {


        public ID RegisterUser(string publicID, string email, string password, string nickname, string pronoun, byte[] profilePicture, DateTime creationTime) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO Users
                                (PublicID, Email, Password, Nickname, Pronoun, ProfilePicture, CreationTime, Status)
                            VALUES
                                (@PublicID, @Email, @Password, @Nickname, @Pronoun, @ProfilePicture, @CreationTime, @Status)
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@PublicID", publicID);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Nickname", nickname);
                    command.Parameters.AddWithValue("@Pronoun", pronoun);
                    command.Parameters.AddWithValue("@ProfilePicture", profilePicture);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);
                    command.Parameters.AddWithValue("@Status", UserStatus.Offline);

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



        public ID CreateGuildPrivilege(GuildPrivilege privilege) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO GuildPrivileges (
                                UserID,
                                GuildID,
                                ManageGuild,
                                ManagePrivileges,
                                CreateCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @GuildID,
                                @ManageGuild,
                                @ManagePrivileges,
                                @CreateCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", privilege.UserID);
                    command.Parameters.AddWithValue("@GuildID", privilege.GuildID);
                    command.Parameters.AddWithValue("@ManageGuild", (sbyte)privilege.ManageGuild);
                    command.Parameters.AddWithValue("@ManagePrivileges", (sbyte)privilege.ManagePrivileges);
                    command.Parameters.AddWithValue("@CreateCategory", (sbyte)privilege.CreateCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateCategoryPrivilege(CategoryPrivilege privilege) {
            try {
                Logger.Warning("category privilege");
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO CategoryPrivileges (
                                UserID,
                                CategoryID,
                                ViewCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @CategoryID,
                                @ViewCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", privilege.UserID);
                    command.Parameters.AddWithValue("@CategoryID", privilege.CategoryID);
                    command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateTextChannelPrivilege(TextChannelPrivilege privilege) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO TextChannelPrivileges (
                                UserID,
                                ChannelID,
                                ViewChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @ChannelID,
                                @ViewChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", privilege.UserID);
                    command.Parameters.AddWithValue("@ChannelID", privilege.ChannelID);
                    command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateDefaultGuildPrivilege(GuildPrivilege privilege) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO DefaultGuildPrivileges (
                                GuildID,
                                ManageGuild,
                                ManagePrivileges,
                                CreateCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @GuildID,
                                @ManageGuild,
                                @ManagePrivileges,
                                @CreateCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", privilege.GuildID);
                    command.Parameters.AddWithValue("@ManageGuild", (sbyte)privilege.ManageGuild);
                    command.Parameters.AddWithValue("@ManagePrivileges", (sbyte)privilege.ManagePrivileges);
                    command.Parameters.AddWithValue("@CreateCategory", (sbyte)privilege.CreateCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateDefaultCategoryPrivilege(CategoryPrivilege privilege) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO DefaultCategoryPrivileges (
                                CategoryID,
                                ViewCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @CategoryID,
                                @ViewCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", privilege.CategoryID);
                    command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateDefaultTextChannelPrivilege(TextChannelPrivilege privilege) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO DefaultTextChannelPrivileges (
                                ChannelID,
                                ViewChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @ChannelID,
                                @ViewChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@ChannelID", privilege.ChannelID);
                    command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateGuildPrivilegeForOwner(ID ownerID, ID guildID) {
            try {
                GuildPrivilege privilege = GuildPrivilege.OwnerPrivilege(ownerID, guildID);
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO GuildPrivileges (
                                UserID,
                                GuildID,
                                ManageGuild,
                                ManagePrivileges,
                                CreateCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @GuildID,
                                @ManageGuild,
                                @ManagePrivileges,
                                @CreateCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", ownerID);
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.Parameters.AddWithValue("@ManageGuild", (sbyte)privilege.ManageGuild);
                    command.Parameters.AddWithValue("@ManagePrivileges", (sbyte)privilege.ManagePrivileges);
                    command.Parameters.AddWithValue("@CreateCategory", (sbyte)privilege.CreateCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateCategoryPrivilegeForOwner(ID ownerID, ID categoryID) {
           
            try {
                CategoryPrivilege privilege = CategoryPrivilege.OwnerPrivilege(ownerID, categoryID);
                Logger.Warning("owner category privilege");
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO CategoryPrivileges (
                                UserID,
                                CategoryID,
                                ViewCategory,
                                UpdateCategory,
                                DeleteCategory,
                                CreateChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @CategoryID,
                                @ViewCategory,
                                @UpdateCategory,
                                @DeleteCategory,
                                @CreateChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", ownerID);
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateTextChannelPrivilegeForOwner(ID ownerID, ID textChannelID) {
            try {
                TextChannelPrivilege privilege = TextChannelPrivilege.OwnerPrivilege(ownerID, textChannelID);
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO TextChannelPrivileges (
                                UserID,
                                TextChannelID,
                                ViewChannel,
                                UpdateChannel,
                                DeleteChannel,
                                Read,
                                Write
                            )
                            VALUES (
                                @UserID,
                                @TextChannelID,
                                @ViewChannel,
                                @UpdateChannel,
                                @DeleteChannel,
                                @Read,
                                @Write
                            )
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", ownerID);
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                    command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

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



        public ID CreateGuildAffiliation(ID userID, ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO GuildAffiliations (UserID, GuildID)
                            VALUES (@UserID, @GuildID)
                            RETURNING ID
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



        public ID CreateGuild(ID ownerID, string publicID, string name, string password, byte[] icon, DateTime creationTime) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO Guilds (Name, PublicID, Password, OwnerID, Icon, CreationTime)
                            VALUES (@Name, @PublicID, @Password, @OwnerID, @Icon, @CreationTime)
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@PublicID", publicID);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@OwnerID", ownerID);
                    command.Parameters.AddWithValue("@Icon", icon);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);

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



        public ID CreateCategory(ID guildID, string name, DateTime creationTime) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO Categories (GuildID, Name, CreationTime)
                            VALUES (@GuildID, @Name, @CreationTime)
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);


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



        public ID CreateTextChannel(ID categoryID, string name, DateTime creationTime) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO TextChannels (CategoryID, Name, CreationTime)
                            VALUES (@CategoryID, @Name, @CreationTime)
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);

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



        public ID CreateDefaultTextChannelPrivilegeForUser(ID userID, ID textChannelID) {
            try {
                TextChannelPrivilege? privilege = GetDefaultTextChannelPrivilege(textChannelID);
                if (privilege == null) {
                    Logger.Error($"Failed to get default text channel privilege for text channel '{textChannelID}'.", MethodBase.GetCurrentMethod());
                    return 0;
                }

                privilege.UserID = userID;

                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO TextChannelPrivileges (UserID, ChannelID, ViewChannel, UpdateChannel, DeleteChannel, Read, Write)
                            VALUES (@UserID, @TextChannelID, @ViewChannel, @UpdateChannel, @DeleteChannel, @Read, @Write)
                            RETURNING ID
                            ;";
                    command.Parameters.AddWithValue("@UserID", privilege.UserID);
                    command.Parameters.AddWithValue("@TextChannelID", privilege.ChannelID);
                    command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            ID privilegeID = reader.GetFieldValue<ID>(0);
                            return privilegeID;
                        }
                    }
                }
                Logger.Error($"Failed to create default text channel privilege for user '{userID}'.", MethodBase.GetCurrentMethod());
                return 0;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return 0;
            }
        }



        public ID CreateDefaultCategoryPrivilegeForUser(ID userID, ID categoryID) {
            try {
                CategoryPrivilege? privilege = GetDefaultCategoryPrivilege(categoryID);
                if (privilege == null) {
                    Logger.Error($"Failed to get default category privilege for user '{userID}' in category '{categoryID}'.", MethodBase.GetCurrentMethod());
                    return 0;
                }

                privilege.UserID = userID;

                Logger.Warning("default category privilege for user");

                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                            INSERT INTO CategoryPrivileges (UserID, CategoryID, ViewCategory, UpdateCategory, DeleteCategory, CreateChannel, UpdateChannel, DeleteChannel, Read, Write)
                                VALUES (@UserID, @CategoryID, @ViewCategory, @UpdateCategory, @DeleteCategory, @CreateChannel, @UpdateChannel, @DeleteChannel, @Read, @Write)
                                RETURNING ID;";
                    command.Parameters.AddWithValue("@UserID", (sbyte)privilege.UserID);
                    command.Parameters.AddWithValue("@CategoryID", (sbyte)privilege.CategoryID);
                    command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                    command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                    command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                    command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                    command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                    command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            ID privilegeID = reader.GetFieldValue<ID>(0);
                            return privilegeID;
                        }
                    }
                }
                Logger.Error($"Failed to create default category privilege for user '{userID}' in category '{categoryID}'.", MethodBase.GetCurrentMethod());
                return 0;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return 0;
            }
        }
    }
}
