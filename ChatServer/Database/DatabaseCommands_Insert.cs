using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatServer.Database {
    public partial class DatabaseCommands {
        private ulong Insert(string tableName, params Entry[] entries) {
            StringBuilder query = new($"INSERT INTO {tableName}(");
            for (int i = 0; i < entries.Length; i++) {
                if (i != (entries.Length - 1)) {
                    query.Append($"{entries[i].Key}, ");
                    continue;
                }
                query.Append($"{entries[i].Key}) VALUES (");
            }
            for (int i = 0; i < entries.Length; i++) {
                if (i != (entries.Length - 1)) {
                    query.Append($"@{entries[i].Key}, ");
                    continue;
                }
                query.Append($"@{entries[i].Key}) RETURNING ID;");
            }

            using var command = Connection.CreateCommand();
            command.CommandText = query.ToString();

            for (int i = 0; i < entries.Length; i++) {
                command.Parameters.AddWithValue($"@{entries[i].Key}", entries[i].Value);
            }

            ulong id = 0;
            try {
                var reader = command.ExecuteReader();
                if (reader is null) {
                    return 0;
                }
                if (reader.Read()) {
                    id = reader.GetFieldValue<ulong>(0);
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }

            return id;
        }


        public ulong InsertUser(RegisterData data, DateTime? creationTime = null) {
            if (creationTime == null) { creationTime = DateTime.Now; }
            return Insert("Users",
                new Entry("PublicID", Guid.NewGuid()),
                new Entry("Email", data.Email),
                new Entry("Password", data.Password),
                new Entry("Nickname", data.Nickname),
                new Entry("Pronoun", data.Pronoun),
                new Entry("CreationTime", creationTime),
                new Entry("ProfilePicture", data.ProfilePicture),
                new Entry("Status", UserStatus.Offline));
        }


        public ulong InsertGuildPrivilege(GuildPrivilege privilege) {
            return Insert("GuildPrivileges",
                new Entry("UserID", privilege.UserID),
                new Entry("GuildID", privilege.GuildID),
                new Entry("ManageGuild", privilege.ManageGuild),
                new Entry("ManagePrivileges", privilege.ManagePrivileges),
                new Entry("CreateCategory", privilege.CreateCategory),
                new Entry("UpdateCategory", privilege.UpdateCategory),
                new Entry("DeleteCategory", privilege.DeleteCategory),
                new Entry("CreateChannel", privilege.CreateChannel),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }

        public ulong InsertCategoryPrivilege(CategoryPrivilege privilege) {
            return Insert("CategoryPrivileges",
                new Entry("UserID", privilege.UserID),
                new Entry("CategoryID", privilege.CategoryID),
                new Entry("UpdateCategory", privilege.UpdateCategory),
                new Entry("DeleteCategory", privilege.DeleteCategory),
                new Entry("ViewCategory", privilege.ViewCategory),
                new Entry("CreateChannel", privilege.CreateChannel),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }

        public ulong InsertTextChannelPrivilege(TextChannelPrivilege privilege) {
            Logger.Warning("WRITE: " + privilege.Write.ToString());
            return Insert("TextChannelPrivileges",
                new Entry("UserID", privilege.UserID),
                new Entry("ChannelID", privilege.ChannelID),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("ViewChannel", privilege.ViewChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }

        public ulong InsertDefaultGuildPrivilege(GuildPrivilege privilege) {
            return Insert("DefaultGuildPrivileges",
                new Entry("GuildID", privilege.GuildID),
                new Entry("ManageGuild", privilege.ManageGuild),
                new Entry("ManagePrivileges", privilege.ManagePrivileges),
                new Entry("CreateCategory", privilege.CreateCategory),
                new Entry("UpdateCategory", privilege.UpdateCategory),
                new Entry("DeleteCategory", privilege.DeleteCategory),
                new Entry("CreateChannel", privilege.CreateChannel),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }

        public ulong InsertDefaultCategoryPrivilege(CategoryPrivilege privilege) {
            return Insert("DefaultCategoryPrivileges",
                new Entry("CategoryID", privilege.CategoryID),
                new Entry("UpdateCategory", privilege.UpdateCategory),
                new Entry("DeleteCategory", privilege.DeleteCategory),
                new Entry("ViewCategory", privilege.ViewCategory),
                new Entry("CreateChannel", privilege.CreateChannel),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }

        public ulong InsertDefaultTextChannelPrivilege(TextChannelPrivilege privilege) {
            return Insert("DefaultTextChannelPrivileges",
                new Entry("ChannelID", privilege.ChannelID),
                new Entry("UpdateChannel", privilege.UpdateChannel),
                new Entry("DeleteChannel", privilege.DeleteChannel),
                new Entry("ViewChannel", privilege.ViewChannel),
                new Entry("Read", privilege.Read),
                new Entry("Write", privilege.Write)
                );
        }



    }
}
