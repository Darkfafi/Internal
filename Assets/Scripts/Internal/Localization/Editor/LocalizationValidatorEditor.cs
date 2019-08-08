using UnityEditor;
using UnityEngine;

public class LocalizationValidatorEditor
{
	[MenuItem("Localization/Validate Localizations")]
	public static void Validate()
	{
		LocalizationValidator.ValidationResponse response = LocalizationValidator.ValidateLocalization();
		foreach(var settingValidation in response.SettingsValidations)
		{
			if(!string.IsNullOrEmpty(settingValidation.ErrorMessage))
			{
				Debug.LogError($"{settingValidation.SettingsID}:<color=red>'{settingValidation.ErrorMessage}'</color>");
			}
			else
			{
				Debug.Log($"{settingValidation.SettingsID}: <color=green>Validated Successfully!</color>");
			}

			foreach(var languageValidation in settingValidation.LanguageValidations)
			{
				if(languageValidation.HasComplication)
				{
					Debug.LogError($"▶{languageValidation.LanguageID}: <color=red>Has Complications!</color>");

					if(languageValidation.KeysNotLocalized.Length > 0)
						PrintComplications("red", "black", "Keys Not Localized", languageValidation.KeysNotLocalized);

					if(languageValidation.MissingKeys.Length > 0)
						PrintComplications("red", "black", "Keys Missing", languageValidation.MissingKeys);

					if(languageValidation.KeysTooMany.Length > 0)
						PrintComplications("red", "black", "Keys Too Many", languageValidation.KeysTooMany);

					if(languageValidation.DuplicateKeys.Length > 0)
						PrintComplications("red", "black", "Duplicate Keys", languageValidation.DuplicateKeys);
				}
				else
				{
					Debug.Log($"▶{languageValidation.LanguageID}: <color=green>Valid!</color>");
				}
			}
		}
	}

	private static void PrintComplications(string colorTitle, string colorKey, string message, string[] keys)
	{
		Debug.LogWarning($"-▶<color={colorTitle}>{message} ({keys.Length})</color>");
		for(int i = 0; i < keys.Length; i++)
		{
			Debug.LogWarning($"- {i} ▶<color={colorKey}>{keys[i]}</color>");
		}
	}
}
