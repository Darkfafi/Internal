using System;
using System.Collections.Generic;
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

	public void OverrideDefaultLanguageID(string languageID)
	{
		_defaultLanguageID = languageID;
	}

	public string[] GetAllLanguageIDs()
	{
		return _languageIds.ToArray();
	}

	public Language[] GetAllLanguages()
	{
		return _languages.ToArray();
	}

	public string Localize(int number)
	{
		return Localize(LanguageID, number);
	}

	public string Localize(string key)
	{
		return Localize(LanguageID, key);
	}

	public string Localize(string languageKey, int number)
	{
		return number.ToString();
	}

	public string Localize(string languageKey, string key)
	{
		Language language = GetLanguage((l) => l.LanguageID == languageKey);
		if(language != null)
		{
			if(language.TryGetTranslation(key, out string translation))
			{
				return translation;
			}

			return "k> " + key;
		}

		return "l- " + (string.IsNullOrEmpty(languageKey) ? "No Language Specified" : languageKey);
	}

	public Language GetLanguage(string languageID)
	{
		if(_idToLanguageMap.TryGetValue(languageID, out Language l))
		{
			return l;
		}

		return null;
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