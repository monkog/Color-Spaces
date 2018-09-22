using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Media;
using ColorSpaces.Helpers;
using Color = System.Drawing.Color;

namespace ColorSpaces.ImageConverters
{
	public static class GrayScaleConverter
	{
		/// <summary>
		/// Converts image to gray-scale using average algorithm.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public static ImageBrush ToGrayScale(this Bitmap source)
		{
			Bitmap outputBitmap = new Bitmap(source.Width, source.Height);

			for (int i = 0; i < outputBitmap.Width; i++)
				for (int j = 0; j < outputBitmap.Height; j++)
				{
					var color = source.GetPixel(i, j).ToGrayScale();
					outputBitmap.SetPixel(i, j, color);
				}

			return outputBitmap.CreateImageBrush();
		}

		/// <summary>
		/// Converts the provided color to the gray scale.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns>Gray scale representation.</returns>
		public static Color ToGrayScale(this Color color)
		{
			var gray = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
			color = Color.FromArgb(color.A, gray, gray, gray);
			return color;
		}
	}
}