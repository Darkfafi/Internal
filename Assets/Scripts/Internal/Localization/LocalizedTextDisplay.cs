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

	public LocalizedString LocalizedString
	{
		get
		{
			return _localizedString;
		}
		set
		{
			_localizedString = value;
			Refresh();
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

	private LocalizedString _localizedString;
	private LocalizationSystem _localizationSystem;
	private bool _deInit = false;

	public void Refresh()
	{
		if(localizationSystem != null)
		{
			UpdateText(localizationSystem.LanguageID);
		}
	}

	protected void Awake()
	{
		LocalizedString = localizationSystem.Localize(_localizedStringKey);
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
	}

	private void OnLanguageChangedEvent(string languageID)
	{
		UpdateText(languageID);
	}

	private void UpdateText(string languageID)
	{
		languageID = string.IsNullOrEmpty(_optionalLanguageId) ? languageID : _optionalLanguageId;
		if(LocalizedString.LanguageKey != languageID)
		{
			LocalizedString = LocalizedString.Relocalize(languageID);
		}

		_textComponentToAffect.text = LocalizedString.GetTranslation();
	}
}
