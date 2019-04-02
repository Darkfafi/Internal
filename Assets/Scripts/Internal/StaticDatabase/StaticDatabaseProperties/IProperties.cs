public interface IProperties
{
	void SetProperty(string key, string value);
	void SetProperty(Property property);
	void SetProperties(string key, PropertiesData properties);
	PropertiesData GetProps(string key, bool defaultIsSelf);
	bool TryGetProps(string key, out PropertiesData value);
	Property GetProp(string key);
	string[] GetAllPropertyKeys();
	bool TryGetProp(string key, out Property value);
}
