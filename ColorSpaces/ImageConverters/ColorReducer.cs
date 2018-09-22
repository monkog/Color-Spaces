﻿using System.Drawing;
using System.Windows.Media;
using ColorSpaces.Helpers;
using Color = System.Drawing.Color;

namespace ColorSpaces.ImageConverters
{
	public static class ColorReducer
	{
		/// <summary>
		/// Reduces number of colors to kR * kG * kB.
		/// </summary>
		/// <param name="source">Source bitmap.</param>
		/// <param name="kR">Number of red intervals</param>
		/// <param name="kG">Number of green intervals</param>
		/// <param name="kB">Number of blue intervals</param>
		public static ImageBrush ReduceColors(this Bitmap source, int kR, int kG, int kB)
		{
			var intervalR = 255 / kR;
			var intervalG = 255 / kG;
			var intervalB = 255 / kB;

			var outputBitmap = new Bitmap(source.Width, source.Height);

			for (int i = 0; i < outputBitmap.Width; i++)
				for (int j = 0; j < outputBitmap.Height; j++)
				{
					var color = source.GetPixel(i, j);
					color = color.ReduceColor(kR, kG, kB, intervalR, intervalG, intervalB);
					outputBitmap.SetPixel(i, j, color);
				}

			return outputBitmap.CreateImageBrush();
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
	}
}
