using CalculatorLibrary;

namespace CalculatorProgram;
public class CalculatorMenu
{
	public Func<bool> View { get; private set; }
	public Calculator Calculator;
	public readonly Dictionary<int, Func<double, double, double>> IntToDoOperationMap;
	public readonly Dictionary<int, Func<double, double, Operation>> IntToGetOperationMap;
	private readonly HistoryContext historyCtx;

	public CalculatorMenu()
	{
		this.View = ViewMainMenu;
		this.Calculator = new();
		this.IntToDoOperationMap = new()
		{
			[0] = Calculator.DoOperation<Addition>,
			[1] = Calculator.DoOperation<Subtraction>,
			[2] = Calculator.DoOperation<Multiplication>,
			[3] = Calculator.DoOperation<Division>,
			[4] = Calculator.DoOperation<Modulus>,
			[5] = Calculator.DoOperation<Power>,
			[6] = Calculator.DoOperation<SquareRoot>,
			[7] = Calculator.DoOperation<Sinus>,
			[8] = Calculator.DoOperation<Cosinus>,
			[9] = Calculator.DoOperation<Tangent>
		};
		this.IntToGetOperationMap = new()
		{
			[0] = Calculator.GetOperation<Addition>,
			[1] = Calculator.GetOperation<Subtraction>,
			[2] = Calculator.GetOperation<Multiplication>,
			[3] = Calculator.GetOperation<Division>,
			[4] = Calculator.GetOperation<Modulus>,
			[5] = Calculator.GetOperation<Power>,
			[6] = Calculator.GetOperation<SquareRoot>,
			[7] = Calculator.GetOperation<Sinus>,
			[8] = Calculator.GetOperation<Cosinus>,
			[9] = Calculator.GetOperation<Tangent>
		};
		this.historyCtx = new();
	}

	private void SaveData() => this.Calculator.SaveCalculations();

	public bool ViewMainMenu()
	{
		this.historyCtx.InUse = false;
		Console.CursorVisible = false;
		printMenuOptions();
		ConsoleKeyInfo keyInfo = Console.ReadKey(true);
		switch (keyInfo.Key)
		{
			case ConsoleKey.D0:
				SaveData();
				return true;
			case ConsoleKey.D1:
				this.View = ViewOperations;
				break;
			case ConsoleKey.D2:
				this.View = ViewHistory;
				break;
		}
		Console.CursorVisible = true;
		return false;

		void printMenuOptions()
		{
			var menuOptions = Enum.GetNames(typeof(MenuOption));
			// 1 Operations
			// 2 History
			for (int i = 1; i < menuOptions.Length; i++)
			{
				if (menuOptions[i] == MenuOption.History.ToString())
				{
					string historyInfo = this.Calculator.Calculations.Count == 0 ? "Empty" : $"{this.Calculator.Calculations.Count}";
					Console.WriteLine($"{i} {menuOptions[i]} ({historyInfo})");
				}
				else
				{
					Console.WriteLine($"{i} {menuOptions[i]}");
				}
			}
			Console.WriteLine($"\n0 {menuOptions[0]}");
		}
	}

	public class HistoryContext
	{
		public bool InUse = false;
		public bool UseNum1;
		public int CurrIdx = 0;
		public int CurrPage = 0;
		public Operation? Operation;
	}

