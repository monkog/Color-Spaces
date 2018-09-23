using System.Drawing;
using ColorSpaces.ImageConverters;
using NUnit.Framework;

namespace ColorSpacesTests.ImageConverters
{
	[TestFixture]
	public class ColorReducerTests
	{
		[Test]
		public void ReduceColor_SingleShadeOfAllColors_SameValueForAll()
		{
			var color = Color.Green;

			var result = color.Reduce(1, 1, 1, 256, 256, 256);

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(128, result.R);
			Assert.AreEqual(128, result.G);
			Assert.AreEqual(128, result.B);
		}

		[Test]
		public void ReduceColor_AllColors_SameColor()
		{
			var color = Color.Brown;

			var result = color.Reduce(256, 256, 256, 1, 1, 1);

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(color.R, result.R);
			Assert.AreEqual(color.G, result.G);
			Assert.AreEqual(color.B, result.B);
		}

		[Test]
		public void ReduceColor_BlackAllColors_SameColor()
		{
			var color = Color.Black;

			var result = color.Reduce(256, 256, 256, 1, 1, 1);

			Assert.AreEqual(color.A, result.A);
			Assert.AreEqual(color.R, result.R);
			Assert.AreEqual(color.G, result.G);
			Assert.AreEqual(color.B, result.B);
		}
	}
}