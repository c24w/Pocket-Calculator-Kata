using System;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _displayValue;
		private decimal _currentResult;
		private Operator _currentOperator;
		private bool _clearDisplayOnNextInput;

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleButton(button);
		}

		private void HandleButton(string button)
		{
			if (_clearDisplayOnNextInput)
			{
				ClearDisplay();
				_clearDisplayOnNextInput = false;
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
					if (DoingCalculation())
						DisplayRunningTotal();
					HandleCalculation(op);
					break;
			}
		}

		private void HandleEquals()
		{
			if (DoingCalculation())
			{
				DisplayRunningTotal();
				ClearStoredOperator();
			}
			else
				_clearDisplayOnNextInput = true;
		}

		private void HandleCalculation(Operator op)
		{
			_currentResult = _displayValue;
			_currentOperator = op;
			_clearDisplayOnNextInput = true;
		}

		private void DisplayRunningTotal()
		{
			switch (_currentOperator.Type)
			{
				case Operator.Types.Divide:
					_displayValue = _currentResult / _displayValue;
					break;
				case Operator.Types.Multiply:
					_displayValue *= _currentResult;
					break;
				case Operator.Types.Add:
					_displayValue += _currentResult;
					break;
				case Operator.Types.Subtract:
					_displayValue = _currentResult - _displayValue;
					break;
			}
		}

		private void HandleCommand(Command command)
		{
			switch (command.Type)
			{
				case Command.Types.ClearAll:
					ClearDisplay();
					break;
				case Command.Types.FlipSign:
					FlipSign();
					break;
			}
		}

		private void FlipSign()
		{
			_displayValue = 0 - _displayValue;
		}

		private bool DoingCalculation()
		{
			return _currentOperator != null;
		}

		private void ClearStoredOperator()
		{
			_currentOperator = null;
		}

		private void ClearDisplay()
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
				return String.Format("{0}.", _displayValue);
			}
		}
	}
}