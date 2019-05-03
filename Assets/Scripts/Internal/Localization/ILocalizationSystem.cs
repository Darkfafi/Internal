public interface ILocalizationSystem
{
	LocalizedString Localize(int number);
	LocalizedString Localize(long number);
	LocalizedString Localize(float number);
	LocalizedString Localize(double number);
	LocalizedString Localize(string key);
	LocalizedString LocalizeFormat(string key, params LocalizedString[] formatParameters);
	LocalizedString LocalizeFormatKeys(string key, params object[] keys);
	LocalizedString LanguageLocalize(string languageKey, int number);
	LocalizedString LanguageLocalize(string languageKey, long number);
	LocalizedString LanguageLocalize(string languageKey, float number);
	LocalizedString LanguageLocalize(string languageKey, double number);
	LocalizedString LanguageLocalize(string languageKey, string key);
	LocalizedString LanguageLocalizeFormat(string languageKey, string key, params LocalizedString[] formatParameters);
	LocalizedString LanguageLocalizeFormatKeys(string languageKey, string key, params object[] keys);
}
