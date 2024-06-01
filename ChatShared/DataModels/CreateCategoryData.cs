using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class CreateCategoryData {
        public ID GuildID { get; set; }
        public string Name { get; set; }

        public CategoryPrivilege DefaultPrivilege { get; set; }

        public CreateCategoryData(ID guildID, string name, CategoryPrivilege defaultPrivilege) {
            GuildID = guildID;
            Name = name;
            DefaultPrivilege = defaultPrivilege;
        }
    }
}
