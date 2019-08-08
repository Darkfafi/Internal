public class Localization
{
	public static LocalizationSystem Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new LocalizationSystem();
			}
			return _instance;
		}
	}

	private static LocalizationSystem _instance = null;
}
