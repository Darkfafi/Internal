using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Vertification
{
	public static class AudioVertification
	{
		public static void Run()
		{
			Vertifier.GenericVerification("Audio",
					new Vertifier.TestStepData("Audio Resources", VertifyAudioResources)
				);
		}

		private static Vertifier.TestResult VertifyAudioResources()
		{
			string message = null;
			bool success = true;

			Type[] typesInProject = Assembly.GetAssembly(typeof(AudioResourcePathAttribute)).GetTypes().ToArray();

			foreach(Type projectFileType in typesInProject)
			{
				FieldInfo[] fields = projectFileType.GetFields(BindingFlags.Public | BindingFlags.Static).Where(t => t.GetCustomAttributes(typeof(AudioResourcePathAttribute), true).Length > 0 && t.IsLiteral && !t.IsInitOnly).ToArray();
				foreach(FieldInfo audioResourceFileField in fields)
				{
					object v = audioResourceFileField.GetValue(null);
					if(v != null)
					{
						string audioResourcePath = v.ToString();
						AudioClip clip = Resources.Load<AudioClip>(audioResourcePath);
						if(clip == null)
						{
							success = false;
							message = string.Format("No audio clip found for field {0} under path Resources/{1}", audioResourceFileField.Name, audioResourcePath);
						}
						else
						{
							Debug.Log(string.Format("<color='green'>Const {0} validated</color>", audioResourceFileField.Name));
						}
					}
				}
			}

			return new Vertifier.TestResult(success, message);
		}
	}
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class AudioResourcePathAttribute : Attribute
{

}