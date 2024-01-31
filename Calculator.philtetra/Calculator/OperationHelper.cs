using CalculatorLibrary;

namespace CalculatorProgram;
public static class OperationHelper
{
	public static ConsoleColor Num1Color { get; set; }
	public static ConsoleColor Num2Color { get; set; }

	static OperationHelper() 
	{
		Num1Color = ConsoleColor.Green;
		Num2Color = ConsoleColor.Red;
	}

	public static readonly Dictionary<int, OperatorType> IntToOperatorTypeMap = new()
	{
		[1] = OperatorType.SingleSymbol, // Addition
		[2] = OperatorType.SingleSymbol, // Subtraction
		[3] = OperatorType.SingleSymbol, // Multiplication
		[4] = OperatorType.SingleSymbol, // Division
		[5] = OperatorType.SingleSymbol, // Modulus
		[6] = OperatorType.Power, // Power
		[7] = OperatorType.SquareRoot, // SquareRoot
		[8] = OperatorType.Trigonometry, // Sinus
		[9] = OperatorType.Trigonometry, // Cosinus
		[10] = OperatorType.Trigonometry,// Tangent
	};

	public static int GetDPadKeysAsIntChoice(ConsoleKey key, int columnNumber) => columnNumber switch
	{
		1 => key switch
		{
			ConsoleKey.D0 => 0,
			ConsoleKey.D1 => 1,
			ConsoleKey.D2 => 2,
			ConsoleKey.D3 => 3,
			ConsoleKey.D4 => 4,
			ConsoleKey.D5 => 5,
			ConsoleKey.D6 => 6,
			ConsoleKey.D7 => 7,
			ConsoleKey.D8 => 8,
			ConsoleKey.D9 => 9,
			_ => -1
		},

		2 => key switch
		{
			ConsoleKey.D0 => 0,
			ConsoleKey.D1 => 10,
			_ => -1
		},

		_ => -1
	};


	/// <summary>
	/// Returns a new Operation inherited type depending on the provided choice:
	/// 1 - Addition, 
	/// 2 - Subtraction, 
	/// 3 - Multiplication, 
	/// 4 - Division, 
	/// 5 - Modulus, 
	/// 6 - Power, 
	/// 7 - SquareRoot, 
	/// 8 - Sinus, 
	/// 9 - Cosinus, 
	/// 10 - Tangent, 
	/// Any other number throws an exception.
	/// </summary>
	/// <param name="choice">
	/// </param>
	/// <returns>Return a new Operation with underlying type of one of the types specified in summary.</returns>
	/// <exception cref="NotSupportedException"></exception>
	public static Operation CreateOperation(int choice) => choice switch
	{
		1 => new Addition(),
		2 => new Subtraction(),
		3 => new Multiplication(),
		4 => new Division(),
		5 => new Modulus(),
		6 => new Power(),
		7 => new SquareRoot(),
		8 => new Sinus(),
		9 => new Cosinus(),
		10 => new Tangent(),
		_ => throw new NotSupportedException(),
	};

