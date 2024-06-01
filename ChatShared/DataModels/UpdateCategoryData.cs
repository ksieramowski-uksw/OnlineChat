using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class UpdateCategoryData {
        public ID CategoryID { get; set; }
        public string Name { get; set; }

        public CategoryPrivilege? Privilege { get; set; }

        public UpdateCategoryData() {
            CategoryID = 0;
            Name = string.Empty;
            Privilege = null;
        }

        public UpdateCategoryData(ID categoryID, string name, CategoryPrivilege? privilege) {
            CategoryID = categoryID;
            Name = name;
            Privilege = privilege;
        }
    }
}
