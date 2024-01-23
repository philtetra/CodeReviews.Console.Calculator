namespace CalculatorLibrary;
public abstract partial class Operation
{
	public double Num1 { get; set; }
	public double Num2 { get; set; }
	public double Result { get => Solve(); }
	protected abstract double Solve();

	#region static methods
	public static double Sum(double a, double b) => a + b;
	public static double Difference(double a, double b) => a - b;
	public static double Product(double a, double b) => a * b;
	public static double Quotient(double a, double b) => a / b;
	public static double Mod(double a, double b) => a % b;
	public static double Pow(double num, double exponent) => Math.Pow(num, exponent);
	public static double Sqrt(double num, double _) => Math.Sqrt(num);
	public static double Sin(double num, double _) => Math.Sin(num);
	public static double Cos(double num, double _) => Math.Cos(num);
	public static double Tan(double num, double _) => Math.Tan(num);
	#endregion
}