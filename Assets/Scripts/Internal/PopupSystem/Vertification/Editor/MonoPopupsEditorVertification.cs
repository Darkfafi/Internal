using Internal;
using UnityEditor;

namespace Vertification
{
	public static class MonoPopupsVertificationEditor
	{
		[MenuItem(InternalConsts.MENU_ITEM_PATH_VERTIFICATIONS + "Popups")]
		public static void VertifyPopupsEditor()
		{
			MonoPopupsVertification.Run();
		}
	}
}