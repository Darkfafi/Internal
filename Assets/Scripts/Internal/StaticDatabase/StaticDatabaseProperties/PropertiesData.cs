using System.Collections.Generic;

public struct PropertiesData : IProperties
{
	public Properties Parent
	{
		get; private set;
	}

	public bool IsValid
	{
		get
		{
			return Parent != null;
		}
	}

	private Dictionary<string, Property> _propertiesMap;
	private Dictionary<string, PropertiesData> _holdingProperties;

	public PropertiesData(Properties parent)
	{
		Parent = parent;
		_propertiesMap = new Dictionary<string, Property>();
		_holdingProperties = new Dictionary<string, PropertiesData>();
	}

	public void SetProperty(Property property)
	{
		_propertiesMap[property.Key] = property;
	}

	public void SetProperty(string key, string value)
	{
		SetProperty(new Property(key, value));
	}

	public void SetProperties(string key, PropertiesData propertiesData)
	{
		_holdingProperties[key] = propertiesData;
	}

	public PropertiesData GetProps(string key, bool defaultIsSelf)
	{
		if(TryGetProps(key, out PropertiesData v))
		{
			return v;
		}

		if(defaultIsSelf)
			return this;

		return default;
	}

	public bool TryGetProps(string key, out PropertiesData value)
	{
		if(string.IsNullOrEmpty(key))
		{
			value = this;
			return true;
		}

		return _holdingProperties.TryGetValue(key, out value);
	}

	public Property GetProp(string key)
	{
		if(TryGetProp(key, out Property v))
		{
			return v;
		}

		return new Property(key, null);
	}

	public string[] GetAllPropertyKeys()
	{
		string[] keys = new string[_propertiesMap.Count];
		_propertiesMap.Keys.CopyTo(keys, 0);
		return keys;
	}

	public bool TryGetProp(string key, out Property value)
	{
		return _propertiesMap.TryGetValue(key, out value);
	}
}