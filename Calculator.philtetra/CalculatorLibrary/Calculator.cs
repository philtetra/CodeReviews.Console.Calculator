﻿using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CalculatorLibrary;
public partial class Calculator
{
	public int UseCount { get; private set; }
	private readonly JsonWriter writer;
	private const string userInfoFile = "user_info.json";
	private const string calculationsFile = "calculations_log.json";
	public List<Operation> Calculations { get; set; }
	public Calculator()
	{
		Calculations = new List<Operation>();
		StreamWriter logFile = File.CreateText("calculations_log.json");
		Trace.AutoFlush = true;
		writer = new JsonTextWriter(logFile);
		writer.Formatting = Formatting.Indented;
		writer.WriteStartObject();
		writer.WritePropertyName("Operations");
		writer.WriteStartArray();
	}
	public double DoOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		SaveOperation(operation); // TO FIX: currently throwing an exception
		this.Calculations.Add(operation);
		IncrementUseCount();

		return operation.Result;
	}

	public T GetOperation<T>(double num1, double num2) where T : Operation, new()
	{
		T operation = new() { Num1 = num1, Num2 = num2 };
		//SaveOperation(operation); // TO FIX: currently throwing an exception
		this.Calculations.Add(operation);
		IncrementUseCount();

		return operation;
	}

	private void SaveOperation(Operation op)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("Operand1");
		writer.WriteValue(op.Num1);
		writer.WritePropertyName("Operand2");
		writer.WriteValue(op.Num2);
		writer.WritePropertyName("Operation");
		writer.WriteValue(op.GetType());
		writer.WritePropertyName("Result");
		writer.WriteValue(op.Result);
		writer.WriteEndObject();
	}

	public void Finish()
	{
		writer.WriteEndArray();
		writer.WriteEndObject();
		writer.Close();
	}

	private void IncrementUseCount() => this.UseCount++;

	public void LoadUserInfo()
	{
		if (!File.Exists(userInfoFile)) return;
		UserInfoAction((_, uses) => this.UseCount = uses);
	}

	public void SaveUserInfo()
	{
		string file = "user_info.json";

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
				json["useCount"] = this.UseCount;
				File.WriteAllText(file, json.ToString());
			});
		}
	}

	private static void UserInfoAction(Action<JObject, int> action)
	{
		string file = userInfoFile;
		string propertyName = "useCount";

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

	private void LoadCalculations()
	{
		string file = calculationsFile;
		//string Num1Name = "Num1";
		//string Num2Name = "Num2";
		//string ResultName = "Result";
		string CalculationsName = "Calculations";

		if (!File.Exists(file))
		{
			Console.WriteLine($"'{file}' file is not present.");
			return;
		}

		try
		{
			JObject json = JObject.Parse(File.ReadAllText(file));

			if (json.ContainsKey(CalculationsName))
			{
				List<Operation>? calculations = JsonConvert.DeserializeObject<List<Operation>>(json.ToString());
				if (calculations is not null)
				{
					this.Calculations = calculations;
				}
			}
			else
			{
				Console.WriteLine($"'{file}' does not contain '{CalculationsName}'.");
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

	private void SaveCalculations()
	{
		string file = calculationsFile;
		string Num1Name = "Num1";
		string Num2Name = "Num2";
		string ResultName = "Result";
		string CalculationsName = "Calculations";
		
		try
		{
			string json = JsonConvert.SerializeObject(this.Calculations, Formatting.Indented);
			File.WriteAllText(file, json);
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
}
