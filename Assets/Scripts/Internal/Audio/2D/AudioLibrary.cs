using System;
using System.Collections.Generic;
using UnityEngine;

public static class AudioLibrary
{
	private static Dictionary<string, AudioClip> _audioRuntimeDict = new Dictionary<string, AudioClip>();

	public static AudioClip GetAudioClip(string resourcePath)
	{
		if(!_audioRuntimeDict.TryGetValue(resourcePath, out AudioClip clip))
		{
			clip = Resources.Load<AudioClip>(resourcePath);

			if(clip != null)
			{
				_audioRuntimeDict[resourcePath] = clip;
			}
			else
			{
				throw new Exception("No Clip found at path Resources/ " + resourcePath);
			}
		}

		return clip;
	}
}