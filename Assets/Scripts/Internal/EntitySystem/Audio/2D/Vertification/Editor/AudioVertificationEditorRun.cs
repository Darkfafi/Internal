using Internal;
using UnityEditor;

namespace Vertification
{
	public static class AudioEditorVertification
	{
		[MenuItem(InternalConsts.MENU_ITEM_PATH_VERTIFICATIONS + "Audio")]
		public static void VertifyAudioEditor()
		{
			AudioVertification.Run();
		}
	}
}