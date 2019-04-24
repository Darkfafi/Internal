using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextDisplay : MonoBehaviour
{
	[SerializeField]
	private Text _textComponentToAffect = null;

	[SerializeField]
	private string _localizedStringKey = null;

	[SerializeField]
	private string _optionalLanguageId = null;

	private Func<string> _translationGetter = null;

	private LocalizationSystem _localizationSystem;

	protected void Awake()
	{
		_localizationSystem = SessionSettings.Request<LocalizationSystem>();
	}

	protected void OnEnable()
	{
		_localizationSystem.LanguageChangedEvent += OnLanguageChangedEvent;
		OnLanguageChangedEvent(_localizationSystem.LanguageID);
	}

	protected void OnDisable()
	{
		_localizationSystem.LanguageChangedEvent -= OnLanguageChangedEvent;
	}

	protected void OnDestroy()
	{
		_localizationSystem = null;
	}

	private void OnLanguageChangedEvent(string languageID)
	{
		UpdateText(string.IsNullOrEmpty(_optionalLanguageId) ? languageID : _optionalLanguageId);
	}

	private void UpdateText(string languageID)
	{
		string textToDisplay = string.Empty;

		if(_translationGetter != null)
		{
			textToDisplay = _translationGetter();
		}
		else if(!string.IsNullOrEmpty(_localizedStringKey))
		{
			if(int.TryParse(_localizedStringKey, out int v))
			{
				textToDisplay = _localizationSystem.Localize(languageID, v);
			}
			else
			{
				textToDisplay = _localizationSystem.Localize(languageID, _localizedStringKey);
			}
		}

		_textComponentToAffect.text = textToDisplay;
	}
}
