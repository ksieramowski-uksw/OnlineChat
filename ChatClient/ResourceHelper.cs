using System.IO;
using System.Windows.Media.Imaging;


namespace ChatClient {
    public static class ResourceHelper {
        public static byte[] GetImagePixels(string path) {
            BitmapImage bitmap = new(new Uri(path));
            byte[] pixels;
            using (MemoryStream stream = new()) {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                pixels = stream.ToArray();
            }
            return pixels;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray) {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(byteArray);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }


    }
}
