﻿using System;
using System.Text.RegularExpressions;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private decimal _currentResult;
		private Operator _currentOperator;
		private bool _shouldClearDisplayOnNextNumberInput;
		private readonly Display _display = new Display();

		public void Process(string inputData)
		{
			var inputs = inputData.Split(' ');
			foreach (var input in inputs)
				HandleInput(input);
		}

		private void HandleInput(string input)
		{
			if(Regex.IsMatch(input, "^[0-9]$"))
			{
				if (_shouldClearDisplayOnNextNumberInput)
				{
					ClearDisplay();
					_shouldClearDisplayOnNextNumberInput = false;
				}
				HandleNumber(int.Parse(input));
				return;
			}

			var operatorMap = InputTypeMaps.OperatorMap;
			if (operatorMap.ContainsKey(input))
			{
				HandleOperator(operatorMap[input]);
			}

			else
			{
				var commandMap = InputTypeMaps.CommandMap;
				if (commandMap.ContainsKey(input))
					HandleCommand(commandMap[input]);
				else throw new ArgumentException("Could not parse input: " + input);
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