	public static void PrintPrompt(this CalculatorMenu.HistoryContext context, int choice)
	{
		if (context.Operation is null) return;

		Operation operationToPerform = CreateOperation(choice);
		var ctxOperation = context.Operation;
		OperatorType operatorType = TypeToOperatorTypeMap[operationToPerform.GetType()];
		Console.ForegroundColor = ConsoleColor.Gray;
		switch (operatorType)
		{
			case OperatorType.SingleSymbol:
				if (!context.UseNum1)
				{
					Console.ForegroundColor = Num1Color;
					Console.Write("_");
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write($" {TypeToOperatorStringMap[operationToPerform.GetType()]} ");
					Console.ForegroundColor = Num2Color;
					Console.Write($"{ctxOperation.Result:0.####}");
					PrintEqualsSign();
				}
				else
				{
					Console.ForegroundColor = Num1Color;
					Console.Write($"{ctxOperation.Result:0.####}");
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write($" {TypeToOperatorStringMap[operationToPerform.GetType()]} ");
					Console.ForegroundColor = Num2Color;
					Console.Write("_");
					PrintEqualsSign();
				}
				break;
			case OperatorType.Power:
				if (!context.UseNum1)
				{
					Console.ForegroundColor = Num1Color;
					Console.Write("_");
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write($"{TypeToOperatorStringMap[operationToPerform.GetType()]}");
					Console.ForegroundColor = Num2Color;
					Console.Write($"{ctxOperation.Result:0.####}");
					PrintEqualsSign();
				}
				else
				{
					Console.ForegroundColor = Num1Color;
					Console.Write($"{ctxOperation.Result:0.####}");
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write($"{TypeToOperatorStringMap[operationToPerform.GetType()]}");
					Console.ForegroundColor = Num2Color;
					Console.Write("_");
					PrintEqualsSign();
				}
				break;
			case OperatorType.SquareRoot:
				Console.ForegroundColor = Num1Color; // As Sqrt always uses Num1 for the calculation
				Console.Write($"{ctxOperation.Result:0.####}");
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write($"{TypeToOperatorStringMap[operationToPerform.GetType()]}");
				PrintEqualsSign();
				break;
			case OperatorType.Trigonometry:
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write($"{TypeToOperatorStringMap[operationToPerform.GetType()]}");
				Console.Write("(");
				Console.ForegroundColor = Num1Color; // As Trig functions always use Num1 for the calculation
				Console.Write($"{ctxOperation.Result:0.####}");
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(")");
				PrintEqualsSign();
				break;
		}
	}

	public static ConsoleColor GetPromptColor(bool useNum1) => useNum1 ? Num1Color : Num2Color;

