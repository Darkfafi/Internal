using System.Collections.Generic;

public struct Property
{
	public string Key;
	public string Value;

	private delegate bool TryParseValueHandler<T>(string input, out T value);

	public Property(string key, string value)
	{
		Key = key;
		Value = value;
	}

	public static string ArrayToPropertyString(string[] array)
	{
		string returnValue = "";
		for(int i = 0; i < array.Length; i++)
		{
			if(i != 0)
			{
				returnValue += ',';
			}

			returnValue += array[i];
		}
		return returnValue;
	}

	#region KeyValuePair Array Methods

	public KeyValuePair<string, string>[] GetValue(KeyValuePair<string, string>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, string>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, string>[] value)
	{
		return TryGetParsedPairValues(TryParseString, out value);
	}

	public KeyValuePair<string, long>[] GetValue(KeyValuePair<string, long>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, long>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, long>[] value)
	{
		return TryGetParsedPairValues(long.TryParse, out value);
	}

	public KeyValuePair<string, int>[] GetValue(KeyValuePair<string, int>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, int>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, int>[] value)
	{
		return TryGetParsedPairValues(int.TryParse, out value);
	}

	public KeyValuePair<string, double>[] GetValue(KeyValuePair<string, double>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, double>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, double>[] value)
	{
		return TryGetParsedPairValues(double.TryParse, out value);
	}

	public KeyValuePair<string, float>[] GetValue(KeyValuePair<string, float>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, float>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, float>[] value)
	{
		return TryGetParsedPairValues(float.TryParse, out value);
	}

