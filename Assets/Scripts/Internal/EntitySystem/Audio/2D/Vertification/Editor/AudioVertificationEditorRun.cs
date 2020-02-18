using UnityEditor;
using UnityEngine;

namespace Vertification
{
	public class AudioEditorVertification : MonoBehaviour
	{
		[MenuItem("Vertification/Audio")]
		public static void VertifyAudioEditor()
		{
			AudioVertification.Run();
		}
	}
}