	public bool ViewHistory()
	{
		bool emptyHistory = this.Calculator.Calculations.Count == 0;
		Console.CursorVisible = false;

		if (emptyHistory)
		{
			Console.WriteLine("Looks empty here");
			Console.WriteLine("\nPress any key to return to the menu");
			Console.ReadKey();
			this.View = ViewMainMenu;
			return false;
		}
		else
		{
			Console.SetCursorPosition(0, 3);
			int calcsCount = this.Calculator.Calculations.Count;
			int initTop = Console.CursorTop;
			int initLeft = Console.CursorLeft;
			int row = initTop;
			int marginBottom = 5;
			int columns = 3;
			int recordsPerColumn = Console.BufferHeight - marginBottom;
			int recordsPerPage = recordsPerColumn * columns;
			int columnWidth = Console.BufferWidth / columns;
			int indents = 0;
			const int recordIndent = -5;

			int firstIdx = this.historyCtx.CurrPage * recordsPerPage;
			int max = firstIdx + recordsPerPage;

			if (firstIdx > calcsCount - 1) firstIdx = calcsCount - (calcsCount - recordsPerPage);
			if (max > calcsCount) max = calcsCount;

			if (historyCtx.InUse)
			{
				Console.SetCursorPosition(columnWidth / 2, 1);
				string controlsHint = $"( 'Esc' to cancel, 'Enter' to confirm, Arrow keys to navigate )";
				Console.Write(controlsHint);
				Console.SetCursorPosition(initLeft, initTop);
			}

			for (int i = firstIdx; i < max; i++)
			{
				if (i > firstIdx + recordsPerPage + 1)
				{
					break;
				}

				if (i >= firstIdx + (indents + 1) * recordsPerColumn)
				{
					indents++;
					row = initTop;
				}
				Console.SetCursorPosition(indents * columnWidth, row++);

				if (this.historyCtx.InUse && i == this.historyCtx.CurrIdx)
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.ForegroundColor = ConsoleColor.Black;
					this.historyCtx.Operation = this.Calculator.Calculations[i];
				}

				Console.WriteLine($"|{$"{$"{i + 1}",-3}| ",recordIndent}{this.Calculator.Calculations[i]}");

				if (this.historyCtx.InUse)
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Gray;
				}

			}
			Console.CursorTop = recordsPerColumn + initTop;
			int totalPages = calcsCount / recordsPerPage + (calcsCount % recordsPerPage == 0 ? 0 : 1);
			string pageInfo = $"Page [{historyCtx.CurrPage + 1}/{totalPages}]\tShowing {max - firstIdx} records out of {calcsCount} total";
			int center = Console.BufferWidth / 2 - pageInfo.Length / 2;
			if (Console.CursorLeft + center < Console.BufferWidth)
			{
				Console.CursorLeft = center;
			}
			Console.WriteLine(pageInfo);

			if (!historyCtx.InUse)
			{
				string bottomText1 = "0 Back to menu\t\t1 Clear history\t\t2 Enter navigation & selection mode";
				if (Console.CursorLeft + center / 2 < Console.BufferWidth)
				{
					Console.CursorLeft = center / 2;
				}
				Console.Write(bottomText1);
			}
			else
			{
				string bottomText2 = $"Current index [{historyCtx.CurrIdx + 1}] => ";
				Console.Write(bottomText2);
				historyCtx.Operation!.PrintInColor();
			}

			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			if (this.historyCtx.InUse)
			{
				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						if (historyCtx.CurrIdx > 0)
						{
							historyCtx.CurrIdx--;
						}
						break;
					case ConsoleKey.DownArrow:
						if (historyCtx.CurrIdx < calcsCount - 1)
						{
							historyCtx.CurrIdx++;
						}
						break;
					case ConsoleKey.RightArrow:
						if (historyCtx.CurrIdx + recordsPerColumn <= calcsCount - 1)
						{
							historyCtx.CurrIdx += recordsPerColumn;
						}
						break;
					case ConsoleKey.LeftArrow:
						if (historyCtx.CurrIdx - recordsPerColumn >= 0)
						{
							historyCtx.CurrIdx -= recordsPerColumn;
						}
						break;
					case ConsoleKey.Enter:
						ClearLine();
						Console.Write("Use result as first or second number? ( '");
						Console.ForegroundColor = OperationHelper.Num1Color;
						Console.Write("1");
						Console.ForegroundColor = ConsoleColor.Gray;
						Console.Write("' / '");
						Console.ForegroundColor = OperationHelper.Num2Color;
						Console.Write("2");
						Console.ForegroundColor = ConsoleColor.Gray;
						Console.Write("' ) => ");
						historyCtx.Operation!.PrintInColor();
						ConsoleKeyInfo numberChoice;
						do
						{
							numberChoice = Console.ReadKey(true);
							switch (numberChoice.Key)
							{
								case ConsoleKey.D1:
									historyCtx.UseNum1 = true;
									break;
								case ConsoleKey.D2:
									historyCtx.UseNum1 = false;
									break;
							}
						} while (numberChoice.Key != ConsoleKey.D1 && numberChoice.Key != ConsoleKey.D2);
						this.View = ViewOperations;
						break;
					case ConsoleKey.Escape:
						historyCtx.InUse = false;
						break;
				}

