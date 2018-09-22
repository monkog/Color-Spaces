using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorSpaces.Helpers;
using ColorSpaces.ImageConverters;
using Color = System.Drawing.Color;

namespace ColorSpaces
{
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

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
	        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
	        {
		        Filter = "all image files(*.bmp; *.gif; *.jpeg; *.jpg; *.png)|*.bmp;*.gif; *.jpeg; *.jpg; *.png"
		                 + "|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png"
	        };

			bool? result = openFileDialog.ShowDialog();

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

        private void ConvertToGrayScaleButton_Click(object sender, RoutedEventArgs e)
        {
	        if (_sourceBitmap == null) return;
	        OutputPhoto.Background = _sourceBitmap.ToGrayScale();
        }

        private void ConvertToAdobeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.AdobeRgb);
        }

        private void ConvertToAppleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.AppleRgb);
        }

        private void ConvertToWideGamutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceBitmap != null)
                ConvertToColorSpace(ColorSpace.WideGamut);
        }

        private void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
	        if (_sourceBitmap == null) return;
	        OutputPhoto.Background = _sourceBitmap.ReduceColors(int.Parse(Kr.Text), int.Parse(Kg.Text), int.Parse(Kb.Text));
		}

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
	}
}