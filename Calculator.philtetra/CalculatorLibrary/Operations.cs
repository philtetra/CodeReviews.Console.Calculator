namespace CalculatorLibrary;
public class Addition : Operation
{
	protected override double Solve() => Sum(Num1, Num2);
	public override string ToString() => $"{Num1} + {Num2} = {Result:0.####}";
}

public class Subtraction : Operation
{
	protected override double Solve() => Difference(Num1, Num2);
	public override string ToString() => $"{Num1} - {Num2} = {Result:0.####}";
}

public class Multiplication : Operation
{
	protected override double Solve() => Product(Num1, Num2);
	public override string ToString() => $"{Num1} * {Num2} = {Result:0.####}";
}

public class Division : Operation
{
	protected override double Solve()
	{
		if (Num2 == 0)
		{
			throw new ArgumentException($"Attempted to divide by zero", nameof(Num2));
		}
		else
		{
			return Quotient(Num1, Num2);
		}
	}
	public override string ToString() => $"{Num1} / {Num2} = {Result:0.####}";
}

public class Modulus : Operation
{
	protected override double Solve()
	{
		if (Num2 == 0)
		{
			throw new ArgumentException($"Attempted to divide by zero", nameof(Num2));
		}
		else
		{
			return Mod(Num1, Num2);
		}
	}
	public override string ToString() => $"{Num1} % {Num2} = {Result:0.####}";
}

public class Power : Operation
{
	protected override double Solve() => Pow(Num1, Num2);
	public override string ToString() => $"{Num1} ^ {Num2} = {Result:0.####}";
}

public class SquareRoot : Operation
{
	protected override double Solve() => Sqrt(Num1);
	public override string ToString() => $"{Num1} ^ (1/2) = {Result:0.####}";
}

public class Sinus : Operation
{
	protected override double Solve() => Sin(Num1);
	public override string ToString() => $"sin( {Num1} ) = {Result:0.####}";
}

public class Cosinus : Operation
{
	protected override double Solve() => Cos(Num1);
	public override string ToString() => $"cos( {Num1} ) = {Result:0.####}";
}

public class Tangent : Operation
{
	protected override double Solve() => Tan(Num1);
	public override string ToString() => $"tan( {Num1} ) = {Result:0.####}";
}