public interface ILocalizationSystem
{
	string Localize(int number);
	string Localize(string key);
	string Localize(string languageKey, int number);
	string Localize(string languageKey, string key);
}
