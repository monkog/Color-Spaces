using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorSpaces.Helpers;
using Color = System.Drawing.Color;

namespace ColorSpaces
{
	/// <summary>
	/// Author: Monika Kogut
	/// This demo converts the color space of a picture.
	/// Most of the information was provided from the articles:
	/// http://www.babelcolor.com/download/A%20comparison%20of%20four%20multimedia%20RGB%20spaces.pdf
	/// http://www.babelcolor.com/download/A%20review%20of%20RGB%20color%20spaces.pdf
	/// and the site:
	/// http://www.brucelindbloom.com/index.html?Eqn_RGB_XYZ_Matrix.html
	/// </summary>
	public partial class MainWindow : Window
    {
        Bitmap _sourceBitmap;
	    readonly ImageBrush _whiteSmokeBitmap;

        public MainWindow()
        {
            InitializeComponent();
            _sourceBitmap = null;
            var grayBitmap = new Bitmap(1, 1);
            grayBitmap.SetPixel(0, 0, Color.WhiteSmoke);
            _whiteSmokeBitmap = grayBitmap.CreateImageBrush();
        }

        private void _openButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
	        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
	        {
		        Filter = "all image files(*.bmp; *.gif; *.jpeg; *.jpg; *.png)|*.bmp;*.gif; *.jpeg; *.jpg; *.png"
		                 + "|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png"
	        };

	        // Set filter for file extension and default file extension 

	        // Display OpenFileDialog by calling ShowDialog method 
			bool? result = openFileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmapImage = new BitmapImage(new Uri(fileName));
                imageBrush.ImageSource = bitmapImage;
                SourcePhoto.Background = imageBrush;
                _sourceBitmap = bitmapImage.CreateBitmap();
                OutputPhoto.Background = _whiteSmokeBitmap;
            }
        }

        private void m_saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap == null || OutputPhoto.Background == _whiteSmokeBitmap)
                return;

	        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
	        {
		        Filter =
			        "BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png"
	        };
	        bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string fileName = saveFileDialog.FileName;
                string extension = Path.GetExtension(fileName);
	            BitmapEncoder encoder;

                switch (extension)
                {
                    case ".bmp":
	                    encoder = new BmpBitmapEncoder();
						break;
                    case ".gif":
	                    encoder = new GifBitmapEncoder();
						break;
                    case ".jpeg":
	                    encoder = new JpegBitmapEncoder();
                        break;
                    case ".png":
	                    encoder = new PngBitmapEncoder();
						break;
					default:
						throw new NotSupportedException($"{extension} is not supported.");
                }

	            SaveToFile(OutputPhoto, fileName, encoder);
            }
        }

        private void _convertToGrayScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToGrayScale();
        }

        private void _convertToAdobeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.AdobeRgb);
        }

        private void _convertToAppleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.AppleRgb);
        }

        private void _convertToWideGamutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.WideGamut);
        }

        private void _reduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ReduceColors(int.Parse(Kr.Text), int.Parse(Kg.Text), int.Parse(Kb.Text));
        }
    }
}
