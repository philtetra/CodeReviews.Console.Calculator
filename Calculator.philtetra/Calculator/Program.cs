namespace CalculatorProgram;
class Program
{
	private static void Main(string[] args)
	{
		//OperationHelper.Num1Color = ConsoleColor.Cyan;
		//OperationHelper.Num2Color = ConsoleColor.Magenta;

		var menu = new CalculatorMenu();
		// menu.GenerateSampleCalculations(100);
		bool endApp = false;
		while (!endApp)
		{
			Console.Clear();
			PrintHeader(false);
			endApp = menu.View();
		}
	}

	static void PrintHeader(bool displayConsoleInfo)
	{
		const string appName = "CONSOLE CALCULATOR";
		const string author = "by philtetra";
		string consoleInfo = $"WW:{Console.WindowWidth}, WH:{Console.WindowHeight}, BW:{Console.BufferWidth}, BH:{Console.BufferHeight}";
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine(appName);
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.WriteLine(author.PadLeft(appName.Length));
		Console.ForegroundColor = ConsoleColor.Blue;
		if (displayConsoleInfo)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(consoleInfo);
			Console.ForegroundColor = ConsoleColor.Blue;
		}
		Console.WriteLine(new string('=', displayConsoleInfo ? consoleInfo.Length : appName.Length));
		Console.ForegroundColor = ConsoleColor.Gray;
	}
}