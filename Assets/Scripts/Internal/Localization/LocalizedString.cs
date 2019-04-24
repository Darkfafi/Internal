public struct LocalizedString
{
	public readonly string LanguageKey;
	public readonly string TranslationKey;

	public LocalizedString[] FormatParams
	{
		get; private set;
	}

	private readonly string _translation;

	public static LocalizedString Empty()
	{
		return new LocalizedString(string.Empty);
	}

	public LocalizedString Relocalize()
	{
		return Relocalize(SessionSettings.Request<LocalizationSystem>().LanguageID);
	}

	public LocalizedString Relocalize(string languageKey)
	{
		if(FormatParams != null && !string.IsNullOrEmpty(TranslationKey) && !string.IsNullOrEmpty(LanguageKey))
		{
			LocalizedString[] l = new LocalizedString[FormatParams.Length];
			for(int i = 0; i < l.Length; i++)
			{
				FormatParams[i] = FormatParams[i].Relocalize(languageKey);
			}

			return SessionSettings.Request<LocalizationSystem>().LanguageLocalizeFormat(languageKey, TranslationKey, l);
		}

		return new LocalizedString(languageKey, TranslationKey, string.Empty, null);
	}

	public bool IsEmpty()
	{
		return string.IsNullOrEmpty(_translation);
	}

	public bool IsLocalized()
	{
		return !IsEmpty() && !string.IsNullOrEmpty(LanguageKey) && !string.IsNullOrEmpty(TranslationKey);
	}

	public LocalizedString(string literalString)
	{
		LanguageKey = null;
		TranslationKey = null;
		_translation = literalString;
		FormatParams = null;
	}

	public LocalizedString(string languageID, string key, string translation, params LocalizedString[] formatParams)
	{
		LanguageKey = languageID;
		TranslationKey = key;
		_translation = translation;
		FormatParams = formatParams ?? (new LocalizedString[] { });
	}

	public void SetFormatParams(LocalizedString[] formatParams)
	{
		FormatParams = formatParams ?? (new LocalizedString[] { });
	}

	public string GetTranslation()
	{
		if(FormatParams != null && FormatParams.Length > 0 && !string.IsNullOrEmpty(TranslationKey))
		{
			string t = _translation;
			string[] fp = new string[FormatParams.Length];
			for(int i = 0; i < fp.Length; i++)
			{
				fp[i] = FormatParams[i].GetTranslation();
			}
			return string.Format(t, fp);
		}

		return _translation;
	}

	public override string ToString()
	{
		return GetTranslation();
	}
}
