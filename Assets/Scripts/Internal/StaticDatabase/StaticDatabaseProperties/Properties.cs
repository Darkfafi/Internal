using System.Collections.Generic;

public class Properties
{
	private Dictionary<string, Property> _propertiesMap = new Dictionary<string, Property>();
	private Dictionary<string, Properties> _holdingProperties = new Dictionary<string, Properties>();

	public Properties Clone()
	{
		Properties newInstance = new Properties();
		newInstance._propertiesMap = new Dictionary<string, Property>(_propertiesMap);

		foreach(var pair in _holdingProperties)
		{
			newInstance._holdingProperties.Add(pair.Key, pair.Value.Clone());
		}

		return newInstance;
	}

	public void SetProperty(Property property)
	{
		_propertiesMap[property.Key] = property;
	}

	public void SetProperty(string key, string value)
	{
		SetProperty(new Property(key, value));
	}

	public void SetProperties(string key, Properties properties)
	{
		_holdingProperties[key] = properties;
	}

	public Properties GetProps(string key, bool defaultIsSelf)
	{
		if(TryGetProps(key, out Properties v))
		{
			return v;
		}

		if(defaultIsSelf)
			return this;

		return default;
	}

	public bool TryGetProps(string key, out Properties value)
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