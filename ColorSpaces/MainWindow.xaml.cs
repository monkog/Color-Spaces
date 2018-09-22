using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
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

		public ICommand ConvertColorSpaceCommand => new RelayCommand<ColorSpace>(ConvertToColorSpace);

		public MainWindow()
		{
			InitializeComponent();
			_sourceBitmap = null;
			var grayBitmap = new Bitmap(1, 1);
			grayBitmap.SetPixel(0, 0, Color.WhiteSmoke);
			_whiteSmokeBitmap = grayBitmap.CreateImageBrush();
			DataContext = this;
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

		private void ConvertToColorSpace(ColorSpace colorSpace)
		{
			if (_sourceBitmap == null) return;

			var outputBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);

			for (int i = 0; i < outputBitmap.Width; i++)
				for (int j = 0; j < outputBitmap.Height; j++)
				{
					var color = _sourceBitmap.GetPixel(i, j);
					var xyz = color.ToCieXyz();
					var convertedColor = xyz.ConvertTo(colorSpace);
					outputBitmap.SetPixel(i, j, convertedColor);
				}

			OutputPhoto.Background = outputBitmap.CreateImageBrush();
		}

		private void ReduceButton_Click(object sender, RoutedEventArgs e)
		{
			if (_sourceBitmap == null) return;
			OutputPhoto.Background = _sourceBitmap.ReduceColors(int.Parse(Kr.Text), int.Parse(Kg.Text), int.Parse(Kb.Text));
		}
		
		private void SaveToFile(FrameworkElement visual, string fileName, BitmapEncoder encoder)
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