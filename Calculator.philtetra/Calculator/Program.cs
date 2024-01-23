using CalculatorLibrary;
using System.Text.RegularExpressions;

namespace CalculatorProgram;
class Program
{
	static void Main(string[] args)
	{
		bool endApp = false;

		Calculator calculator = new();
		calculator.LoadUserInfo();
		while (!endApp)
		{
			Console.Clear();
			// Display title as the C# console calculator app.
			Console.WriteLine("Console Calculator in C#\r");
			Console.WriteLine("------------------------\n");

			// Declare variables and set to empty.
			string? numInput1;
			string? numInput2;
			double result;

			// Ask the user to type the first number.
			Console.Write("Type a number, and then press Enter: ");
			numInput1 = Console.ReadLine();

			double cleanNum1;
			while (!double.TryParse(numInput1, out cleanNum1))
			{
				Console.Write("This is not valid input. Please enter an integer value: ");
				numInput1 = Console.ReadLine();
			}

			// Ask the user to type the second number.
			Console.Write("Type another number, and then press Enter: ");
			numInput2 = Console.ReadLine();

			double cleanNum2;
			while (!double.TryParse(numInput2, out cleanNum2))
			{
				Console.Write("This is not valid input. Please enter an integer value: ");
				numInput2 = Console.ReadLine();
			}

			// Ask the user to choose an operator.
			Console.WriteLine("Choose an operator from the following list:");

			foreach(KeyValuePair<int, Type> kvp in OperationHelper.OperationsTypeDict)
			{
                Console.WriteLine($"{kvp.Key + 1}. {OperationHelper.OperationTypeMap[kvp.Value]}");
            }
			Console.Write("Your option? ");

			string? option;
			do
			{
				option = Console.ReadLine();
			} while (!int.TryParse(option, out int parsedOption)
					|| (parsedOption < 0 && parsedOption > OperationHelper.OperationsTypeDict.Count));

			try
			{
				result = calculator.DoOperation<Addition>(cleanNum1, cleanNum2);
				Console.WriteLine("Your result: {0:0.##}\n", result);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occured:\n{ex.Message}");
			}
			Console.WriteLine("------------------------\n");

			// Wait for the user to respond before closing.
			Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
			if (Console.ReadLine() == "n") endApp = true;

			Console.WriteLine("\n"); // Friendly linespacing.
		}
		// Add call to close the JSON writer before return
		calculator.Finish();
		calculator.SaveUserInfo();
		return;
	}
}

public static class OperationHelper
{
	public readonly static Dictionary<int, Type> OperationsTypeDict = new()
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

	public readonly static Dictionary<Type, OperationType> OperationTypeMap = new()
	{
		[typeof(Addition)] = OperationType.Addition,
		[typeof(Subtraction)] = OperationType.Subtraction,
		[typeof(Multiplication)] = OperationType.Multiplication,
		[typeof(Division)] = OperationType.Division,
		[typeof(Modulus)] = OperationType.Modulus,
		[typeof(Power)] = OperationType.Power,
		[typeof(SquareRoot)] = OperationType.SquareRoot,
		[typeof(Sinus)] = OperationType.Sinus,
		[typeof(Cosinus)] = OperationType.Cosinus,
		[typeof(Tangent)] = OperationType.Tangent
	};
}

