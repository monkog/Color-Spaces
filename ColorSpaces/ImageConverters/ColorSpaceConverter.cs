using System;
using System.Drawing;
using ColorSpaces.Helpers;

namespace ColorSpaces.ImageConverters
{
	public static class ColorSpaceConverter
	{
		/// <summary>
		/// Converts the given color to the desired color space.
		/// </summary>
		/// <param name="xyz">Color in CIE XYZ color space.</param>
		/// <param name="colorSpace">Color space to convert to.</param>
		/// <returns>Color in desired color space.</returns>
		public static Color ConvertTo(this double[] xyz, ColorSpace colorSpace)
		{
			switch (colorSpace)
			{
				case ColorSpace.AdobeRgb:
					return xyz.ToAdobeSpace();
				case ColorSpace.AppleRgb:
					return xyz.ToAppleSpace();
				case ColorSpace.WideGamut:
					return xyz.ToWideGamutSpace();
				default:
					throw new ArgumentException($"The {colorSpace} is not supported.");
			}
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
		private static Color ToAdobeSpace(this double[] xyz)
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
		private static Color ToAppleSpace(this double[] xyz)
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
		private static Color ToWideGamutSpace(this double[] xyz)
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