	public KeyValuePair<string, bool>[] GetValue(KeyValuePair<string, bool>[] defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, bool>[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, bool>[] value)
	{
		return TryGetParsedPairValues(bool.TryParse, out value);
	}

	#endregion

	#region KeyValuePair Methods

	public KeyValuePair<string, string> GetValue(KeyValuePair<string, string> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, string> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, string> value)
	{
		return TryGetParsedPairValue(TryParseString, out value);
	}

	public KeyValuePair<string, long> GetValue(KeyValuePair<string, long> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, long> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, long> value)
	{
		return TryGetParsedPairValue(long.TryParse, out value);
	}

	public KeyValuePair<string, int> GetValue(KeyValuePair<string, int> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, int> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, int> value)
	{
		return TryGetParsedPairValue(int.TryParse, out value);
	}

	public KeyValuePair<string, double> GetValue(KeyValuePair<string, double> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, double> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, double> value)
	{
		return TryGetParsedPairValue(double.TryParse, out value);
	}

	public KeyValuePair<string, float> GetValue(KeyValuePair<string, float> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, float> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, float> value)
	{
		return TryGetParsedPairValue(float.TryParse, out value);
	}

	public KeyValuePair<string, bool> GetValue(KeyValuePair<string, bool> defaultValue)
	{
		if(TryGetValue(out KeyValuePair<string, bool> v))
		{
			return v;
		}
		return defaultValue;
	}

	public bool TryGetValue(out KeyValuePair<string, bool> value)
	{
		return TryGetParsedPairValue(bool.TryParse, out value);
	}

	#endregion

	#region Array Methods

	public string[] GetValue(string[] defaultValue)
	{
		if(TryGetValue(out string[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out string[] value)
	{
		return TryGetSplitInputValue(Value, ',', out value);
	}

	public long[] GetValue(long[] defaultValue)
	{
		if(TryGetValue(out long[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out long[] value)
	{
		return TryGetSplitInputValue(long.TryParse, Value, ',', out value);
	}

	public int[] GetValue(int[] defaultValue)
	{
		if(TryGetValue(out int[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out int[] value)
	{
		return TryGetSplitInputValue(int.TryParse, Value, ',', out value);
	}

	public double[] GetValue(double[] defaultValue)
	{
		if(TryGetValue(out double[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out double[] value)
	{
		return TryGetSplitInputValue(double.TryParse, Value, ',', out value);
	}

	public float[] GetValue(float[] defaultValue)
	{
		if(TryGetValue(out float[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out float[] value)
	{
		return TryGetSplitInputValue(float.TryParse, Value, ',', out value);
	}

	public bool[] GetValue(bool[] defaultValue)
	{
		if(TryGetValue(out bool[] v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out bool[] value)
	{
		return TryGetSplitInputValue(bool.TryParse, Value, ',', out value);
	}

	#endregion

	#region Single Value Methods

	public float GetValue(float defaultValue)
	{
		if(TryGetValue(out float v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out float value)
	{
		if(TryGetValue(out string v))
		{
			return float.TryParse(v, out value);
		}

		value = 0f;
		return false;
	}

	public uint GetValue(uint defaultValue)
	{
		if(TryGetValue(out uint v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out uint value)
	{
		if(TryGetValue(out string v))
		{
			return uint.TryParse(v, out value);
		}

		value = 0u;
		return false;
	}

	public int GetValue(int defaultValue)
	{
		if(TryGetValue(out int v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out int value)
	{
		if(TryGetValue(out string v))
		{
			return int.TryParse(v, out value);
		}

		value = 0;
		return false;
	}

	public bool GetValue(bool defaultValue)
	{
		if(TryGetValue(out bool v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out bool value)
	{
		if(TryGetValue(out string v))
		{
			return bool.TryParse(v, out value);
		}

		value = false;
		return false;
	}

	public long GetValue(long defaultValue)
	{
		if(TryGetValue(out long v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out long value)
	{
		if(TryGetValue(out string v))
		{
			return long.TryParse(v, out value);
		}

		value = 0L;
		return false;
	}

	public ulong GetValue(ulong defaultValue)
	{
		if(TryGetValue(out ulong v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out ulong value)
	{
		if(TryGetValue(out string v))
		{
			return ulong.TryParse(v, out value);
		}

		value = 0UL;
		return false;
	}

	public string GetValue(string defaultValue)
	{
		if(TryGetValue(out string v))
		{
			return v;
		}

		return defaultValue;
	}

	public bool TryGetValue(out string value)
	{
		value = Value;
		return !string.IsNullOrEmpty(value);
	}

	#endregion

	#region Util Methods

	private bool TryGetParsedPairValue<T>(TryParseValueHandler<T> tryParseMethod, out KeyValuePair<string, T> value)
	{
		if(TryGetValue(out string[] v))
		{
			if(v.Length == 2)
			{
				if(tryParseMethod(v[1], out T parsedValue))
				{
					value = new KeyValuePair<string, T>(v[0], parsedValue);
					return true;
				}
			}
		}
		value = new KeyValuePair<string, T>();
		return false;
	}

	private bool TryGetParsedPairValues<T>(TryParseValueHandler<T> tryParseMethod, out KeyValuePair<string, T>[] value)
	{
		if(TryGetValue(out string plainText))
		{
			plainText = plainText.Replace(';', ',');
			if(TryGetSplitInputValue(plainText, ',', out string[] v))
			{
				if(v.Length % 2 == 0)
				{
					value = new KeyValuePair<string, T>[v.Length / 2];
					for(int i = 0; i < value.Length; i++)
					{
						int index = i * 2;
						if(tryParseMethod(v[index + 1], out T parsedValue))
						{
							value[i] = new KeyValuePair<string, T>(v[index], parsedValue);
						}
						else
						{
							value = new KeyValuePair<string, T>[] { };
							return false;
						}
					}
					return true;
				}
			}
		}
		value = new KeyValuePair<string, T>[] { };
		return false;
	}

	private bool TryGetSplitInputValue(string input, char splitValue, out string[] value)
	{
		return TryGetSplitInputValue(TryParseString, input, splitValue, out value);
	}

	private bool TryGetSplitInputValue<T>(TryParseValueHandler<T> tryParseMethod, string input, char splitValue, out T[] value)
	{
		input = input.Trim();
		if(!string.IsNullOrEmpty(input))
		{
			if(input[input.Length - 1].Equals(splitValue))
			{
				input = input.Remove(input.Length - 1, 1);
			}

			string[] arrayEntries = input.Split(splitValue);
			value = new T[arrayEntries.Length];
			for(int i = 0, c = value.Length; i < c; i++)
			{
				if(tryParseMethod(arrayEntries[i].Trim(), out T parsedValue))
				{
					value[i] = parsedValue;
				}
				else
				{
					value = new T[] { };
					return false;
				}
			}
			return true;
		}

		value = new T[] { };
		return false;
	}

	private bool TryParseString(string input, out string v)
	{
		v = input;
		return true;
	}

	#endregion
}