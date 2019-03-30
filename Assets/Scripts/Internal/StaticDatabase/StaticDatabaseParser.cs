using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDatabaseParser
{
	public static StaticDatabase<T> ParseDatabase<T>(string databaseTextFile) where T : struct, IStaticDatabaseData
	{
		// Format: dataID->obj.obj.obj = value;
		// Concept: id(properties)->key = value
		// Concept: id(properties)->props.props.key = value

		List<T> databaseEntries = new List<T>();
		Dictionary<string, Properties> entryToPropertiesMap = new Dictionary<string, Properties>();

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
				Debug.LogWarning($"Skipped line {i} for path to value is not in format x->a.b.c = v");
				continue;
			}

			string[] fullPath = pathAndValue[0].Trim().Split('.');
			Array.Reverse(fullPath);
			Stack<string> path = new Stack<string>(fullPath);
			string value = pathAndValue[1].Trim();

			if(!entryToPropertiesMap.TryGetValue(key, out Properties properties))
			{
				properties = new Properties();
				entryToPropertiesMap[key] = properties;
			}

			// id->path.path.path (get to top part / create layers to top)
			while(path.Count > 1)
			{
				string propertiesHolderKey = path.Pop();
				if(!properties.TryGetProps(propertiesHolderKey, out Properties props))
				{
					props = new Properties();
					properties.AddProperties(propertiesHolderKey, props);
					properties = props;
				}
			}

			//id->path.path.path = value (insert into top part)
			Property property = new Property(path.Pop(), value);
			properties.AddProperty(property);
		}

		foreach(var dataPair in entryToPropertiesMap)
		{
			T entry = default;
			entry.SetProperties(dataPair.Key, dataPair.Value);
			databaseEntries.Add(entry);
		}

		return new StaticDatabase<T>(databaseEntries.ToArray());
	}
}
