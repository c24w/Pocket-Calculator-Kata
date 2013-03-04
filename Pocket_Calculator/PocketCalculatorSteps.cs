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
			var operatorMap = new Dictionary<string, Operator>
			{
				{"+", Operator.Add}, {"-", Operator.Subtract}, {"/", Operator.Divide}, {"*", Operator.Multiply}, {"=", Operator.Equal}
			};

			var commandMap = new Dictionary<string, Commands>
			{
				{"AC", Commands.ClearAll}, {"+/-", Commands.FlipSign}
			};

			_pocketCalculator = new PocketCalculator(commandMap, operatorMap);
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