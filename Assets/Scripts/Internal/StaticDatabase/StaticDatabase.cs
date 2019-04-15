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

	private Dictionary<string, T> _allData = new Dictionary<string, T>();

	public StaticDatabase(T[] allData, Properties databaseProperties)
	{
		Properties = databaseProperties;
		Version = Properties.GetProp(PROPERTY_KEY_VERSION).GetValue(0UL);

		for(int i = 0; i < allData.Length; i++)
		{
			T data = allData[i];

			if(_allData.ContainsKey(data.DataID))
			{
				UnityEngine.Debug.LogWarningFormat("StaticDatabase `{0}` Already containing data with ID `{1}`. Data skipped.", GetType().Name, data.DataID);
				continue;
			}

			_allData.Add(data.DataID, data);
		}
	}

	public Dictionary<string, T> GetAllDataCopy()
	{
		return new Dictionary<string, T>(_allData);
	}

	public T[] GetData(Predicate<T> predicate)
	{
		List<T> allData = new List<T>();
		foreach(var pair in _allData)
		{
			if(predicate(pair.Value))
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
		_allData.TryGetValue(dataID, out T data);
		return data;
	}

	public bool GetFirstData(Predicate<T> predicate, out T data)
	{
		foreach(var pair in _allData)
		{
			if(predicate(pair.Value))
			{
				data = pair.Value;
				return true;
			}
		}

		data = default(T);
		return false;
	}

	public bool GetFirstData(string dataID, out T data)
	{
		return _allData.TryGetValue(dataID, out data);
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