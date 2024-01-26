namespace CalculatorProgram;
public static class CalculatorMenuExtensions
{
	public static void GenerateSampleCalculations(this CalculatorMenu calcMenu, int count, int max = 10)
	{
		for (int i = 0; i < count; i++)
		{
			double num1 = CalculatorExtensions.Random.Next(1, max);
			double num2 = CalculatorExtensions.Random.Next(1, max);
			int index = CalculatorExtensions.Random.Next(calcMenu.IntToDoOperationMap.Count);
			calcMenu.IntToDoOperationMap[index](num1, num2);
		}
	}
}
