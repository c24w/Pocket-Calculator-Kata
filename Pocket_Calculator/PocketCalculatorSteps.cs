using System.Collections.Generic;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Pocket_Calculator
{
	[Binding]
	public class PocketCalculatorSteps
	{
		private PocketCalculator _pocketCalculator;

		[Given(@"I have a pocket calculator")]
		public void GivenIHaveAPocketCalculator()
		{
			_pocketCalculator = new PocketCalculator();
		}

		[Given(@"it is turned on")]
		public void GivenItIsTurnedOn()
		{
			_pocketCalculator.Process("AC");
		}

		[When(@"I press ""(.*)""")]
		public void WhenIPress(string data)
		{
			_pocketCalculator.Process(data);
		}

		[Then(@"the display shows ""(.*)""")]
		public void ThenTheDisplayShows(string display)
		{
			Assert.That(_pocketCalculator.Display, Is.EqualTo(display));
		}

	}
}
