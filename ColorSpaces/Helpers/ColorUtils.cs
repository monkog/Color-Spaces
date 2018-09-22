namespace ColorSpaces.Helpers
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
	}
}