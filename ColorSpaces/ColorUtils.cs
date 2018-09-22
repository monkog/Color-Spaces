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
		/// Reduces number of colors to kR * kG * kB.
		/// </summary>
		/// <param name="kR">Number of red intervals</param>
		/// <param name="kG">Number of green intervals</param>
		/// <param name="kB">Number of blue intervals</param>
		/// <param name="color">Color to reduce.</param>
		/// <param name="intervalR">Red interval span.</param>
		/// <param name="intervalG">Green interval span.</param>
		/// <param name="intervalB">Blue interval span.</param>
		public static Color ReduceColor(this Color color, int kR, int kG, int kB, int intervalR, int intervalG, int intervalB)
		{
			var rI = color.R / intervalR;
			var gI = color.G / intervalG;
			var bI = color.B / intervalB;

			if (rI == kR) rI--;
			if (gI == kG) gI--;
			if (bI == kB) bI--;

			var r = ((rI + 0.5) * intervalR / 255).ToRgb();
			var g = ((gI + 0.5) * intervalG / 255).ToRgb();
			var b = ((bI + 0.5) * intervalB / 255).ToRgb();
			return Color.FromArgb(color.A, r, g, b);
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

		/// <summary>
		/// Converts provided color in XYZ coordinates to corresponding color in Adobe RGB color space.
		/// </summary>
		/// <param name="xyz">Provided color in XYZ coordinates</param>
		/// <returns>Color in Adobe RGB color space</returns>
		public static Color ToAdobeSpace(this double[] xyz)
		{
			// Assuming that white point is D50.
			// Uses the Bradford-adapted XYZ to RGB matrix.
			double r = xyz[0] * 1.9624274 + xyz[1] * -0.6105343 + xyz[2] * -0.3413404;
			double g = xyz[0] * -0.9787684 + xyz[1] * 1.9161415 + xyz[2] * 0.0334540;
			double b = xyz[0] * 0.0286869 + xyz[1] * -0.1406752 + xyz[2] * 1.3487655;

			// Gamma correction
			r = Math.Pow(r, 1 / 2.2);
			g = Math.Pow(g, 1 / 2.2);
			b = Math.Pow(b, 1 / 2.2);

			return Color.FromArgb(255, r.ToRgb(), g.ToRgb(), b.ToRgb());
		}

		/// <summary>
		/// Converts provided color in XYZ coordinates to corresponding color in Apple RGB color space.
		/// </summary>
		/// <param name="xyz">Provided color in XYZ coordinates</param>
		/// <returns>Color in Apple RGB color space</returns>
		public static Color ToAppleSpace(this double[] xyz)
		{
			// Assuming that white point is D50.
			// Uses the Bradford-adapted XYZ to RGB matrix.
			double r = xyz[0] * 2.8510695 + xyz[1] * -1.3605261 + xyz[2] * -0.4708281;
			double g = xyz[0] * -1.092768 + xyz[1] * 2.0348871 + xyz[2] * 0.0227598;
			double b = xyz[0] * 0.1027403 + xyz[1] * -0.2964984 + xyz[2] * 1.4510659;

			// Gamma correction
			r = Math.Pow(r, 1 / 1.8);
			g = Math.Pow(g, 1 / 1.8);
			b = Math.Pow(b, 1 / 1.8);

			return Color.FromArgb(255, r.ToRgb(), g.ToRgb(), b.ToRgb());
		}

		/// <summary>
		/// Converts provided color in XYZ coordinates to corresponding color in Wide Gamut color space.
		/// </summary>
		/// <param name="xyz">Provided color in XYZ coordinates</param>
		/// <returns>Color in Wide Gamut color space</returns>
		public static Color ToWideGamutSpace(this double[] xyz)
		{
			// Assuming that white point is D50.
			// Uses the Bradford-adapted XYZ to RGB matrix.
			double r = xyz[0] * 1.4628067 + xyz[1] * -0.1840623 + xyz[2] * -0.2743606;
			double g = xyz[0] * -0.5217933 + xyz[1] * 1.4472381 + xyz[2] * 0.0677227;
			double b = xyz[0] * 0.0349342 + xyz[1] * -0.096893 + xyz[2] * 1.2884099;

			// Gamma correction
			r = Math.Pow(r, 1 / 1.2);
			g = Math.Pow(g, 1 / 1.2);
			b = Math.Pow(b, 1 / 1.2);

			return Color.FromArgb(255, r.ToRgb(), g.ToRgb(), b.ToRgb());
		}
	}
}