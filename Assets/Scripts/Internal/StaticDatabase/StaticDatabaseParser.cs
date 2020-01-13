using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticDatabaseParser
{
	public const string DATABASE_DATA_ID = "DATABASE";

	public static StaticDatabase<T> ParseDatabaseAtResource<T>(string resourcePath) where T : struct, IStaticDatabaseData
	{
		 return ParseDatabase<T>(Resources.Load<TextAsset>(resourcePath).text);
	}

	public static StaticDatabase<T> ParseDatabase<T>(string databaseTextFile) where T : struct, IStaticDatabaseData
	{
		// Format: dataID->obj.obj.obj = value;
		// Concept: id(properties)->key = value
		// Concept: id(properties)->props.props.key = value

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

			string value = pathAndValue[1].Trim();

			if(!entryToPropertiesMap.TryGetValue(key, out Properties properties))
			{
				properties = new Properties();
				entryToPropertiesMap[key] = properties;
			}

			Properties topProperties = GetTopPropertiesData(pathAndValue[0], properties, true, out string propKey);

			//id->path.path.path = value (insert into top part)
			Property property = new Property(propKey, value);
			topProperties.SetProperty(property);
		}

		if(entryToPropertiesMap.TryGetValue(DATABASE_DATA_ID, out Properties databaseProperties))
		{
			entryToPropertiesMap.Remove(DATABASE_DATA_ID);
		}
		else
		{
			databaseProperties = new Properties();
		}

		return new StaticDatabase<T>(entryToPropertiesMap, databaseProperties);
	}

	public static Properties GetTopPropertiesData(string path, Properties baseProperties, out string propKey)
	{
		return GetTopPropertiesData(path, baseProperties, false, out propKey);
	}

	private static Properties GetTopPropertiesData(string path, Properties baseProperties, bool addIfNotAvailable, out string propKey)
	{
		string[] fullPath = path.Trim().Split('.');
		Array.Reverse(fullPath);
		Stack<string> p = new Stack<string>(fullPath);

		// id->path.path.path (get to top part / create layers to top)
		while(p.Count > 1)
		{
			string propertiesHolderKey = p.Pop();
			if(!baseProperties.TryGetProps(propertiesHolderKey, out Properties props))
			{
				if(addIfNotAvailable)
				{
					props = new Properties();
					baseProperties.SetProperties(propertiesHolderKey, props);
				}
				else
				{
					throw new Exception($"Path {path} invalid. No properties found under key {propertiesHolderKey}");
				}
			}

			baseProperties = props;
		}

		propKey = p.Pop();
		return baseProperties;
	}
}
