using CalculatorLibrary;

namespace CalculatorProgram;
public static class CalculatorExtensions
{
	public static readonly Random Random = new((int)DateTime.Now.Ticks);
	public static void GenerateSampleCalculations(this Calculator calculator, int count)
	{
		for (int i = 0; i < count; i++)
		{
			double num1 = Random.Next(10);
			double num2 = Random.Next(10);
			calculator.DoOperation<Addition>(num1, num2);
		}
	}
}
