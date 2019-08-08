using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LocalizationSystem : ILocalizationSystem
{
	public const string LOCALIZATION_DATABASE_RESOURCE = "localizationSettingsDatabase";
	public const string DEFAULT_SETTINGS_ID = "settings";

	public event Action<string> LanguageChangedEvent;

	public string LanguageID
	{
		get
		{
			return _languageID;
		}
		set
		{
			if(_languageID == value)
			{
				return;
			}

			if(_settingsToIdToLanguageMap.TryGetValue(CurrentSettingsID, out Dictionary<string, Language> languageMap))
			{
				if(!languageMap.ContainsKey(value))
				{
					Debug.LogError($"No Language found with ID; {value}, set LanguageID aborted!");
					return;
				}
			}
			else
			{
				Debug.LogError($"{CurrentSettingsID} is not found in the {LOCALIZATION_DATABASE_RESOURCE} file");
				return;
			}

			_languageID = value;
			LanguageChangedEvent?.Invoke(_languageID);
		}
	}

	public string CurrentSettingsID
	{
		get
		{
			return _currentSettingsData.DataID;
		}
	}

	public string DefaultLanguageID
	{
		get
		{
			return _currentSettingsData.DefaultLanguageID;
		}
	}

	private string _languageID;

	private Dictionary<string, List<Language>> _settingsTolanguages = new Dictionary<string, List<Language>>();
	private Dictionary<string, List<string>> _settingsToLanguageIds = new Dictionary<string, List<string>>();
	private Dictionary<string, Dictionary<string, Language>> _settingsToIdToLanguageMap = new Dictionary<string, Dictionary<string, Language>>();
	private StaticDatabase<SettingsData> _settingsDatabase;
	private SettingsData _currentSettingsData;

	public static StaticDatabase<SettingsData> ParseSettingsDatabase(string localizationDatabaseResourcePath = LOCALIZATION_DATABASE_RESOURCE)
	{
		TextAsset settings = Resources.Load<TextAsset>(localizationDatabaseResourcePath);
		if(settings != null)
		{
			return StaticDatabaseParser.ParseDatabase<SettingsData>(settings.text);
		}
		else
		{
			Dictionary<string, Properties> defaultDatabase = new Dictionary<string, Properties>();
			defaultDatabase.Add(DEFAULT_SETTINGS_ID, new Properties());
			return new StaticDatabase<SettingsData>(defaultDatabase, new Properties());
		}
	}

	public LocalizationSystem(string settingsID = DEFAULT_SETTINGS_ID)
	{
		_settingsDatabase = ParseSettingsDatabase();
		SetSettings(settingsID);
	}

	public LocalizationSystem(string settingsID, out string errorMessage)
	{
		_settingsDatabase = ParseSettingsDatabase();
		SetSettings(settingsID, out errorMessage);
	}

	public void SwitchToDefaultLanguage()
	{
		LanguageID = _currentSettingsData.DefaultLanguageID;
	}

	public void SetSettings(string settingsID)
	{
		SetSettings(settingsID, out string errorMessage);
		if(!string.IsNullOrEmpty(errorMessage))
		{
			Debug.LogError(errorMessage);
		}
	}

	public void SetSettings(string settingsID, out string errorMessage)
	{
		if(!_settingsToIdToLanguageMap.ContainsKey(settingsID))
		{
			Initialize(settingsID, out errorMessage);
			if(!string.IsNullOrEmpty(errorMessage))
			{
				return;
			}
		}
		errorMessage = string.Empty;
		_currentSettingsData = _settingsDatabase.GetFirstData(settingsID);
		LanguageID = _currentSettingsData.DefaultLanguageID;
	}

	public void SetLanguage(CultureInfo culture)
	{
		Language l = GetLanguage(culture);
		if(l != null)
		{
			LanguageID = l.LanguageID;
		}
		else
		{
			Debug.LogError($"No language found with culture of name {culture.EnglishName}");
		}
	}

	public string[] GetAllLanguageIDs()
	{
		if(_settingsToLanguageIds.TryGetValue(CurrentSettingsID, out List<string> languageIDs))
		{
			return languageIDs.ToArray();
		}
		return new string[] { };
	}

	public Language[] GetAllLanguages()
	{
		if(_settingsTolanguages.TryGetValue(CurrentSettingsID, out List<Language> languages))
		{
			return languages.ToArray();
		}
		return new Language[] { };
	}

	public LocalizedString Localize(int number)
	{
		return LanguageLocalize(LanguageID, number);
	}

	public LocalizedString Localize(long number)
	{
		return LanguageLocalize(LanguageID, number);
	}

	public LocalizedString Localize(double number)
	{
		return LanguageLocalize(LanguageID, number);
	}

	public LocalizedString Localize(float number)
	{
		return LanguageLocalize(LanguageID, number);
	}

	public LocalizedString Localize(string key)
	{
		return LanguageLocalize(LanguageID, key);
	}

	public LocalizedString LocalizeFormat(string key, params LocalizedString[] formatParameters)
	{
		return LanguageLocalizeFormat(LanguageID, key, formatParameters);
	}

	public LocalizedString LocalizeFormatKeys(string key, params object[] keys)
	{
		return LanguageLocalizeFormatKeys(LanguageID, key, keys);
	}

	public LocalizedString LanguageLocalize(string languageKey, double number)
	{
		return new LocalizedString(languageKey, number.ToString(), number.ToString("N1", GetLanguage(languageKey).CultureInfo.NumberFormat));
	}

	public LocalizedString LanguageLocalize(string languageKey, long number)
	{
		return new LocalizedString(languageKey, number.ToString(), number.ToString("N0", GetLanguage(languageKey).CultureInfo.NumberFormat));
	}

	public LocalizedString LanguageLocalize(string languageKey, int number)
	{
		return LanguageLocalize(languageKey, (long)number);
	}

	public LocalizedString LanguageLocalize(string languageKey, float number)
	{
		return LanguageLocalize(languageKey, (double)number);
	}

	public LocalizedString LanguageLocalize(string languageKey, string key)
	{
		return LanguageLocalizeFormat(languageKey, key, new LocalizedString[] { });
	}

	public LocalizedString LanguageLocalizeFormat(string languageKey, string key, params LocalizedString[] formatParameters)
	{
		LocalizedString? specialLocalizedString = TrySpecialLocalization(languageKey, key, formatParameters);
		if(specialLocalizedString.HasValue)
		{
			return specialLocalizedString.Value;
		}
		else
		{
			Language language = GetLanguage(languageKey);
			string translation = string.Empty;
			if(language != null)
			{
				if(!language.TryGetTranslation(key, out translation))
				{
					translation = "k> " + key;
				}
			}
			else
			{
				translation = "l- " + (string.IsNullOrEmpty(languageKey) ? "No Language Specified" : languageKey);
			}

			return new LocalizedString(languageKey, key, translation, formatParameters);
		}
	}

	public LocalizedString LanguageLocalizeFormatKeys(string languageKey, string key, params object[] keys)
	{
		LocalizedString[] ls = new LocalizedString[keys.Length];
		for(int i = 0; i < ls.Length; i++)
		{
			ls[i] = LanguageLocalize(languageKey, keys[i].ToString());
		}
		return LanguageLocalizeFormat(languageKey, key, ls);
	}

	public Language GetLanguage(string languageID)
	{
		if(_settingsToIdToLanguageMap.TryGetValue(CurrentSettingsID, out Dictionary<string, Language> languageMap))
		{
			if(languageMap.TryGetValue(languageID, out Language l))
			{
				return l;
			}
		}
		return null;
	}

	public Language GetLanguage(CultureInfo culture)
	{
		return GetLanguage(l => l.CultureInfo == culture);
	}

	public Language GetLanguage(Func<Language, bool> condition)
	{
		Language[] languages = GetAllLanguages();
		for(int i = 0, c = languages.Length; i < c; i++)
		{
			if(condition(languages[i]))
			{
				return languages[i];
			}
		}

		return null;
	}

	private LocalizedString? TrySpecialLocalization(string languageKey, string key, params LocalizedString[] formatParameters)
	{
		if(long.TryParse(key, out long vl))
		{
			return LanguageLocalize(languageKey, vl);
		}

		if(int.TryParse(key, out int vi))
		{
			return LanguageLocalize(languageKey, vi);
		}

		if(double.TryParse(key, out double vd))
		{
			return LanguageLocalize(languageKey, vd);
		}

		if(float.TryParse(key, out float vf))
		{
			return LanguageLocalize(languageKey, vf);
		}

		return null;
	}

	private void Initialize(string settingsID, out string errorMessage)
	{
		if(!_settingsDatabase.GetFirstData(settingsID, out SettingsData settingsData))
		{
			errorMessage = $"{settingsID} is not found in the {LOCALIZATION_DATABASE_RESOURCE} file";
			return;
		}

		TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>(settingsData.LanguagesResourceLocation);

		if(jsonFiles.Length == 0)
		{
			errorMessage = $"No text files found at Resource Location; {settingsData.LanguagesResourceLocation}";
			return;
		}

		_settingsTolanguages[settingsID] = new List<Language>();
		_settingsToLanguageIds[settingsID] = new List<string>();
		_settingsToIdToLanguageMap[settingsID] = new Dictionary<string, Language>();

		for(int i = 0, c = jsonFiles.Length; i < c; i++)
		{
			TextAsset asset = jsonFiles[i];
			LanguageData data = JsonUtility.FromJson<LanguageData>(asset.text);

			if(string.IsNullOrEmpty(data.CultureCode))
			{
				errorMessage = $"No culture found in langauge asset with id {asset.name}";
				return;
			}

			if(data.Localizations.Length == 0)
			{
				errorMessage = $"No localizations found in langauge asset with id {asset.name}";
				return;
			}

			try
			{
				Language l = new Language(asset.name, data);
				_settingsTolanguages[settingsID].Add(l);
				_settingsToLanguageIds[settingsID].Add(l.LanguageID);
				_settingsToIdToLanguageMap[settingsID].Add(l.LanguageID, l);
			}
			catch(Exception ex)
			{
				errorMessage = $"Language {asset.name} could not be created: {ex.Message}";
				return;
			}
		}

		errorMessage = string.Empty;
		SwitchToDefaultLanguage();
	}

	public struct SettingsData : IStaticDatabaseData
	{
		// Properties
		public const string PROP_DEFAULT_LANGUAGE_ID = "default_language_id";
		public const string PROP_LANGUAGES_RESOURCE_PATH = "languages_resource_path";

		// Default values
		public const string DEFAULT_VALUE_LANGUAGES_RESOURCE_LOCATION = "Languages";
		public const string DEFAULT_VALUE_DEFAULT_LANGUAGE_ID = "english";

		public string DataID
		{
			get; private set;
		}

		public string DefaultLanguageID
		{
			get; private set;
		}

		public string LanguagesResourceLocation
		{
			get; private set;
		}

		public void SetProperties(string dataID, Properties properties)
		{
			DataID = dataID;
			DefaultLanguageID = properties.GetProp(PROP_DEFAULT_LANGUAGE_ID).GetValue(DEFAULT_VALUE_DEFAULT_LANGUAGE_ID);
			LanguagesResourceLocation = properties.GetProp(PROP_LANGUAGES_RESOURCE_PATH).GetValue(DEFAULT_VALUE_LANGUAGES_RESOURCE_LOCATION);
		}
	}
}