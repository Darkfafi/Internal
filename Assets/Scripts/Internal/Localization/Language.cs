using System;
using System.Collections.Generic;
using System.Globalization;

public class Language
{
	public string LanguageID
	{
		get; private set;
	}

	public CultureInfo CultureInfo
	{
		get; private set;
	}

	public LanguageData LanguageData
	{
		get; private set;
	}

	private Dictionary<string, string> _translations = new Dictionary<string, string>();

	public Language(string languageID, LanguageData languageData)
	{
		LanguageID = languageID;
		LanguageData = languageData;
		CultureInfo = new CultureInfo(LanguageData.CultureCode);

		for(int i = 0; i < languageData.Localizations.Length; i++)
		{
			LanguageData.Localization localizationData = languageData.Localizations[i];
			_translations[localizationData.Key] = localizationData.Translation;
		}
	}

	public bool HasTranslation(string key)
	{
		return _translations.ContainsKey(key);
	}

	public bool TryGetTranslation(string key, out string translation)
	{
		return _translations.TryGetValue(key, out translation);
	}
}

[Serializable]
public struct LanguageData
{
	public string CultureCode;
	public Localization[] Localizations;

	[Serializable]
	public struct Localization
	{
		public string Key;
		public string Translation;
		public string[] Tags;
	}
}