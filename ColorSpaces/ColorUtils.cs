using System.Drawing;

namespace ColorSpaces
{
	public static class ColorUtils
	{
		/// <summary>
		/// Returns R,G and B values in range [0,255].
		/// </summary>
		/// <param name="n">R,G,B value in range [0,1]</param>
		/// <returns>R,G,B value in range [0,255]</returns>
		public static int ToRgb(this double n)
		{
			var result = (int)(255.0 * n);

			if (result <= 0) return 0;
			if (result >= 255) return 255;

			return result;
		}

		/// <summary>
		/// Converts the provided color to the greyscale.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns>Greyscale representation.</returns>
		public static Color ToGreyscale(this Color color)
		{
			var gray = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
			color = Color.FromArgb(color.A, gray, gray, gray);
			return color;
		}
	}
}