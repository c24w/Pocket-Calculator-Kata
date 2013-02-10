using System;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _currentResult;
		private Operator _currentOperator;
		private bool _shouldClearDisplayOnNextNumberInput;
		private readonly Display _display = new Display();

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

			var operatorMap = InputTypeMaps.OperatorMap;
			if (operatorMap.ContainsKey(button))
			{
				HandleOperator(operatorMap[button]);
			}

			else
			{
				var commandMap = InputTypeMaps.CommandMap;
				if (commandMap.ContainsKey(button))
					HandleCommand(commandMap[button]);
			}
		}

		private void HandleNumber(int value)
		{
			if (CanDisplayMoreDigits())
				AppendNumber(value);
		}

		private void HandleOperator(Operator op)
		{
			switch (op)
			{
				case Operator.Equal:
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
			_currentResult = _display.Value;
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
				case Operator.Divide:
					DoDivision();
					break;
				case Operator.Multiply:
					DoMultiplication();
					break;
				case Operator.Add:
					DoAddition();
					break;
				case Operator.Subtract:
					DoSubtraction();
					break;
			}
		}

		private void DoSubtraction()
		{
			_display.Value = _currentResult - _display.Value;
		}
		private void DoAddition()
		{
			_display.Value += _currentResult;
		}
		private void DoMultiplication()
		{
			_display.Value *= _currentResult;
		}
		private void DoDivision()
		{
			_display.Value = _currentResult / _display.Value;
		}

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
			_display.Value = 0 - _display.Value;
		}

		private bool DoingCalculation()
		{
			return _currentOperator != Operator.None;
		}

		private void ClearCurrentOperator()
		{
			_currentOperator = Operator.None;
		}

		private void ClearDisplay()
		{
			_display.Value = 0;
		}

		private void AppendNumber(decimal value)
		{
			if (_display.Value != 0)
				ShiftValue();
			_display.Value += value;
		}

		private void ShiftValue()
		{
			_display.Value *= 10;
		}

		private bool CanDisplayMoreDigits()
		{
			var displayedDigits = ((int)Math.Log10((double)_display.Value)) + 1;
			return displayedDigits < 10;
		}

		public string Display
		{
			get
			{
				return String.Format("{0}.", _display.Value);
			}
		}
	}
}