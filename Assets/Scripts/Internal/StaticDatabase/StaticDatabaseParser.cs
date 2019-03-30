using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDatabaseParser
{
	public static StaticDatabase<T> ParseDatabase<T>(string databaseTextFile) where T : struct, IStaticDatabaseData
	{
		// Format: dataID->obj.obj.obj = value;
		// Concept: KEY/PATH = VALUE

		List<T> databaseEntries = new List<T>();
		Dictionary<string, List<Property>> entryToPropertiesMap = new Dictionary<string, List<Property>>();

		string[] lines = databaseTextFile.Split(new [] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

		for(int i = 0, c = lines.Length; i < c; i++)
		{
			string line = lines[i];

			if(string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
				continue;

			string[] keyToPathAndValue = line.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

			if(keyToPathAndValue.Length != 2)
			{
				Debug.LogWarning($"Skipped line {i} for key to data is not in format x->a.b.c = v");
				continue;
			}

			string key = keyToPathAndValue[0].Trim();
			string[] pathAndValue = keyToPathAndValue[1].Trim().Split('=');

			if(pathAndValue.Length != 2)
			{
				Debug.LogWarning($"Skipped line {i} for path to value is in format x->a.b.c = v");
				continue;
			}

			string path = pathAndValue[0].Trim();
			string value = pathAndValue[1].Trim();

			Property property = new Property(path, value);

			if(!entryToPropertiesMap.TryGetValue(key, out List<Property> properties))
			{
				properties = new List<Property>();
				entryToPropertiesMap[key] = properties;
			}

			properties.Add(property);
		}

		foreach(var dataPair in entryToPropertiesMap)
		{
			T entry = default;
			entry.SetProperties(dataPair.Key, new Properties(dataPair.Value.ToArray()));
			databaseEntries.Add(entry);
		}

		return new StaticDatabase<T>(databaseEntries.ToArray());
	}
}
