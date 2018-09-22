using System.Drawing;
using ColorSpaces.ImageConverters;
using NUnit.Framework;

namespace ColorSpacesTests.ImageConverters
{
	[TestFixture]
	public class GrayScaleConverterTests
	{

		[Test]
		public void ToGrayScale_White_White()
		{
			var color = Color.White;
			var result = color.ToGrayScale();

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(color.R, result.R);
			Assert.AreEqual(color.G, result.G);
			Assert.AreEqual(color.B, result.B);
		}

		[Test]
		public void ToGrayScale_Black_Black()
		{
			var color = Color.Black;
			var result = color.ToGrayScale();

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(color.R, result.R);
			Assert.AreEqual(color.G, result.G);
			Assert.AreEqual(color.B, result.B);
		}

		[Test]
		public void ToGrayScale_Red_Converted()
		{
			var color = Color.Red;
			var value = (int)(0.3 * color.R);
			var result = color.ToGrayScale();

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(value, result.R);
			Assert.AreEqual(value, result.G);
			Assert.AreEqual(value, result.B);
		}

		[Test]
		public void ToGrayScale_Green_Converted()
		{
			var color = Color.Green;
			var value = (int)(0.59 * color.G);
			var result = color.ToGrayScale();

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(value, result.R);
			Assert.AreEqual(value, result.G);
			Assert.AreEqual(value, result.B);
		}

		[Test]
		public void ToGrayScale_Blue_Converted()
		{
			var color = Color.Blue;
			var value = (int)(0.11 * color.B);
			var result = color.ToGrayScale();

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(value, result.R);
			Assert.AreEqual(value, result.G);
			Assert.AreEqual(value, result.B);
		}
	}
}
