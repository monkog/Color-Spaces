using System;
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

		/// <summary>
		/// Returns the provided color in CIE XYZ color space.
		/// Uses gamma conversion to optimize the color.
		/// </summary>
		/// <param name="color">Provided color</param>
		/// <returns>Output table with XYZ coordinates of the provided color</returns>
		public static double[] ToCieXyz(this Color color)
		{
			// Gamma correction
			var r = Math.Pow(color.R / 255.0, 2.2);
			var g = Math.Pow(color.G / 255.0, 2.2);
			var b = Math.Pow(color.B / 255.0, 2.2);

			// Assuming that white point is D50.
			// Use the Bradford-adapted RGB to XYZ matrix.
			var x = r * 0.4360747 + g * 0.3850649 + b * 0.1430804;
			var y = r * 0.2225045 + g * 0.7168786 + b * 0.0606169;
			var z = r * 0.0139322 + g * 0.0971045 + b * 0.7141733;

			return new[] { x, y, z };
		}
	}
}