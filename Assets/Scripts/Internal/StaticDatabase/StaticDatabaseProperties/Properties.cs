public class Properties : IProperties
{
	public PropertiesData PropertiesData
	{
		get; private set;
	}

	public Properties()
	{
		PropertiesData = new PropertiesData(this);
	}

	public void SetProperty(string key, string value)
	{
		PropertiesData.SetProperty(key, value);
	}

	public void SetProperty(Property property)
	{
		PropertiesData.SetProperty(property);
	}

	public void SetProperties(string key, PropertiesData properties)
	{
		PropertiesData.SetProperties(key, properties);
	}

	public PropertiesData GetProps(string key, bool defaultIsSelf)
	{
		return PropertiesData.GetProps(key, defaultIsSelf);
	}

	public bool TryGetProps(string key, out PropertiesData value)
	{
		return PropertiesData.TryGetProps(key, out value);
	}

	public Property GetProp(string key)
	{
		return PropertiesData.GetProp(key);
	}

	public string[] GetAllPropertyKeys()
	{
		return PropertiesData.GetAllPropertyKeys();
	}

	public bool TryGetProp(string key, out Property value)
	{
		return PropertiesData.TryGetProp(key, out value);
	}
}