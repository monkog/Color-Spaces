using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorSpaces.Helpers
{
	public static class BitmapExtensions
	{
		/// <summary>
		/// Creates BitmapImage from provided Bitmap.
		/// </summary>
		/// <param name="bitmap">Provided Bitmap</param>
		/// <returns>Converted BitmapImage</returns>
		[ExcludeFromCodeCoverage]
		public static ImageBrush CreateImageBrush(this Bitmap bitmap)
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
		[ExcludeFromCodeCoverage]
		public static Bitmap CreateBitmap(this BitmapImage bitmapImage)
		{
			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
				bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
				bitmapEncoder.Save(outStream);
				var bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}
	}
}