				if (historyCtx.CurrIdx > (historyCtx.CurrPage + 1) * recordsPerPage - 1)
				{
					historyCtx.CurrPage++;
				}
				else if (historyCtx.CurrPage > 0 && historyCtx.CurrIdx < historyCtx.CurrPage * recordsPerPage)
				{
					historyCtx.CurrPage--;
				}

			}
			else
			{
				switch (keyInfo.Key)
				{
					case ConsoleKey.D0:
						this.View = ViewMainMenu;
						break;
					case ConsoleKey.D1:
						this.Calculator.Calculations.Clear();
						break;
					case ConsoleKey.D2:
						this.historyCtx.InUse = true;
						break;
				}
			}
		}
		Console.CursorVisible = true;
		return false;
	}

	public bool ViewOperations()
	{
		Console.WriteLine("Choose an operation from the following list:");
		string selectionInfo1 = "| Press 'SPACE' to switch between columns |";
		Console.WriteLine(new string('-', selectionInfo1.Length));
		Console.WriteLine(selectionInfo1);
		string selectionInfo2 = "| Operation is selected with keys D1 - D9 |";
		string selectionInfo3 = "| in its respective column".PadRight(selectionInfo2.Length - 2) + " |";
		Console.WriteLine(selectionInfo2);
		Console.WriteLine(selectionInfo3);
		Console.WriteLine(new string('-', selectionInfo1.Length));

		int columnWidth = 24;
		int columnHeight = 9;
		int initTop = Console.CursorTop;
		int top = initTop;
		string[]? operationNames = Enum.GetNames(typeof(OperationType));
		for (int i = 0; i < operationNames.Length; i++)
		{
			if (i > columnHeight - 1 && i < columnHeight * 2 - 1)
			{
				Console.CursorLeft = columnWidth;
				Console.CursorTop = top++;
				Console.WriteLine($"{i + 1 - columnHeight} {operationNames[i]}");
			}
			else
			{
				Console.WriteLine($"{i + 1} {operationNames[i]}");
			}
		}
		Console.SetCursorPosition(0, columnHeight + initTop);
		Console.WriteLine("\n0 Back to menu\n");
		int topAfterBackToMenu = Console.CursorTop;

		if (historyCtx.InUse)
		{
			Console.Write("\nUsing result ");
			Console.ForegroundColor = OperationHelper.GetPromptColor(historyCtx.UseNum1);
			Console.Write($"{historyCtx.Operation!.Result:0.####}");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine($" as {(historyCtx.UseNum1 ? "the first number" : "the second number")}");
			Console.WriteLine("( For Sqrt & Trig functions it is always used as the first number )");
		}


		Console.CursorVisible = false;
		int parsedChoice;
		int columnNumber = 1;
		const int columnCount = 2;
		do
		{
			Console.CursorTop = initTop + columnHeight;
			ClearLine();
			Console.CursorLeft = columnWidth * (columnNumber - 1);
			Console.WriteLine(new string('=', columnWidth));
			Console.CursorTop = topAfterBackToMenu;
			ClearLine();
			Console.Write($"Your option? ");
			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			if (keyInfo.Key == ConsoleKey.Spacebar && columnNumber != columnCount)
			{
				columnNumber++;
			}
			else if (keyInfo.Key == ConsoleKey.Spacebar)
			{
				columnNumber--;
			}
			parsedChoice = OperationHelper.GetDPadKeysAsIntChoice(keyInfo.Key, columnNumber);
		} while (parsedChoice == -1);
		ClearLine();
		Console.Write($"Your option => {(OperationType)parsedChoice}");

		if (parsedChoice == 0)
		{
			this.View = ViewMainMenu;
			return false;
		}
		Console.WriteLine();
		Console.CursorVisible = true;

		// Sqrt & Trig functions require only one number
		// ( options 7+ )
		bool IsNum2Needed = parsedChoice < 7;
		double num1, num2 = 0;
		string opOperator = OperationHelper.TypeToOperatorStringMap[OperationHelper.IntToTypeMap[parsedChoice - 1]];
		if (!historyCtx.InUse)
		{
			num1 = GetUserInput(stayOnLine: true);
			Console.Write($"{num1} {opOperator} ");
			if (IsNum2Needed)
			{
				num2 = GetUserInput(stayOnLine: true);
				Console.Write(num2);
			}
			Console.Write(" = ");
		}
		else
		{
			ClearLine();
			historyCtx.PrintPrompt(parsedChoice);
			Console.WriteLine();

			OperationHelper.OperatorType opType = OperationHelper.IntToOperatorTypeMap[parsedChoice];
			if (opType == OperationHelper.OperatorType.SquareRoot || opType == OperationHelper.OperatorType.Trigonometry)
			{
				num1 = historyCtx.Operation!.Result;
			}
			else
			{
				ClearLine();
				if (historyCtx.UseNum1)
				{
					num1 = historyCtx.Operation!.Result;
					num2 = GetUserInput("Type second number: ", stayOnLine: true);
				}
				else
				{
					num1 = GetUserInput("Type first number: ", stayOnLine: true);
					num2 = historyCtx.Operation!.Result;
				}
			}
		}

		Operation? operation = null;
		try
		{
			//double result = this.IntToDoOperationMap[parsedChoice - 1](num1, num2);
			//ClearLine();
			//this.Calculator.Calculations[^1].PrintInColor();
			operation = this.IntToGetOperationMap[parsedChoice - 1](num1, num2);
			ClearLine();
			operation.PrintInColor();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occured:\n{ex.Message}");
		}

		int lineLength = operation is null ? columnWidth : operation.ToString().Length;
		Console.WriteLine('\n' + new string('-', lineLength) + '\n');
		Console.CursorVisible = false;
		Console.ReadKey(true);
		Console.CursorVisible = true;

		return false;
	}

	public static void ClearLine()
	{
		Console.CursorLeft = 0;
		Console.Write(new string(' ', Console.BufferWidth));
		Console.CursorLeft = 0;
	}

	public static double GetUserInput(string promptMessage = "", string correctionMessage = "", bool stayOnLine = false)
	{
		Console.Write(promptMessage);
		int left = Console.CursorLeft;
		string? numInput = Console.ReadLine();
		double cleanNum;
		while (!double.TryParse(numInput, out cleanNum))
		{
			if (numInput is not null)
			{
				Console.SetCursorPosition(left, Console.CursorTop - 1);
				Console.Write(new string(' ', numInput.Length));
				Console.CursorLeft -= numInput.Length;
			}
			else
			{
				Console.SetCursorPosition(0, Console.CursorTop - 1);
				Console.Write(new string(' ', Console.BufferWidth));
				Console.CursorLeft = 0;
			}
			Console.Write(correctionMessage);
			numInput = Console.ReadLine();
		}
		if (stayOnLine) Console.CursorTop--;
		return cleanNum;
	}

	public enum MenuOption
	{
		Quit = 0,
		Operations,
		History
	}
}