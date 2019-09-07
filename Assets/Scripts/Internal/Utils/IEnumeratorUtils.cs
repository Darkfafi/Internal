using System.Collections;
using System.Threading.Tasks;

public static class IEnumeratorUtils
{
	public static async void StartAsync(this IEnumerator enumerator)
	{
		while(enumerator.MoveNext())
		{
			await Task.Delay(1);
		}
		enumerator = null;
	}
}
