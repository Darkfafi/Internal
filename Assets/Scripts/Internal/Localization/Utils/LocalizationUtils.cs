public static class LocalizationUtils 
{
	public static LocalizationSystem GetSystem()
	{
		return SessionSettings.Request<LocalizationSystem>();
	}
}
