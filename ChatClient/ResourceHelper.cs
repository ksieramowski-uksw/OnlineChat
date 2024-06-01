using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;


namespace ChatClient {
    public static class ResourceHelper {
        public static string DefaultImage {
            get {
                return $"{Directory.GetCurrentDirectory()}\\Resources\\Images\\Default.png";
            }
        }

        public static byte[] GetScaledImagePixels(string filePath, int maxDimension = 96) {
            using (Image originalImage = Image.FromFile(filePath)) {
                float scaleFactor = Math.Min((float)maxDimension / originalImage.Width, (float)maxDimension / originalImage.Height);

                int newWidth = (int)(originalImage.Width * scaleFactor);
                int newHeight = (int)(originalImage.Height * scaleFactor);

                using (Bitmap scaledImage = new(newWidth, newHeight)) {
                    using (Graphics graphics = Graphics.FromImage(scaledImage)) {
                        graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                    }

                    using (MemoryStream memoryStream = new()) {
                        scaledImage.Save(memoryStream, ImageFormat.Png);
                        return CropImageToSquare(memoryStream.ToArray());
                    }
                }
            }
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray) {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(byteArray);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static byte[] CropImageToSquare(byte[] imageBytes) {
            using (MemoryStream ms = new MemoryStream(imageBytes)) {
                using (Image originalImage = Image.FromStream(ms)) {
                    int width = originalImage.Width;
                    int height = originalImage.Height;
                    int sideLength = Math.Min(width, height);

                    // Calculate coordinates for the crop area to center the square
                    int x = (width - sideLength) / 2;
                    int y = (height - sideLength) / 2;

                    Rectangle cropArea = new Rectangle(x, y, sideLength, sideLength);

                    // Crop the image
                    using (Bitmap bitmap = new Bitmap(originalImage)) {
                        using (Bitmap croppedBitmap = bitmap.Clone(cropArea, bitmap.PixelFormat)) {
                            using (MemoryStream croppedMs = new MemoryStream()) {
                                croppedBitmap.Save(croppedMs, ImageFormat.Png);
                                return croppedMs.ToArray();
                            }
                        }
                    }
                }
            }
        }

    }
}
