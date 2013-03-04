using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _currentResult;
		private Operator _currentOperator;
		private bool _clearDisplayOnNextNumberInput;
		private readonly Display _display = new Display();
		private readonly Dictionary<string, Commands> _commandMap;
		private readonly Dictionary<string, Operator> _operatorMap;

		public PocketCalculator(Dictionary<string, Commands> commandMap, Dictionary<string, Operator> operatorMap)
		{
			_commandMap = commandMap;
			_operatorMap = operatorMap;
		}

		public void Process(string inputData)
		{
			var inputs = inputData.Split(' ');
			foreach (var input in inputs)
				HandleInput(input);
		}

		private void HandleInput(string input)
		{
			if (IsNumberSequence(input))
			{
				if (_clearDisplayOnNextNumberInput)
				{
					ClearDisplay();
					_clearDisplayOnNextNumberInput = false;
				}
				HandleNumber(int.Parse(input));
				return;
			}

			if (IsOperator(input))
			{
				HandleOperator(_operatorMap[input]);
				return;
			}

			if (IsCommand(input))
			{
				HandleCommand(MapCommand(input));
			}

			else throw new ArgumentException("Could not parse input: " + input);
		}

		private bool IsOperator(string input)
		{
			return _operatorMap.ContainsKey(input);
		}

		private Commands MapCommand(string input)
		{
			return _commandMap[input];
		}

		private bool IsCommand(string input)
		{
			return _commandMap.ContainsKey(input);
		}

		private static bool IsNumberSequence(string input)
		{
			return Regex.IsMatch(input, "^[0-9]$");
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
			_clearDisplayOnNextNumberInput = true;
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