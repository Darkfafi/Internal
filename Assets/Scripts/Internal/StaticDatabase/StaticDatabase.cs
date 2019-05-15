using System;
using System.Collections.Generic;

public class StaticDatabase<T> where T : struct, IStaticDatabaseData
{
	public const string PROPERTY_KEY_VERSION = "version";

	public Properties Properties
	{
		get; private set;
	}

	public ulong Version
	{
		get; private set;
	}

	private Dictionary<string, T> _prototypeData = new Dictionary<string, T>();
	private Dictionary<string, Properties> _allData = new Dictionary<string, Properties>();

	public StaticDatabase(Dictionary<string, Properties> allData, Properties databaseProperties)
	{
		Properties = databaseProperties;
		Version = Properties.GetProp(PROPERTY_KEY_VERSION).GetValue(0UL);

		_allData = allData;

		foreach(var pair in _allData)
		{
			_prototypeData.Add(pair.Key, CreateData(pair.Key));
		}
	}

	public T[] GetData(Predicate<T> predicate)
	{
		List<T> allData = new List<T>();
		foreach(var pair in _prototypeData)
		{
			if(predicate == null || predicate(pair.Value))
			{
				allData.Add(pair.Value);
			}
		}
		return allData.ToArray();
	}

	public T GetFirstData(Predicate<T> predicate)
	{
		GetFirstData(predicate, out T data);
		return data;
	}

	public T GetFirstData(string dataID)
	{
		GetFirstData(dataID, out T data);
		return data;
	}

	public bool GetFirstData(Predicate<T> predicate, out T data)
	{
		foreach(var pair in _prototypeData)
		{
			if(predicate(pair.Value))
			{
				data = CreateData(pair.Key);
				return true;
			}
		}

		data = default(T);
		return false;
	}

	public bool GetFirstData(string dataID, out T data)
	{
		if(_allData.ContainsKey(dataID))
		{
			data = CreateData(dataID);
			return true;
		}

		data = default(T);
		return false;
	}

	private T CreateData(string id)
	{
		T data = default;
		data.SetProperties(id, _allData[id].Clone());
		return data;
	}
}

public interface IStaticDatabaseData
{
	string DataID
	{
		get;
	}

	void SetProperties(string dataID, Properties properties);
}