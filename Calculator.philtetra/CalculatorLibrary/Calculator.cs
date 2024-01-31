using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CalculatorLibrary;
public partial class Calculator
{
	public int UseCount { get; private set; }
	// private readonly JsonWriter writer;
	private const string binPath = ".\\bin\\Debug\\net8.0";
	private const string userInfoFile = $"{binPath}\\user_info.json";
	private const string calculationsFile = $"{binPath}\\calculations_log.json";
	private const string calculationsTypesFile = $"{binPath}\\calculations_types.json";
	public List<Operation> Calculations { get; set; }
	private List<int> calculationsTypesEncoded;
	public Calculator()
	{
		Calculations = new List<Operation>();
		// StreamWriter logFile = File.CreateText("calculations_log.json");
		// Trace.AutoFlush = true;
		// writer = new JsonTextWriter(logFile);
		// writer.Formatting = Formatting.Indented;
		// writer.WriteStartObject();
		// writer.WritePropertyName("Operations");
		// writer.WriteStartArray();
	}
	public double DoOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		// SaveOperation(operation); // TO FIX: currently throwing an exception
		this.Calculations.Add(operation);
		IncrementUseCount();

		return operation.Result;
	}

	public T GetOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		//SaveOperation(operation); // TO FIX: currently throwing an exception
		this.Calculations.Add(operation);
		this.calculationsTypesEncoded.Add(TypeToIntMap[operation.GetType()]);
		IncrementUseCount();

		return operation;
	}

	// private void SaveOperation(Operation op)
	// {
	// 	writer.WriteStartObject();
	// 	writer.WritePropertyName("Operand1");
	// 	writer.WriteValue(op.Num1);
	// 	writer.WritePropertyName("Operand2");
	// 	writer.WriteValue(op.Num2);
	// 	writer.WritePropertyName("Operation");
	// 	writer.WriteValue(op.GetType());
	// 	writer.WritePropertyName("Result");
	// 	writer.WriteValue(op.Result);
	// 	writer.WriteEndObject();
	// }

	// public void Finish()
	// {
	// 	writer.WriteEndArray();
	// 	writer.WriteEndObject();
	// 	writer.Close();
	// }

	private void IncrementUseCount() => this.UseCount++;

	public void LoadUserInfo()
	{
		if (!File.Exists(userInfoFile)) return;
		UserInfoAction((_, uses) => this.UseCount = uses);
	}

	public void SaveUserInfo()
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
		string file = calculationsFile;
		string typesFile = calculationsTypesFile;

		if (!File.Exists(file))
		{
			Console.WriteLine($"'{file}' file is not present.");
			return;
		}

		if (!File.Exists(calculationsTypesFile))
		{
			Console.WriteLine($"Types cannot be determined as "
			+ "'{calculationsTypesFile}' file is missing or is corrupted.");
			return;
		}

		try
		{
			JArray typeArr = JArray.Parse(File.ReadAllText(typesFile));
			List<int>? types = JsonConvert.DeserializeObject<List<int>>(typeArr.ToString());
			for (int i = 0; i < types?.Count; i++)
			{
				// JObject calculation = JObject.Parse()
			}

			JArray jsonArr = JArray.Parse(File.ReadAllText(file));
			List<Operation>? calculations = JsonConvert.DeserializeObject<List<Operation>>(jsonArr.ToString());

			if (calculations is not null)
			{
				this.Calculations = calculations;
			}
		}
		catch (JsonReaderException ex)
		{
			Console.WriteLine($"Parsing file '{file}' failed:\n{ex.Message}\nStack Trace:\n{ex.StackTrace}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\nInner - {ex.InnerException}: {ex.InnerException?.Message}");
		}
	}

	public void SaveCalculations()
	{
		string file = calculationsFile;
		string typesFile = calculationsTypesFile;

		try
		{
			string json = JsonConvert.SerializeObject(this.Calculations, Formatting.Indented);
			string types = JsonConvert.SerializeObject(this.calculationsTypesEncoded, Formatting.Indented);
			File.WriteAllText(file, json);
			File.WriteAllText(typesFile, types);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{ex.Message}\nInner - {ex.InnerException}: {ex.InnerException?.Message}");
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
