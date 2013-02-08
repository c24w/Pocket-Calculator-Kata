using System;
using System.Collections.Generic;
using System.Linq;
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
		private decimal _displayValue;
		private decimal _storedValue;
		private Command _storedCommand;

		readonly Dictionary<string, Command> _mapCommand = new Dictionary<string, Command>
		{
			{"AC", Command.ClearAll},
			{"/", Command.Divide},
			{"*", Command.Multiply},
			{"+", Command.Add},
			{"-", Command.Subtract},
			{"=", Command.Equals}
		};

		enum Command
		{
			None,
			ClearAll,
			Divide,
			Multiply,
			Add,
			Subtract,
			Equals
		}

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleButton(button);
		}

		private void HandleButton(string button)
		{
			int value;
			var isNumber = int.TryParse(button, out value);

			if (isNumber) HandleNumber(value);
			else HandleCommand(_mapCommand[button]);
		}

		private void HandleCommand(Command command)
		{
			if (command == Command.ClearAll)
			{
				ResetDisplay();
			}
			else if (command == Command.Equals)
			{
				switch (_storedCommand)
				{
					case Command.None:
						ResetDisplay();
						break;
					case Command.Add:
						_displayValue += _storedValue;
						break;
				}
				ClearStoredCommand();
				ClearStoredValue();
			}
			else
			{
				_storedCommand = command;
				StoreValue();
			}
		}

		private void StoreValue()
		{
			_storedValue = _displayValue;
			ResetDisplay();
		}

		private void ClearStoredValue() { _storedValue = 0; }
		private void ClearStoredCommand() { _storedCommand = Command.None; }

		private void HandleNumber(int value)
		{
			if (DisplayValueLength() < 10)
				AppendNumber(value);
		}

		private void ResetDisplay()
		{
			_displayValue = 0;
		}

		private void AppendNumber(decimal value)
		{
			if (_displayValue != 0)
				ShiftValue();
			_displayValue += value;
		}

		private void ShiftValue()
		{
			_displayValue *= 10;
		}

		private int DisplayValueLength()
		{
			return ((int)Math.Log10((double)_displayValue)) + 1;
		}

		public string Display
		{
			get
			{
				return _displayValue.ToString() + '.';
			}
		}
	}
}
