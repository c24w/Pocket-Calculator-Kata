using System;
using System.Collections.Generic;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _displayValue;
		private decimal _currentResult;
		private Operators _currentOperator;
		private bool _shouldClearDisplayOnNextNumberInput;

		public void Process(string data)
		{
			var buttons = data.Split(' ');
			foreach (var button in buttons)
				HandleInput(button);
		}

		private void HandleInput(string button) // ? add maps as dependency and have another map which looks up a Type against a delegate function taking two params for the calculation
		{
			var operatorMap = new Dictionary<string, Operators>
			{
				{"+", Operators.Add},
				{"-", Operators.Subtract},
				{"/", Operators.Divide},
				{"*", Operators.Multiply},
				{"=", Operators.Equal}
			};

			var commandMap = new Dictionary<string, Commands>
			{
				{"AC", Commands.ClearAll},
				{"+/-", Commands.FlipSign}
			};

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

			if (operatorMap.ContainsKey(button))
				HandleOperator(operatorMap[button]);

			else if (commandMap.ContainsKey(button))
				HandleCommand(commandMap[button]);
		}

		private void HandleNumber(int value)
		{
			if (CanDisplayMoreDigits())
				AppendNumber(value);
		}

		private void HandleOperator(Operators op)
		{
			switch (op)
			{
				case Operators.Equal:
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

		private void HandleCalculation(Operators op)
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
			switch (_currentOperator)
			{
				case Operators.Divide:
					DoDivision();
					break;
				case Operators.Multiply:
					DoMultiplication();
					break;
				case Operators.Add:
					DoAddition();
					break;
				case Operators.Subtract:
					DoSubtraction();
					break;
			}
		}

		private void DoSubtraction() { _displayValue = _currentResult - _displayValue; }
		private void DoAddition() { _displayValue += _currentResult; }
		private void DoMultiplication() { _displayValue *= _currentResult; }
		private void DoDivision() { _displayValue = _currentResult / _displayValue; }

		private void HandleCommand(Commands command)
		{
			switch (command)
			{
				case Commands.ClearAll:
					ClearDisplay();
					break;
				case Commands.FlipSign:
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
			return _currentOperator != Operators.None;
		}

		private void ClearCurrentOperator()
		{
			_currentOperator = Operators.None;
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