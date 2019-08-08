public static class StaticDatabaseUtils
{
	/// <summary>
	/// Can, based on the base properties given and the target string, localize a string based on the parameters given;
	/// objectID->foo_set = mister_x[Bart] || Results to `Mister Bart` (if the localization is "Mister {0}")
	/// -----------------------------------------------------------------
	/// objectID2->foo_set = has_x_houses[*house_amount] || Results to `He has 5 houses` (if the localization is "He has {0} houses")
	/// objectID2->house_amount = 5
	/// </summary>
	/// <param name="formatSetString">String in format given above (or simply a localization key)</param>
	/// <param name="baseProperties">Base properties of the given object</param>
	/// <returns>LocalizedString based on the keys and given parameters</returns>
	public static LocalizedString LocalizeDatabaseFormatSet(string formatSetString, Properties baseProperties)
	{
		string key = formatSetString.RemoveContentInside('[', ']');
		string content = formatSetString.GetContentInside('[', ']');
		if(content != null)
		{
			string[] contentItems = content.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
			LocalizedString[] localizedItems = new LocalizedString[contentItems.Length];
			for(int i = 0; i < contentItems.Length; i++)
			{
				localizedItems[i] = LocalizeDatabaseFormatSet(GetValueOfSetString(contentItems[i].Trim(), baseProperties), baseProperties);
			}
			return Localization.Instance.LocalizeFormat(GetValueOfSetString(key, baseProperties), localizedItems);
		}
		return Localization.Instance.Localize(GetValueOfSetString(key, baseProperties));
	}

	/// <summary>
	/// Provides you with the true value of a given set item. For example;
	/// objectID->foo_set = house_amount || Set `foo_set` to `house_amount`
	/// -----------------------------------------------------------------
	/// objectID2->foo_set = *house_amount || Set `foo_set` to `5`
	/// objectID2->house_amount = 5
	/// </summary>
	/// <param name="setString">String after the `=` sign in the examples (Set Format)</param>
	/// <param name="baseProperties">Properties to use to dig into the objects values</param>
	/// <returns>String with true value of the string</returns>
	public static string GetValueOfSetString(string setString, Properties baseProperties)
	{
		if(setString.IndexOf('*') == 0)
		{
			setString = setString.Remove(0, 1);
			Properties topProp = StaticDatabaseParser.GetTopPropertiesData(setString, baseProperties, out string key);
			return topProp.GetProp(key).GetValue(setString);
		}
		else
		{
			return setString;
		}
	}
}