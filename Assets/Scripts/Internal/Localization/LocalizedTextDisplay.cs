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

	public Text TextComponent
	{
		get
		{
			return _textComponentToAffect;
		}
	}

	protected LocalizationSystem localizationSystem
	{
		get
		{
			if(_localizationSystem == null && !_deInit)
			{
				_localizationSystem = SessionSettings.Request<LocalizationSystem>();
			}

			return _localizationSystem;
		}
	}

	private Func<string> _translationGetter = null;
	private LocalizationSystem _localizationSystem;
	private bool _deInit = false;

	public void Refresh()
	{
		UpdateText(localizationSystem.LanguageID);
	}

	public void SetTranslationGetter(Func<string> translationGetter)
	{
		_translationGetter = translationGetter;
	}

	protected void OnEnable()
	{
		localizationSystem.LanguageChangedEvent += OnLanguageChangedEvent;
		Refresh();
	}

	protected void OnDisable()
	{
		localizationSystem.LanguageChangedEvent -= OnLanguageChangedEvent;
	}

	protected void OnDestroy()
	{
		_deInit = true;
		_localizationSystem = null;
		SetTranslationGetter(null);
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
				textToDisplay = localizationSystem.Localize(languageID, v);
			}
			else
			{
				textToDisplay = localizationSystem.Localize(languageID, _localizedStringKey);
			}
		}

		_textComponentToAffect.text = textToDisplay;
	}
}
