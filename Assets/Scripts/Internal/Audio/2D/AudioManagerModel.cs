using System.Collections.Generic;

public delegate void AudioHandler(string path, int station, float volumeScale = 1f, float pitchScale = 1f);
public delegate void AudioStationHandler(int station);

public class AudioManagerModel : BaseModel, IAudioPlayer
{
	public event AudioHandler PlayAudioEvent;
	public event AudioHandler PlaySoloAudioEvent;
	public event AudioStationHandler StopAudioEvent;
	public event AudioStationHandler StationSettingsChangedEvent;

	private Dictionary<int, StationSettings> _registeredStations = new Dictionary<int, StationSettings>();
	private EntityFilter<EntityModel> _audioRequesters;

	public AudioManagerModel(params KeyValuePair<int, StationSettings>[] preSetupStations)
	{
		foreach(var pair in preSetupStations)
		{
			SetAudioStationSettings(pair.Key, pair.Value);
		}

		FilterRules fr;
		FilterRules.OpenConstructNoTags();
		FilterRules.AddComponentToConstruct<AudioReqComp>(true);
		FilterRules.CloseConstruct(out fr);

		_audioRequesters = EntityFilter<EntityModel>.Create(fr);
		_audioRequesters.TrackedEvent += OnTrackedEvent;
		_audioRequesters.UntrackedEvent += OnUntrackedEvent;

		EntityModel[] entitiesToTrack = _audioRequesters.GetAll();

		for(int i = 0; i < entitiesToTrack.Length; i++)
		{
			OnTrackedEvent(entitiesToTrack[i]);
		}
	}

	protected override void OnModelDestroy()
	{
		_audioRequesters.TrackedEvent -= OnTrackedEvent;
		_audioRequesters.UntrackedEvent -= OnUntrackedEvent;

		EntityModel[] trackingEntities = _audioRequesters.GetAll();

		for(int i = 0; i < trackingEntities.Length; i++)
		{
			OnUntrackedEvent(trackingEntities[i]);
		}

		StopAudio();

		_registeredStations.Clear();
		_audioRequesters.Clean();
		_audioRequesters = null;
	}

	public int[] GetAllRegisteredStations()
	{
		int[] keys = new int[_registeredStations.Count];
		_registeredStations.Keys.CopyTo(keys, 0);
		return keys;
	}

	public void PlayAudio(string path, int station = 0, float volumeScale = 1f, float pitchScale = 1f)
	{
		if(_audioRequesters == null)
			return;

		StationSettings settings = GetStationSettings(station);
		PlayAudioEvent?.Invoke(path, station, volumeScale * (settings.Muted ? 0f : 1f), pitchScale);
	}

	public void PlaySoloAudio(string path, int station = 0, float volumeScale = 1, float pitchScale = 1f)
	{
		if(_audioRequesters == null)
			return;

		StationSettings settings = GetStationSettings(station);
		PlaySoloAudioEvent?.Invoke(path, station, volumeScale * (settings.Muted ? 0f : 1f), pitchScale);
	}

	public void SetAudioStationSettings(int station, StationSettings settings)
	{
		_registeredStations[station] = settings;
		StationSettingsChangedEvent?.Invoke(station);
	}

	public void StopAudio()
	{
		if(_audioRequesters == null)
			return;

		foreach(var pair in _registeredStations)
		{
			StopAudio(pair.Key);
		}
	}

	public void StopAudio(int station)
	{
		if(_audioRequesters == null)
			return;

		StopAudioEvent?.Invoke(station);
	}

	public StationSettings GetStationSettings(int station)
	{
		StationSettings settings;
		if(!_registeredStations.TryGetValue(station, out settings))
		{
			settings = new StationSettings()
			{
				Pitch = 1f,
				Volume = 1f,
				Loop = false
			};

			SetAudioStationSettings(station, settings);
		}

		return settings;
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		AudioReqComp reqComp = entity.GetComponent<AudioReqComp>();
		reqComp.PlayAudioEvent += PlayAudio;
		reqComp.PlaySoloAudioEvent += PlaySoloAudio;
		reqComp.StopAudioEvent += StopAudio;
		reqComp.StopAllAudioEvent += StopAudio;
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		AudioReqComp reqComp = entity.GetComponent<AudioReqComp>();
		reqComp.PlayAudioEvent -= PlayAudio;
		reqComp.PlaySoloAudioEvent -= PlaySoloAudio;
		reqComp.StopAudioEvent -= StopAudio;
		reqComp.StopAllAudioEvent -= StopAudio;
	}

	public struct StationSettings
	{
		public bool Muted;
		public float Pitch;
		public float Volume;
		public bool Loop;

		public static StationSettings DefaultStationSettings()
		{
			return new StationSettings()
			{
				Muted = false,
				Pitch = 1f,
				Volume = 1f,
				Loop = false
			};
		}
	}
}

public interface IAudioPlayer
{
	event AudioHandler PlayAudioEvent;
	event AudioHandler PlaySoloAudioEvent;
	event AudioStationHandler StopAudioEvent;

	void PlayAudio(string path, int station = 0, float volumeScale = 1f, float pitchScale = 1f);
	void PlaySoloAudio(string path, int station = 0, float volumeScale = 1f, float pitchScale = 1f);
	void StopAudio();
	void StopAudio(int station);
}
