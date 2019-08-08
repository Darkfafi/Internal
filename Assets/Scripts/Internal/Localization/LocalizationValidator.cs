using System;
using System.Collections.Generic;

public class LocalizationValidator
{
	public static ValidationResponse ValidateLocalization()
	{
		StaticDatabase<LocalizationSystem.SettingsData> settingsDatabase = LocalizationSystem.ParseSettingsDatabase();
		LocalizationSystem.SettingsData[] allSettingsData = settingsDatabase.GetAllData();
		LocalizationSystem system = null;
		List<ValidationResponse.SettingsValidation> responceItems = new List<ValidationResponse.SettingsValidation>();
		for(int i = 0, c = allSettingsData.Length; i < c; i++)
		{
			string errorMessage = string.Empty;
			if(system == null)
			{
				system = new LocalizationSystem(allSettingsData[i].DataID, out errorMessage);
			}
			else
			{
				system.SetSettings(allSettingsData[i].DataID, out errorMessage);
			}

			// Check languages
			List<ValidationResponse.SettingsValidation.LanguageValidation> languageValidations = new List<ValidationResponse.SettingsValidation.LanguageValidation>();
			if(string.IsNullOrEmpty(errorMessage))
			{
				List<string> keysNotLocalized = new List<string>();
				List<string> missingKeys = new List<string>();
				List<string> keysTooMany = new List<string>();
				string[] languageIDs = system.GetAllLanguageIDs();
				for(int j = 0; j < languageIDs.Length; j++)
				{
					ValidationResponse.SettingsValidation.LanguageValidation response = ValidateLanguage(system, system.GetLanguage(languageIDs[j]));
					languageValidations.Add(response);
					if(response.HasComplication)
					{
						errorMessage = "Languages have complications";
					}
				}
			}
			responceItems.Add(new ValidationResponse.SettingsValidation(allSettingsData[i].DataID, errorMessage, languageValidations.ToArray()));
		}
		return new ValidationResponse(responceItems.ToArray());
	}

	private static ValidationResponse.SettingsValidation.LanguageValidation ValidateLanguage(LocalizationSystem localizationSystem, Language language)
	{
		List<string> keysNotLocalized = new List<string>();
		List<string> missingKeys = new List<string>();
		List<string> keysTooMany = new List<string>();
		List<string> duplicateKeys = new List<string>();
		List<string> keysScanned = new List<string>();
		bool isDefaultLanguage = localizationSystem.DefaultLanguageID == language.LanguageID;
		Language defaultLanguage = null;

		Array.ForEach(language.LanguageData.Localizations, (languageLocalization) => 
		{
			if(string.IsNullOrEmpty(languageLocalization.Translation))
			{
				keysNotLocalized.Add(languageLocalization.Key);
			}
			if(!keysScanned.Contains(languageLocalization.Key))
			{
				keysScanned.Add(languageLocalization.Key);
			}
			else
			{
				duplicateKeys.Add(languageLocalization.Key);
			}
			// Compare diff with default language if not the default language
			if(!isDefaultLanguage)
			{
				if(defaultLanguage == null)
				{
					defaultLanguage = localizationSystem.GetLanguage(localizationSystem.DefaultLanguageID);
				}

				if(!defaultLanguage.HasTranslation(languageLocalization.Key))
				{
					keysTooMany.Add(languageLocalization.Key);
				}
			}
		});

		if(!isDefaultLanguage)
		{
			// Check for missing keys in the given language
			Array.ForEach(defaultLanguage.LanguageData.Localizations, (defaultLocalization) =>
			{
				if(!language.HasTranslation(defaultLocalization.Key))
				{
					missingKeys.Add(defaultLocalization.Key);
				}
			});
		}

		return new ValidationResponse.SettingsValidation.LanguageValidation
		(
			language.LanguageID, 
			isDefaultLanguage, 
			keysNotLocalized.ToArray(), 
			missingKeys.ToArray(), 
			keysTooMany.ToArray(),
			duplicateKeys.ToArray()
		);
	}

	public struct ValidationResponse
	{
		public SettingsValidation[] SettingsValidations;

		public ValidationResponse(SettingsValidation[] validations)
		{
			SettingsValidations = validations;
		}

		public struct SettingsValidation
		{
			public string SettingsID;
			public string ErrorMessage;
			public LanguageValidation[] LanguageValidations;

			public SettingsValidation(string settingsID, string errorMessage, LanguageValidation[] languageValidations)
			{
				SettingsID = settingsID;
				ErrorMessage = errorMessage;
				LanguageValidations = languageValidations;
			}

			public struct LanguageValidation
			{
				public string LanguageID;
				public bool IsDefault;
				public string[] KeysNotLocalized;
				public string[] MissingKeys;
				public string[] KeysTooMany;
				public string[] DuplicateKeys;

				public bool HasComplication
				{
					get
					{
						return KeysNotLocalized.Length > 0 || MissingKeys.Length > 0 || KeysTooMany.Length > 0 || DuplicateKeys.Length > 0;
					}
				}

				public LanguageValidation(string languageID, bool isDefault, string[] notLocalizedKeys, string[] missingKeys, string[] keysTooMany, string[] duplicateKeys)
				{
					LanguageID = languageID;
					IsDefault = isDefault;
					KeysNotLocalized = notLocalizedKeys;
					MissingKeys = missingKeys;
					KeysTooMany = keysTooMany;
					DuplicateKeys = duplicateKeys;
				}
			}
		}
	}
}
