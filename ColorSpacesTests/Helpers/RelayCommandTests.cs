﻿using ColorSpaces.Helpers;
using NUnit.Framework;

namespace ColorSpacesTests.Helpers
{
	[TestFixture]
	public class RelayCommandTests
	{
		private RelayCommand _unitUnderTest;

		[SetUp]
		public void Initialize()
		{
			_unitUnderTest = new RelayCommand(() => { });
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
			var expectedValue = "Hello";
			var unitUnderTest = new RelayCommand(() => { value = expectedValue; });

			unitUnderTest.Execute(null);

			Assert.AreEqual(expectedValue, value);
		}
	}
}