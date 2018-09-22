using ColorSpaces.Helpers;
using NUnit.Framework;

namespace ColorSpacesTests.Helpers
{
	[TestFixture]
	public class ColorUtilsTests
	{
		[Test]
		[TestCase(-0.1, 0)]
		[TestCase(0, 0)]
		[TestCase(0.2, 51)]
		[TestCase(1, 255)]
		[TestCase(2.5, 255)]
		public void ToRgb_Parameter_Result(double parameter, int expectedResult)
		{
			var result = parameter.ToRgb();
			Assert.AreEqual(expectedResult, result);
		}
	}
}
