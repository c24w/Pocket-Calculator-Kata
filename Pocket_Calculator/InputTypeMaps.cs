using System.Collections.Generic;

namespace Pocket_Calculator
{
	public static class InputTypeMaps
	{
		public static readonly Dictionary<string, Operator> OperatorMap = new Dictionary<string, Operator>
		{
			{"+", Operator.Add},
			{"-", Operator.Subtract},
			{"/", Operator.Divide},
			{"*", Operator.Multiply},
			{"=", Operator.Equal}
		};

		public static readonly Dictionary<string, Commands> CommandMap = new Dictionary<string, Commands>
		{
			{"AC", Commands.ClearAll},
			{"+/-", Commands.FlipSign}
		};
	}
}