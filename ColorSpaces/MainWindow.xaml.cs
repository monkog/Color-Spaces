using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorSpaces.Helpers;
using ColorSpaces.ImageConverters;
using Microsoft.Win32;
using Color = System.Drawing.Color;

namespace ColorSpaces
{
	[ExcludeFromCodeCoverage]
	public partial class MainWindow
	{
		private Bitmap _sourceBitmap;
		private readonly ImageBrush _whiteSmokeBitmap;

		public ICommand OpenCommand => new RelayCommand(OpenFile);

		public ICommand SaveCommand => new RelayCommand(SaveFile);

		public ICommand ConvertColorSpaceCommand => new RelayCommand<ColorSpace>(ConvertToColorSpace);

		public ICommand ConvertToGrayScaleCommand => new RelayCommand(ConvertToGrayScale);

		public ICommand ReduceColorsCommand => new RelayCommand(ReduceColors);

		public int Kr { get; set; }

		public int Kg { get; set; }

		public int Kb { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			_sourceBitmap = null;
			var grayBitmap = new Bitmap(1, 1);
			grayBitmap.SetPixel(0, 0, Color.WhiteSmoke);
			_whiteSmokeBitmap = grayBitmap.CreateImageBrush();
			DataContext = this;

			Kr = 4;
			Kg = 2;
			Kb = 4;
		}

		private void OpenFile()
		{
			var openFileDialog = new OpenFileDialog
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

		private void SaveFile()
		{
			if (_sourceBitmap == null || OutputPhoto.Background == _whiteSmokeBitmap)
				return;

			var saveFileDialog = new SaveFileDialog
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

		private void SaveToFile(FrameworkElement visual, string fileName, BitmapEncoder encoder)
		{
			var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			bitmap.Render(visual);
			var frame = BitmapFrame.Create(bitmap);
			encoder.Frames.Add(frame);

			try
			{
				using (var stream = File.Create(fileName))
				{
					encoder.Save(stream);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Cannot access the file. Try again later.");
			}
		}

		private void ConvertToGrayScale()
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

		private void ReduceColors()
		{
			if (_sourceBitmap == null) return;
			OutputPhoto.Background = _sourceBitmap.ReduceColors(Kr, Kg, Kb);
		}
	}
}