using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LocalizationSystem : ILocalizationSystem, ISettings
{
	public const string LANGUAGES_RESOURCE_LOCATION = "Languages";
	public const string AUTO_DEFAULT_LANGUAGE_ID = "english";

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

			if(!_idToLanguageMap.ContainsKey(value))
			{
				Debug.LogError($"No Language found with ID; {value}, set LanguageID aborted!");
				return;
			}

			_languageID = value;
			LanguageChangedEvent?.Invoke(_languageID);
		}
	}

	private string _languageID;

	private List<Language> _languages = new List<Language>();
	private List<string> _languageIds = new List<string>();
	private Dictionary<string, Language> _idToLanguageMap = new Dictionary<string, Language>();
	private string _originalDefaultLangaugeID;
	private string _defaultLanguageID;

	public LocalizationSystem()
	{
		TextAsset[] assets = Resources.LoadAll<TextAsset>(LANGUAGES_RESOURCE_LOCATION);

		if(assets.Length == 0)
		{
			Debug.LogError($"No text files found at Resource Location; {LANGUAGES_RESOURCE_LOCATION}");
		}

		Initialize(AUTO_DEFAULT_LANGUAGE_ID, assets);
	}

	public LocalizationSystem(string defaultLanguageID, TextAsset[] jsonFiles)
	{
		Initialize(defaultLanguageID, jsonFiles);
	}

	public void SwitchToDefaultLanguage()
	{
		LanguageID = _defaultLanguageID;
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

	public void OverrideDefaultLanguageID(string languageID)
	{
		_defaultLanguageID = languageID;
	}

	public void OverrideDefaultLanguageID(CultureInfo culture)
	{
		Language l = GetLanguage(culture);
		if(l != null)
		{
			OverrideDefaultLanguageID(l.LanguageID);
		}
		else
		{
			Debug.LogError($"No language found with culture of name {culture.EnglishName}");
		}
	}

	public string[] GetAllLanguageIDs()
	{
		return _languageIds.ToArray();
	}

	public Language[] GetAllLanguages()
	{
		return _languages.ToArray();
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
		if(_idToLanguageMap.TryGetValue(languageID, out Language l))
		{
			return l;
		}

		return null;
	}

	public Language GetLanguage(CultureInfo culture)
	{
		return GetLanguage(l => l.CultureInfo == culture);
	}

	public Language GetLanguage(Func<Language, bool> condition)
	{
		for(int i = 0, c = _languages.Count; i < c; i++)
		{
			if(condition(_languages[i]))
			{
				return _languages[i];
			}
		}

		return null;
	}

	public void Reset()
	{
		OverrideDefaultLanguageID(_originalDefaultLangaugeID);
		SwitchToDefaultLanguage();
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

	private void Initialize(string defaultLanguageID, TextAsset[] jsonFiles)
	{
		_languages.Clear();
		_languageIds.Clear();
		_idToLanguageMap.Clear();
		_originalDefaultLangaugeID = _defaultLanguageID = defaultLanguageID;
		if(jsonFiles.Length == 0)
		{
			Debug.LogError($"No Translations given");
			return;
		}

		for(int i = 0, c = jsonFiles.Length; i < c; i++)
		{
			TextAsset asset = jsonFiles[i];
			LanguageData data = JsonUtility.FromJson<LanguageData>(asset.text);

			if(string.IsNullOrEmpty(data.CultureCode))
			{
				Debug.LogError($"No culture found in langauge asset with id {asset.name}");
				continue;
			}

			if(data.Localizations.Length == 0)
			{
				Debug.LogError($"No localizations found in langauge asset with id {asset.name}");
				continue;
			}

			Language l = new Language(asset.name, data);
			_languages.Add(l);
			_languageIds.Add(l.LanguageID);
			_idToLanguageMap.Add(l.LanguageID, l);
		}

		SwitchToDefaultLanguage();
	}
}