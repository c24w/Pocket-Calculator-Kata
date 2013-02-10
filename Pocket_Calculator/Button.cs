using System.Collections.Generic;

namespace Pocket_Calculator
{

	public enum ButtonType
	{
		None,
		ClearAll,
		Equal,
		Divide,
		Multiply,
		Add,
		Subtract
	}

	class Operator
	{
		public enum Types
		{
			Divide,
			Multiply,
			Add,
			Subtract,
			Equal
		}

		public readonly Types Type;
		public readonly static Operator Equal = new Operator(Types.Equal);
		public readonly static Operator Add = new Operator(Types.Add);

		public Operator(Types type)
		{
			Type = type;
		}

		public static bool TryMap(string symbol, out Operator result)
		{
			if (symbol.Length > 1)
			{
				result = null;
				return false;
			}

			var op = char.Parse(symbol);
			var typeMap = new Dictionary<char, Types>
			{
				{'+', Types.Add},
				{'-', Types.Subtract},
				{'/', Types.Divide},
				{'*', Types.Multiply},
				{'=', Types.Equal}
			};
			result = new Operator(typeMap[op]);
			return typeMap.ContainsKey(op);
		}
	}

	class Command
	{
		public enum Types
		{
			ClearAll,
			PlusMinus
		}

		public readonly Types Type;

		public readonly static Command ClearAll = new Command(Types.ClearAll);

		public Command(Types type)
		{
			Type = type;
		}

		public static bool TryMap(string command, out Command result)
		{
			var typeMap = new Dictionary<string, Types>
			{
				{"AC", Types.ClearAll},
				{"+/-", Types.PlusMinus}
			};
			result = new Command(typeMap[command]);
			return typeMap.ContainsKey(command);
		}
	}

}