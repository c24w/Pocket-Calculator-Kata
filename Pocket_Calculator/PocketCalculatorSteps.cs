using System;
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

	public class PocketCalculator
	{
		private decimal _displayValue;
		private decimal _storedValue;
		private Operator _storedOperator;
		private Command _storedCommand;

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleButton(button);
		}

		private void HandleButton(string button)
		{
			if (_storedCommand == Command.ClearAll)
			{
				ResetDisplay();
				ClearStoredCommand();
			}

			int value;
			var buttonIsType = int.TryParse(button, out value);
			if (buttonIsType)
			{
				HandleNumber(value);
				return;
			}

			Operator op;
			buttonIsType = Operator.TryMap(button, out op);
			if (buttonIsType)
			{
				HandleOperator(op);
				return;
			}

			Command command;
			buttonIsType = Command.TryMap(button, out command);
			if (buttonIsType) HandleCommand(command);
		}

		private void HandleNumber(int value)
		{
			if (CanDisplayMoreDigits())
				AppendNumber(value);
		}

		private void HandleOperator(Operator op)
		{
			switch (op.Type)
			{
				case Operator.Types.Equal:
					HandleEquals();
					break;
				default:
					_storedValue = _displayValue;
					_storedOperator = op;
					_storedCommand = Command.ClearAll;
					break;
			}
		}

		private void HandleCommand(Command command)
		{
			switch (command.Type)
			{
				case Command.Types.ClearAll:
					ResetDisplay();
					break;
				case Command.Types.PlusMinus:
					FlipSign();
					break;
			}
		}

		private void FlipSign()
		{
			_displayValue = 0 - _displayValue;
		}

		private void HandleEquals()
		{
			if (DoingCalculation())
			{
				switch (_storedOperator.Type)
				{
					case Operator.Types.Divide:
						_displayValue = _storedValue / _displayValue;
						break;
					case Operator.Types.Multiply:
						_displayValue *= _storedValue;
						break;
					case Operator.Types.Add:
						_displayValue += _storedValue;
						break;
					case Operator.Types.Subtract:
						_displayValue = _storedValue - _displayValue;
						break;
				}
				ClearStoredOperator();
			}
			else
				_storedCommand = Command.ClearAll;
		}

		private bool DoingCalculation()
		{
			return _storedOperator != null;
		}

		private void ClearStoredCommand()
		{
			_storedCommand = null;
		}

		private void ClearStoredOperator()
		{
			_storedOperator = null;
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

		private bool CanDisplayMoreDigits()
		{
			var displayedDigits = ((int)Math.Log10((double)_displayValue)) + 1;
			return displayedDigits < 10;
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
