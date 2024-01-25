namespace CalculatorLibrary;
public abstract class Operation
{
	public double Num1 { get; set; }
	public double Num2 { get; set; }
	public double Result { get => Solve(); }
	protected abstract double Solve();
	public T UseAs<T>(double num, bool asNum1 = true) where T : Operation, new()
	=> asNum1 switch
	{
		true => new T() { Num1 = Result, Num2 = num },
		_ => new T() { Num1 = num, Num2 = Result }
	};
	
	public static TResult UseAs<TSource, TResult>(TSource operation, double num, bool asNum1 = true) 
		where TSource : Operation, new()
		where TResult : Operation, new()
	=> asNum1 switch
	{
		true => new TResult() { Num1 = operation.Result, Num2 = num },
		_ => new TResult() { Num1 = num, Num2 = operation.Result }
	};

	#region static methods for basic math functions
	public static double Sum(double a, double b) => a + b;
	public static double Difference(double a, double b) => a - b;
	public static double Product(double a, double b) => a * b;
	public static double Quotient(double a, double b) => a / b;
	public static double Mod(double a, double b) => a % b;
	public static double Pow(double num, double exponent) => Math.Pow(num, exponent);
	public static double Sqrt(double num) => Math.Sqrt(num);
	public static double Sin(double num) => Math.Sin(num);
	public static double Cos(double num) => Math.Cos(num);
	public static double Tan(double num) => Math.Tan(num);
	#endregion
}