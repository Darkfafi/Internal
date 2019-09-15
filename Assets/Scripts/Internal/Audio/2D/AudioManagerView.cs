using System.Collections.Generic;
using UnityEngine;

public class AudioManagerView : MonoBaseView
{
	private AudioManagerModel _audioManagerModel;
	private Dictionary<int, AudioSource> _stationToAudioSource = new Dictionary<int, AudioSource>();

	protected override void OnViewReady()
	{
		_audioManagerModel = MVCUtil.GetModel<AudioManagerModel>(this);
		_audioManagerModel.PlayAudioEvent += OnPlayAudioEvent;
		_audioManagerModel.PlaySoloAudioEvent += OnPlaySoloAudioEvent;
		_audioManagerModel.StopAudioEvent += OnStopAudioEvent;
		_audioManagerModel.StationSettingsChangedEvent += OnStationSettingsChangedEvent;
	}

	private void OnPlayAudioEvent(string path, int station, float volumeScale = 1f, float pitchScale = 1f)
	{
		AudioSource audioSource = GetAudioSourceForStation(station);
		if(audioSource.isActiveAndEnabled)
		{
			AudioManagerModel.StationSettings settings = _audioManagerModel.GetStationSettings(station);
			audioSource.clip = AudioLibrary.GetAudioClip(path);
			audioSource.pitch = settings.Pitch * pitchScale;
			audioSource.volume = settings.Volume * volumeScale;
			audioSource.Play();
		}
	}

	private void OnPlaySoloAudioEvent(string path, int station, float volumeScale = 1f, float pitchScale = 1f)
	{
		AudioSource audioSource = GetAudioSourceForStation(station);
		if(audioSource.isActiveAndEnabled)
		{
			AudioManagerModel.StationSettings settings = _audioManagerModel.GetStationSettings(station);
			audioSource.pitch = settings.Pitch * pitchScale;
			audioSource.PlayOneShot(AudioLibrary.GetAudioClip(path), volumeScale);
		}
	}

	private void OnStopAudioEvent(int station)
	{
		GetAudioSourceForStation(station).Stop();
	}

	private void OnStationSettingsChangedEvent(int station)
	{
		AudioSource audioSource = GetAudioSourceForStation(station);
		if(audioSource.isActiveAndEnabled)
		{
			AudioManagerModel.StationSettings settings = _audioManagerModel.GetStationSettings(station);
			audioSource.pitch = settings.Pitch;
			audioSource.volume = settings.Muted ? 0f : settings.Volume;
			audioSource.loop = settings.Loop;
		}
	}

	protected override void OnViewDestroy()
	{
		_audioManagerModel.PlayAudioEvent -= OnPlayAudioEvent;
		_audioManagerModel.PlaySoloAudioEvent -= OnPlaySoloAudioEvent;
		_audioManagerModel.StopAudioEvent -= OnStopAudioEvent;
		_audioManagerModel.StationSettingsChangedEvent -= OnStationSettingsChangedEvent;

		_stationToAudioSource = null;
		_audioManagerModel = null;
	}

	private AudioSource GetAudioSourceForStation(int station)
	{
		AudioSource audioSource;
		if(!_stationToAudioSource.TryGetValue(station, out audioSource))
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			_stationToAudioSource[station] = audioSource;
			OnStationSettingsChangedEvent(station);
		}

		return audioSource;
	}
}
