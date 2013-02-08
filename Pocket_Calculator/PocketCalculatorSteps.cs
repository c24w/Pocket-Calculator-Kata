using System;
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

	public class PocketCalculator
	{
		private decimal _value;

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleButton(button);
		}

		private void HandleButton(string button)
		{
			switch (button)
			{
				case "AC":
					_value = 0;
					break;
				case "=":
					_value = 0;
					break;
				default:
					HandleNumber(int.Parse(button));
					break;
			}
		}

		private void HandleNumber(int value)
		{
			if (_value == 0) _value = value;
			else if (ValueLength() < 10)
			{
				AppendNumber(value);
			}
		}

		private void AppendNumber(decimal value)
		{
			ShiftValueLeft();
			_value += value;
		}

		private void ShiftValueLeft()
		{
			_value *= 10;
		}

		private int ValueLength()
		{
			return _value.ToString().Length;
		}

		public string Display
		{
			get
			{
				return _value.ToString() + '.';
			}
		}
	}
}
