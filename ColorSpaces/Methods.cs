using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorSpaces.Helpers;
using Color = System.Drawing.Color;

namespace ColorSpaces
{
	public partial class MainWindow
    {
		/// <summary>
		/// Converts color space of an image to the chosen color space.
		/// </summary>
		/// <param name="colorSpace">The chosen color space</param>
		void ConvertToColorSpace(ColorSpace colorSpace)
        {
            var outputBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);

            for (int i = 0; i < outputBitmap.Width; i++)
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    var color = _sourceBitmap.GetPixel(i, j);
	                var xyz = color.ToCieXyz();
                    switch (colorSpace)
                    {
                        case ColorSpace.AdobeRgb:
                            color = xyz.ToAdobeSpace();
                            break;
                        case ColorSpace.AppleRgb:
                            color = xyz.ToAppleSpace();
                            break;
                        case ColorSpace.WideGamut:
                            color = xyz.ToWideGamutSpace();
                            break;
                    }
                    outputBitmap.SetPixel(i, j, color);
                }

            OutputPhoto.Background = outputBitmap.CreateImageBrush();
        }

        /// <summary>
        /// Universal function to save image with a chosen extension.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        /// <param name="encoder">Matching encoder</param>
        void SaveToFile(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        /// <summary>
        /// Reduces number of colors to kR * kG * kB.
        /// </summary>
        /// <param name="kR">Number of red intervals</param>
        /// <param name="kG">Number of green intervals</param>
        /// <param name="kB">Number of blue intervals</param>
        void ReduceColors(int kR, int kG, int kB)
        {
            int intervalR = 255 / kR;
            int intervalG = 255 / kG;
            int intervalB = 255 / kB;

            Bitmap outputBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);

            for (int i = 0; i < outputBitmap.Width; i++)
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    Color color = _sourceBitmap.GetPixel(i, j);
                    color = color.ReduceColor(kR, kG, kB, intervalR, intervalG, intervalB);
	                outputBitmap.SetPixel(i, j, color);
                }

            OutputPhoto.Background = outputBitmap.CreateImageBrush();
        }
    }
}