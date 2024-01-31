using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace CalculatorLibrary;
public partial class Calculator
{
	public int UseCount { get; private set; }
	private const string userInfoFile = "user_info.json";
	private const string calculationsFile = "calculations_log.json";
	private const string calculationsTypesFile = $"calculations_types.json";
	public List<Operation> Calculations { get; set; }
	private List<int> calculationsTypesEncoded;

	public readonly Dictionary<int, Func<double, double, double>> IntToDoOperationMap;
	public readonly Dictionary<int, Func<double, double, Operation>> IntToGetOperationMap;

	public Calculator()
	{
		Calculations = new List<Operation>();
		calculationsTypesEncoded = new List<int>();

		this.IntToDoOperationMap = new()
		{
			[0] = DoOperation<Addition>,
			[1] = DoOperation<Subtraction>,
			[2] = DoOperation<Multiplication>,
			[3] = DoOperation<Division>,
			[4] = DoOperation<Modulus>,
			[5] = DoOperation<Power>,
			[6] = DoOperation<SquareRoot>,
			[7] = DoOperation<Sinus>,
			[8] = DoOperation<Cosinus>,
			[9] = DoOperation<Tangent>
		};

		this.IntToGetOperationMap = new()
		{
			[0] = GetOperation<Addition>,
			[1] = GetOperation<Subtraction>,
			[2] = GetOperation<Multiplication>,
			[3] = GetOperation<Division>,
			[4] = GetOperation<Modulus>,
			[5] = GetOperation<Power>,
			[6] = GetOperation<SquareRoot>,
			[7] = GetOperation<Sinus>,
			[8] = GetOperation<Cosinus>,
			[9] = GetOperation<Tangent>
		};

		LoadCalculations();
	}
	public double DoOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		this.Calculations.Add(operation);
		this.calculationsTypesEncoded.Add(TypeToIntMap[operation.GetType()]);
		IncrementUseCount();

		return operation.Result;
	}

	public T GetOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		this.Calculations.Add(operation);
		this.calculationsTypesEncoded.Add(TypeToIntMap[operation.GetType()]);
		IncrementUseCount();

		return operation;
	}

	private void IncrementUseCount() => this.UseCount++;

	[Obsolete($"LoadUserInfo is deprecated, {nameof(this.UseCount)} now gets set while loading {nameof(this.Calculations)} list")]
	private void LoadUserInfo()
	{
		if (!File.Exists(userInfoFile)) return;
		UserInfoAction((_, uses) => this.UseCount = uses);
	}

	[Obsolete($"SaveUserInfo is deprecated, as {nameof(this.UseCount)} equals to the {nameof(this.Calculations)}.Count")]
	private void SaveUserInfo()
	{
		string file = userInfoFile;
		string propertyName = nameof(this.UseCount);

		if (!File.Exists(file))
		{
			JObject json = (JObject)JToken.FromObject(new
			{
				this.UseCount,
			});

			File.WriteAllText(file, json.ToString());
		}
		else
		{
			UserInfoAction((json, _) =>
			{
				json[propertyName] = this.UseCount;
				File.WriteAllText(file, json.ToString());
			});
		}
	}

	private static void UserInfoAction(Action<JObject, int> action)
	{
		string file = userInfoFile;
		string propertyName = nameof(UseCount);

		try
		{
			JObject json = JObject.Parse(File.ReadAllText(file));
			if (json.ContainsKey(propertyName))
			{
				if (int.TryParse(json[propertyName]?.ToString(), out int currentUses))
				{
					action(json, currentUses);
				}
				else
				{
					Console.WriteLine($"'{propertyName}' is in an invalid format.");
				}
			}
			else
			{
				Console.WriteLine($"'{file}' does not contain property '{propertyName}'.");
			}
		}
		catch (JsonReaderException ex)
		{
			Console.WriteLine($"Parsing file '{file}' failed:\n{ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\nInner - {ex.InnerException}: {ex.InnerException?.Message}");
		}
	}

	public void LoadCalculations()
	{
		if (!File.Exists(calculationsFile))

		{
			Console.WriteLine($"'{calculationsFile}' file is missing.");
			return;
		}

		if (!File.Exists(calculationsTypesFile))
		{
			Console.WriteLine("Types cannot be determined, as "
			+ $"'{calculationsTypesFile}' file is missing or is corrupted.");
			return;
		}

		try
		{
			JArray encodedTypesJArr = JArray.Parse(File.ReadAllText(calculationsTypesFile));
			int[]? encodedTypes = JsonConvert.DeserializeObject<int[]>(encodedTypesJArr.ToString());

			if (encodedTypes is null)
			{
				Console.WriteLine($"{nameof(encodedTypes)} is null");
				return;
			}

			JArray intermediatesJArr = JArray.Parse(File.ReadAllText(calculationsFile));
			List<Addition>? intermediates = JsonConvert.DeserializeObject<List<Addition>>(intermediatesJArr.ToString());

			if (intermediates is null)
			{
				Console.WriteLine($"{nameof(intermediates)} is null");
				return;
			}
			else
			{
				SetCalculations(intermediates, encodedTypes);
			}
		}
		catch (JsonReaderException ex)
		{
			Console.WriteLine($"Parsing file '{calculationsFile}' failed:\n{ex.Message}\nStack Trace:\n{ex.StackTrace}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\nInner - {ex.InnerException}: {ex.InnerException?.Message}");
		}
	}

	public void SaveCalculations()
	{
		try
		{
			string json = JsonConvert.SerializeObject(GetIntermediateCalculations<Addition>(), Formatting.Indented);
			string types = JsonConvert.SerializeObject(GetEncodedCalculationsTypes(), Formatting.Indented);
			File.WriteAllText(calculationsFile, json);
			File.WriteAllText(calculationsTypesFile, types);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\nInner - {ex.InnerException}: {ex.InnerException?.Message}");
		}
	}

	private List<T> GetIntermediateCalculations<T>() where T : Operation, new()
	{
		List<T> intermediates = new List<T>();
		foreach (Operation op in this.Calculations)
		{
			intermediates.Add(new T() { Num1 = op.Num1, Num2 = op.Num2 });
		}
		return intermediates;
	}

	private int[] GetEncodedCalculationsTypes()
	{
		int[] encodedTypes = new int[this.Calculations.Count];
		for (int i = 0; i < encodedTypes.Length; i++)
		{
			encodedTypes[i] = TypeToIntMap[this.Calculations[i].GetType()];
		}
		return encodedTypes;
	}

	private void SetCalculations<T>(IReadOnlyList<T> operations, IReadOnlyList<int> typesEncoded) where T : Operation, new()
	{
		if (operations.Count != typesEncoded.Count)
		{
			throw new ArgumentException("Length of operations and calculationsTypesEncoded array parameters must be equal.");
		}

		for (int i = 0; i < operations.Count; i++)
		{
			this.IntToDoOperationMap[typesEncoded[i]](operations[i].Num1, operations[i].Num2);
		}
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

	public static readonly Dictionary<Type, int> TypeToIntMap = new()
	{
		[typeof(Addition)] = 0,
		[typeof(Subtraction)] = 1,
		[typeof(Multiplication)] = 2,
		[typeof(Division)] = 3,
		[typeof(Modulus)] = 4,
		[typeof(Power)] = 5,
		[typeof(SquareRoot)] = 6,
		[typeof(Sinus)] = 7,
		[typeof(Cosinus)] = 8,
		[typeof(Tangent)] = 9
	};
}
