﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pocket_Calculator
{
	public class PocketCalculator
	{
		private bool _clearDisplayOnNumberInput;
		private readonly Display _display = new Display();
		private readonly PendingCalculation _pendingCalculation = new PendingCalculation();
		private readonly Dictionary<string, Commands> _commandMap;
		private readonly Dictionary<string, Operator> _operatorMap;
		private decimal _memory;

		public string Display
		{
			get { return String.Format("{0}.", _display.Value); }
		}

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
				HandleNumberSequence(input);
				return;
			}

			if (IsOperator(input))
			{
				HandleOperator(MapToOperator(input));
				return;
			}

			if (IsCommand(input))
			{
				HandleCommand(MapToCommand(input));
			}

			else throw new ArgumentException("Could not parse input: " + input);
		}

		private void ClearDisplayIfRequired()
		{
			if (_clearDisplayOnNumberInput)
			{
				ClearDisplay();
				_clearDisplayOnNumberInput = false;
			}
		}

		private bool IsOperator(string input)
		{
			return _operatorMap.ContainsKey(input);
		}

		private Operator MapToOperator(string input)
		{
			return _operatorMap[input];
		}

		private bool IsCommand(string input)
		{
			return _commandMap.ContainsKey(input);
		}

		private Commands MapToCommand(string input)
		{
			return _commandMap[input];
		}

		private static bool IsNumberSequence(string input)
		{
			return Regex.IsMatch(input, "^[0-9]$");
		}

		private void HandleNumberSequence(string value)
		{
			ClearDisplayIfRequired();
			if (CanDisplayMoreDigits())
				AppendNumberSequence(value);
		}

		private void HandleOperator(Operator op)
		{
			switch (op)
			{
				case Operator.Equal:
					DoEquals();
					break;
				default:
					if (DoingCalculation())
						DisplayRunningTotal();
					HandleCalculation(op);
					break;
			}
		}

		private void DoEquals()
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
			_pendingCalculation.Value = _display.Value;
			_pendingCalculation.Operator = op;
			ClearDisplayOnNextNumberInput();
		}

		private void ClearDisplayOnNextNumberInput()
		{
			_clearDisplayOnNumberInput = true;
		}

		private void DisplayRunningTotal()
		{
			switch (_pendingCalculation.Operator)
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
			_display.Value = _pendingCalculation.Value - _display.Value;
		}

		private void DoAddition()
		{
			_display.Value += _pendingCalculation.Value;
		}

		private void DoMultiplication()
		{
			_display.Value *= _pendingCalculation.Value;
		}

		private void DoDivision()
		{
			_display.Value = _pendingCalculation.Value / _display.Value;
		}

		private void HandleCommand(Commands command)
		{
			switch (command)
			{
				case Commands.Clear:
					ClearDisplay();
					break;
				case Commands.ClearAll:
					ClearDisplay();
					ClearPendingCalculation();
					break;
				case Commands.FlipSign:
					FlipSign();
					break;
				case Commands.MemoryPlus:
					MemoryPlus();
					break;
				case Commands.MemoryMinus:
					MemoryMinus();
					break;
				case Commands.MemoryRecall:
					RecallMemory();
					break;
				case Commands.SquareRoot:
					SquareRoot();
					break;
			}
		}

		private void SquareRoot()
		{
			_display.Value = (decimal) Math.Sqrt((double) _display.Value);
		}

		private void MemoryPlus()
		{
			DoEquals();
			_memory += _display.Value;
			_clearDisplayOnNumberInput = true;
		}

		private void MemoryMinus()
		{
			DoEquals();
			_memory -= _display.Value;
			_clearDisplayOnNumberInput = true;
		}

		private void RecallMemory()
		{
			_display.Value = _memory;
		}

		private void ClearPendingCalculation()
		{
			ClearCurrentCalculationOperator();
			ClearCurrentOperator();
		}

		private void ClearCurrentCalculationOperator()
		{
			_pendingCalculation.Value = 0;
		}

		private void FlipSign()
		{
			_display.Value = 0 - _display.Value;
		}

		private bool DoingCalculation()
		{
			return _pendingCalculation.Operator != Operator.None;
		}

		private void ClearCurrentOperator()
		{
			_pendingCalculation.Operator = Operator.None;
		}

		private void ClearDisplay()
		{
			_display.Value = 0;
		}

		private void AppendNumberSequence(string numberSequence)
		{
			if (_display.Value != 0)
				ShiftValue();
			var value = int.Parse(numberSequence);
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
	}
}