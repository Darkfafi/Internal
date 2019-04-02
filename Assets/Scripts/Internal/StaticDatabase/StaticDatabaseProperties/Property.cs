using System.Collections.Generic;

public struct Property
{
	public string Key;
	public string Value;

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
		if(TryGetValue(out string[] v))
		{
			if(v.Length == 2)
			{
				value = new KeyValuePair<string, string>(v[0], v[1]);
				return true;
			}
		}

		value = new KeyValuePair<string, string>();
		return false;
	}

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
		if(TryGetValue(out string v))
		{
			string[] arrayEntries = v.Trim().Split(',');
			value = new string[arrayEntries.Length];
			for(int i = 0, c = value.Length; i < c; i++)
			{
				value[i] = arrayEntries[i].Trim();
			}
			return true;
		}

		value = new string[] { };
		return false;
	}

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
}