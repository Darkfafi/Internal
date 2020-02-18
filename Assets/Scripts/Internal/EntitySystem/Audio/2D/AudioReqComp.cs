using System;

namespace MVC.ECS
{
	public class AudioReqComp : BaseModelComponent, IAudioPlayer
	{
		public event AudioHandler PlayAudioEvent;
		public event AudioHandler PlaySoloAudioEvent;
		public event AudioStationHandler StopAudioEvent;
		public event Action StopAllAudioEvent;

		public void PlayAudio(string path, int station = 0, float volumeScale = 1f, float pitchScale = 1f)
		{
			PlayAudioEvent?.Invoke(path, station, volumeScale, pitchScale);
		}

		public void PlaySoloAudio(string path, int station = 0, float volumeScale = 1, float pitchScale = 1f)
		{
			PlaySoloAudioEvent?.Invoke(path, station, volumeScale, pitchScale);
		}

		public void StopAudio()
		{
			StopAllAudioEvent?.Invoke();
		}

		public void StopAudio(int station)
		{
			StopAudioEvent?.Invoke(station);
		}
	}
}