using System.Collections.Generic;

public static class ArrayListUtils
{
	public static bool InBounds<T>(this T[] array, int index)
	{
		return index >= 0 && index < array.Length;
	}

	public static bool InBounds<T>(this List<T> list, int index)
	{
		return index >= 0 && index < list.Count;
	}
}