	public static void PrintInColor(this Operation operation)
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		OperatorType operatorType = TypeToOperatorTypeMap[operation.GetType()];
		switch (operatorType)
		{
			case OperatorType.SingleSymbol:
				operation.PrintNum1();
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write($" {TypeToOperatorStringMap[operation.GetType()]} ");
				operation.PrintNum2();
				break;
			case OperatorType.Power:
				operation.PrintNum1();
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(TypeToOperatorStringMap[operation.GetType()]);
				operation.PrintNum2();
				break;
			case OperatorType.SquareRoot:
				operation.PrintNum1();
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(TypeToOperatorStringMap[operation.GetType()]);
				break;
			case OperatorType.Trigonometry:
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write($"{TypeToOperatorStringMap[operation.GetType()]}");
				Console.Write("(");
				operation.PrintNum1();
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(")");
				break;
		}
		operation.PrintResult();
		Console.ForegroundColor = ConsoleColor.Gray;
	}

	public static void PrintInColorV2(this Operation operation)
	{
		var strings = GetOperationAsStringArray(operation);
		var opType = TypeToOperatorTypeMap[operation.GetType()];
		var colors = GetOperationColors(opType);

		switch (opType)
		{
			case OperatorType.SingleSymbol:
			case OperatorType.Power:
				for (int i = 0; i < strings.Length; i++)
				{
					Console.ForegroundColor = colors[i];
					Console.Write(strings[i]);
				}
				break;
			case OperatorType.SquareRoot:
				for (int i = 0; i < strings.Length; i++)
				{
					if (i < colors.Length) Console.ForegroundColor = colors[i];
					Console.Write(strings[i]);
				}
				break;
			case OperatorType.Trigonometry:
				for (int i = 0; i < strings.Length; i++)
				{
					if (i < colors.Length) Console.ForegroundColor = colors[i];
					Console.Write(strings[i]);
				}
				break;
		}
	}

	// not working as intended atm, not used anyway
	public static ConsoleColor[] GetOperationColors(OperatorType operatorType)
	{
		var colors = new List<ConsoleColor>();
		switch (operatorType)
		{
			case OperatorType.SingleSymbol:
			case OperatorType.Power:
				colors.Add(Num1Color);
				colors.Add(ConsoleColor.Gray);
				colors.Add(Num2Color);
				colors.Add(ConsoleColor.Gray);
				break;
			case OperatorType.SquareRoot:
				colors.Add(Num1Color);
				colors.Add(ConsoleColor.Gray);
				break;
			case OperatorType.Trigonometry:
				colors.Add(ConsoleColor.Gray);
				colors.Add(Num1Color);
				break;
			default:
				return Array.Empty<ConsoleColor>();
		}
		return colors.ToArray();
	}

	// not working as intended atm, not used anyway
	public static string[] GetOperationAsStringArray(Operation operation)
	{
		OperatorType operatorType = TypeToOperatorTypeMap[operation.GetType()];
		var strings = new List<string>();
		switch (operatorType)
		{
			case OperatorType.SingleSymbol:
				strings.Add($"{operation.Num1}");
				strings.Add($" {TypeToOperatorStringMap[operation.GetType()]} ");
				strings.Add($"{operation.Num2}");
				break;
			case OperatorType.Power:
				strings.Add($"{operation.Num1}");
				strings.Add(TypeToOperatorStringMap[operation.GetType()]);
				strings.Add($"{operation.Num2}");
				break;
			case OperatorType.SquareRoot:
				strings.Add($"{operation.Num1}");
				strings.Add(TypeToOperatorStringMap[operation.GetType()]);
				break;
			case OperatorType.Trigonometry:
				strings.Add(TypeToOperatorStringMap[operation.GetType()]);
				strings.Add("(");
				strings.Add($"{operation.Num1}");
				strings.Add(")");
				break;
		}
		strings.Add($" = {operation.Result}");
		return strings.ToArray();
	}

	private static void PrintNum1(this Operation operation)
	{
		Console.ForegroundColor = Num1Color;
		Console.Write($"{operation.Num1:0.####}");
	}

	private static void PrintNum2(this Operation operation)
	{
		Console.ForegroundColor = Num2Color;
		Console.Write($"{operation.Num2:0.####}");
	}

	private static void PrintResult(this Operation operation)
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write($" = {operation.Result}");
	}

	private static void PrintEqualsSign()
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write(" = ");
	}

	public static readonly Dictionary<int, Type> IntToTypeMap = new()
	{
		[0] = typeof(Addition),
		[1] = typeof(Subtraction),
		[2] = typeof(Multiplication),
		[3] = typeof(Division),
		[4] = typeof(Modulus),
		[5] = typeof(Power),
		[6] = typeof(SquareRoot),
		[7] = typeof(Sinus),
		[8] = typeof(Cosinus),
		[9] = typeof(Tangent)
	};


	public static readonly Dictionary<Type, OperatorType> TypeToOperatorTypeMap = new()
	{
		[typeof(Addition)] = OperatorType.SingleSymbol,
		[typeof(Subtraction)] = OperatorType.SingleSymbol,
		[typeof(Multiplication)] = OperatorType.SingleSymbol,
		[typeof(Division)] = OperatorType.SingleSymbol,
		[typeof(Modulus)] = OperatorType.SingleSymbol,
		[typeof(Power)] = OperatorType.Power,
		[typeof(SquareRoot)] = OperatorType.SquareRoot,
		[typeof(Sinus)] = OperatorType.Trigonometry,
		[typeof(Cosinus)] = OperatorType.Trigonometry,
		[typeof(Tangent)] = OperatorType.Trigonometry,
	};

	public readonly static Dictionary<Type, string> TypeToOperatorStringMap = new()
	{
		[typeof(Addition)] = "+",
		[typeof(Subtraction)] = "-",
		[typeof(Multiplication)] = "*",
		[typeof(Division)] = "/",
		[typeof(Modulus)] = "%",
		[typeof(Power)] = "^",
		[typeof(SquareRoot)] = "^(1/2)",
		[typeof(Sinus)] = "sin",
		[typeof(Cosinus)] = "cos",
		[typeof(Tangent)] = "tan",
	};

	public enum OperatorType
	{
		SingleSymbol,
		Power,
		SquareRoot,
		Trigonometry
	}
}