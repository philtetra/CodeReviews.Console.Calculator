namespace CalculatorLibrary;
public class Addition : Operation
{
	protected override double Solve() => Sum(Num1, Num2);
}

public class Subtraction : Operation
{
	protected override double Solve() => Difference(Num1, Num2);
}

public class Multiplication : Operation
{
	protected override double Solve() => Product(Num1, Num2);
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
}

public class Power : Operation
{
	protected override double Solve() => Pow(Num1, Num2);
}

public class SquareRoot : Operation
{
	protected override double Solve() => Sqrt(Num1, Num2);
}

public class Sinus : Operation
{
	protected override double Solve() => Sin(Num1, Num2);
}

public class Cosinus : Operation
{
	protected override double Solve() => Cos(Num1, Num2);
}

public class Tangent : Operation
{
	protected override double Solve() => Tan(Num1, Num2);
}