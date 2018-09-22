using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;

namespace ColorSpaces
{
	public partial class MainWindow
    {
        /// <summary>
        /// Creates BitmapImage from provided Bitmap.
        /// </summary>
        /// <param name="bitmap">Provided Bitmap</param>
        /// <returns>Converted BitmapImage</returns>
        private static ImageBrush createImageBrushFromBitmap(Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap()
                , IntPtr.Zero
                , Int32Rect.Empty
                , BitmapSizeOptions.FromEmptyOptions());

            return new ImageBrush(bitmapSource);
        }

        /// <summary>
        /// Creates Bitmap from the provided BitmapImage.
        /// </summary>
        /// <param name="bitmapImage">Provided BitmapImage</param>
        /// <returns>Bitmap from BitmapImage</returns>
        private Bitmap createBitmapFromBitmapImage(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                bitmapEncoder.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// Returns R,G and B values in range [0,255].
        /// </summary>
        /// <param name="n">R,G,B value in range [0,1]</param>
        /// <returns>R,G,B value in range [0,255]</returns>
        private static double toRGB(double n)
        {
            var result = 255.0 * n;

            if (result >= 0)
                if (result <= 255)
                    return result;
                else
                    return 255;
            return 0;
        }

        /// <summary>
        /// Converts image to gray-scale using average algoithm.
        /// </summary>
        void convertToGrayScale()
        {
            Bitmap outputBitmap = new Bitmap((int)m_sourceBitmap.Width, (int)m_sourceBitmap.Height);

            for (int i = 0; i < outputBitmap.Width; i++)
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    Color color = m_sourceBitmap.GetPixel(i, j);
                    int grayColor = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
                    color = Color.FromArgb(color.A, grayColor, grayColor, grayColor);
                    outputBitmap.SetPixel(i, j, color);
                }

            m_outputPhoto.Background = createImageBrushFromBitmap(outputBitmap);
        }

        /// <summary>
        /// Converts color space of an image to the chosen color space.
        /// </summary>
        /// <param name="colorSpace">The chosen color space</param>
        void convertToColorSpace(string colorSpace)
        {
            Bitmap outputBitmap = new Bitmap((int)m_sourceBitmap.Width, (int)m_sourceBitmap.Height);

            for (int i = 0; i < outputBitmap.Width; i++)
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    Color color = m_sourceBitmap.GetPixel(i, j);
                    double[] XYZ;
                    getXYZ(color, out XYZ);
                    switch (colorSpace)
                    {
                        case "Adobe RGB":
                            color = toAdobeSpace(XYZ);
                            break;
                        case "Apple RGB":
                            color = toAppleSpace(XYZ);
                            break;
                        case "Wide Gamut":
                            color = toWideGamutSpace(XYZ);
                            break;
                    }
                    outputBitmap.SetPixel(i, j, color);
                }

