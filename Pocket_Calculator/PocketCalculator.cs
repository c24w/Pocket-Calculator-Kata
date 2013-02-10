using System;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _displayValue;
		private decimal _currentResult;
		private Operator _currentOperator;
		private bool _shouldClearDisplayOnNextNumberInput;

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleInput(button);
		}

		private void HandleInput(string button)
		{
			int value;
			var buttonIsType = int.TryParse(button, out value);
			if (buttonIsType)
			{
				if (_shouldClearDisplayOnNextNumberInput)
				{
					ClearDisplay();
					_shouldClearDisplayOnNextNumberInput = false;
				}
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
				ClearCurrentOperator();
			}
			else ClearDisplayOnNextNumberInput();
		}

		private void HandleCalculation(Operator op)
		{
			_currentResult = _displayValue;
			_currentOperator = op;
			ClearDisplayOnNextNumberInput();
		}

		private void ClearDisplayOnNextNumberInput()
		{
			_shouldClearDisplayOnNextNumberInput = true;
		}

		private void DisplayRunningTotal()
		{
			switch (_currentOperator.Type)
			{
				case Operator.Types.Divide:
					DoDivision();
					break;
				case Operator.Types.Multiply:
					DoMultiplication();
					break;
				case Operator.Types.Add:
					DoAddition();
					break;
				case Operator.Types.Subtract:
					DoSubtraction();
					break;
			}
		}

		private void DoSubtraction() { _displayValue = _currentResult - _displayValue; }
		private void DoAddition() { _displayValue += _currentResult; }
		private void DoMultiplication() { _displayValue *= _currentResult; }
		private void DoDivision() { _displayValue = _currentResult / _displayValue; }

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

		private void ClearCurrentOperator()
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