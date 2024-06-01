

namespace ChatServer {
    public static class ResourceHelper {

        public static byte[] GetDefaultImage() {
            string filePath = "Resources/Images/Default.png";
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("File not found.", filePath);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader br = new BinaryReader(fs)) {
                    if (fs.Length > int.MaxValue) {
                        throw new Exception("File is too large.");
                    }
                    return br.ReadBytes((int)fs.Length);
                }
            }
        }

        public static byte[] GetDefaultGuildIcon() {
            return GetDefaultImage();
        }

        public static byte[] GetDefaultProfilePicture() {
            return GetDefaultImage();
        }

    }
}
