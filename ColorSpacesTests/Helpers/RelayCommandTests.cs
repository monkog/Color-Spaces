using ColorSpaces.Helpers;
using NUnit.Framework;

namespace ColorSpacesTests.Helpers
{
	[TestFixture]
	public class RelayCommandTests
	{
		private RelayCommand<object> _unitUnderTest;

		[SetUp]
		public void Initialize()
		{
			_unitUnderTest = new RelayCommand<object>(param => { });
		}

		[Test]
		public void CanExecute_Always_True()
		{
			var result = _unitUnderTest.CanExecute(null);
			Assert.IsTrue(result);
		}

		[Test]
		public void Execute_Parameter_Executed()
		{
			var value = string.Empty;
			var passedParam = "Hello";
			var unitUnderTest = new RelayCommand<string>(param => { value = param; });

			unitUnderTest.Execute(passedParam);

			Assert.AreEqual(passedParam, value);
		}
	}
}