            m_outputPhoto.Background = createImageBrushFromBitmap(outputBitmap);
        }

        /// <summary>
        /// Returns the provided color in XYZ coordinates.
        /// Uses gamma conversion to optimize the color.
        /// </summary>
        /// <param name="color">Provided color</param>
        /// <param name="XYZ">Output table with XYZ coordinates of the provided color</param>
        void getXYZ(Color color, out double[] XYZ)
        {
            // Gamma correction
            //double r = (color.R / 255.0 > 0.04045 ? Math.Pow((color.R / 255.0 + 0.055) / 1.055, 2.4) : color.R / 255.0 / 12.92);
            //double g = (color.G / 255.0 > 0.04045 ? Math.Pow((color.G / 255.0 + 0.055) / 1.055, 2.4) : color.G / 255.0 / 12.92);
            //double b = (color.B / 255.0 > 0.04045 ? Math.Pow((color.B / 255.0 + 0.055) / 1.055, 2.4) : color.B / 255.0 / 12.92);
            double r = Math.Pow(color.R / 255.0, 2.2);
            double g = Math.Pow(color.G / 255.0, 2.2);
            double b = Math.Pow(color.B / 255.0, 2.2);

            // Basic matrix to calculate the coordinates with D65 white point.
            // W won't use this matrix in this case.
            // We could use this matrix with Adobe RGB or Apple RGB conversion 
            // because white points match within these three spaces. 
            // Although we can't use it with Wide Gamut convertion without another Bradford matrix.
            // double X = r * 0.4124 + g * 0.3576 + b * 0.1805;
            // double Y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            // double Z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            // Assuming that white point is D50.
            // Uses the Bradford-adapted matrix.
            double X = r * 0.4360747 + g * 0.3850649 + b * 0.1430804;
            double Y = r * 0.2225045 + g * 0.7168786 + b * 0.0606169;
            double Z = r * 0.0139322 + g * 0.0971045 + b * 0.7141733;

            XYZ = new double[3] { X, Y, Z };
        }

        /// <summary>
        /// Converts provided color in XYZ coordinates to corresponding color in Adobe RGB color space.
        /// </summary>
        /// <param name="XYZ">Provided color in XYZ coordinates</param>
        /// <returns>Color in Adobe RGB color space</returns>
        Color toAdobeSpace(double[] XYZ)
        {
            // Basic matrix to calculate the coordinates with D65 white point.
            // W won't use this matrix in this case.
            // double r = XYZ[0] * 2.041369 + XYZ[1] * -0.5649464 + XYZ[2] * -0.3446944;
            // double g = XYZ[0] * -0.9692660 + XYZ[1] * 1.8760108 + XYZ[2] * 0.041556;
            // double b = XYZ[0] * 0.0134474 + XYZ[1] * -0.1183897 + XYZ[2] * 1.0154096;

            // Assuming that white point is D50.
            // Uses the Bradford-adapted matrix.
            double r = XYZ[0] * 1.9624274 + XYZ[1] * -0.6105343 + XYZ[2] * -0.3413404;
            double g = XYZ[0] * -0.9787684 + XYZ[1] * 1.9161415 + XYZ[2] * 0.0334540;
            double b = XYZ[0] * 0.0286869 + XYZ[1] * -0.1406752 + XYZ[2] * 1.3487655;

            // Gamma correction
            r = Math.Pow(r, 1 / 2.2);
            g = Math.Pow(g, 1 / 2.2);
            b = Math.Pow(b, 1 / 2.2);

            return Color.FromArgb(255, (int)toRGB(r), (int)toRGB(g), (int)toRGB(b));
        }

        /// <summary>
        /// Converts provided color in XYZ coordinates to corresponding color in Apple RGB color space.
        /// </summary>
        /// <param name="XYZ">Provided color in XYZ coordinates</param>
        /// <returns>Color in Apple RGB color space</returns>
        Color toAppleSpace(double[] XYZ)
        {
            // Basic matrix to calculate the coordinates with D65 white point.
            // W won't use this matrix in this case.
            // double r = XYZ[0] * 2.9515373 + XYZ[1] * -1.2894116 + XYZ[2] * -0.4738445;
            // double g = XYZ[0] * -1.0851093 + XYZ[1] * 1.9908566 + XYZ[2] * 0.0372026;
            // double b = XYZ[0] * 0.0854934 + XYZ[1] * -0.2694964 + XYZ[2] * 1.0912975;

            // Assuming that white point is D50.
            // Uses the Bradford-adapted matrix.
            double r = XYZ[0] * 2.8510695 + XYZ[1] * -1.3605261 + XYZ[2] * -0.4708281;
            double g = XYZ[0] * -1.092768 + XYZ[1] * 2.0348871 + XYZ[2] * 0.0227598;
            double b = XYZ[0] * 0.1027403 + XYZ[1] * -0.2964984 + XYZ[2] * 1.4510659;

            // Gamma correction
            r = Math.Pow(r, 1 / 1.8);
            g = Math.Pow(g, 1 / 1.8);
            b = Math.Pow(b, 1 / 1.8);

            return Color.FromArgb(255, (int)toRGB(r), (int)toRGB(g), (int)toRGB(b));
        }

        /// <summary>
        /// Converts provided color in XYZ coordinates to corresponding color in Wide Gamut color space.
        /// </summary>
        /// <param name="XYZ">Provided color in XYZ coordinates</param>
        /// <returns>Color in Wide Gamut color space</returns>
        Color toWideGamutSpace(double[] XYZ)
        {
            // Assuming that white point is D50.
            // Uses the Bradford-adapted matrix.
            double r = XYZ[0] * 1.4628067 + XYZ[1] * -0.1840623 + XYZ[2] * -0.2743606;
            double g = XYZ[0] * -0.5217933 + XYZ[1] * 1.4472381 + XYZ[2] * 0.0677227;
            double b = XYZ[0] * 0.0349342 + XYZ[1] * -0.096893 + XYZ[2] * 1.2884099;

            // Gamma correction
            r = Math.Pow(r, 1 / 1.2);
            g = Math.Pow(g, 1 / 1.2);
            b = Math.Pow(b, 1 / 1.2);

            return Color.FromArgb(255, (int)toRGB(r), (int)toRGB(g), (int)toRGB(b));
        }

        /// <summary>
        /// Saves the converted image with .bmp extension.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        void saveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            saveUsingEncoder(visual, fileName, encoder);
        }

        /// <summary>
        /// Saves the converted image with .png extension.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        void saveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            saveUsingEncoder(visual, fileName, encoder);
        }

        /// <summary>
        /// Saves the converted image with .jpeg extension.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        void saveToJpeg(FrameworkElement visual, string fileName)
        {
            var encoder = new JpegBitmapEncoder();
            saveUsingEncoder(visual, fileName, encoder);
        }

        /// <summary>
        /// Saves the converted image with .gif extension.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        void saveToGif(FrameworkElement visual, string fileName)
        {
            var encoder = new GifBitmapEncoder();
            saveUsingEncoder(visual, fileName, encoder);
        }

        /// <summary>
        /// Universal function to save image with a chosen extention.
        /// </summary>
        /// <param name="visual">Image to save</param>
        /// <param name="fileName">Output file name</param>
        /// <param name="encoder">Matching encoder</param>
        void saveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
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
        void reduceColors(int kR, int kG, int kB)
        {
            int intervalR = (int)(255 / kR);
            int intervalG = (int)(255 / kG);
            int intervalB = (int)(255 / kB);

            Bitmap outputBitmap = new Bitmap((int)m_sourceBitmap.Width, (int)m_sourceBitmap.Height);

            for (int i = 0; i < outputBitmap.Width; i++)
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    Color color = m_sourceBitmap.GetPixel(i, j);
                    int rI = color.R / intervalR;
                    int gI = color.G / intervalG;
                    int bI = color.B / intervalB;

                    if (rI == kR) rI--;
                    if (gI == kG) gI--;
                    if (bI == kB) bI--;

                    int r = (int)toRGB((rI + 0.5) * intervalR / 255);
                    int g = (int)toRGB((gI + 0.5) * intervalG / 255);
                    int b = (int)toRGB((bI + 0.5) * intervalB / 255);
                    color = Color.FromArgb(255, r, g, b);
                    outputBitmap.SetPixel(i, j, color);
                }

            m_outputPhoto.Background = createImageBrushFromBitmap(outputBitmap);
        }
    }
}