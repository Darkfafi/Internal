public static class StringUtils
{
	public static string GetContentInside(this string target, char a, char b, bool includeNestedContent = true)
	{
		int indexA = target.IndexOf(a);
		int indexB = target.LastIndexOf(b);

		if(indexA < 0 || indexB < 0 || indexA == indexB)
			return null;

		string result = target.Substring(indexA + 1, indexB - indexA - 1);

		if(!includeNestedContent)
		{
			result = RemoveContentInside(result, a, b);
		}

		return result;
	}

	public static string RemoveContentInside(this string target, char a, char b)
	{
		string content = GetContentInside(target, a, b, true);
		if(content != null)
		{
			int indexContent = target.IndexOf(content);
			target = target.Remove(indexContent - 1, content.Length + 2);
		}
		return target;
	}
}
