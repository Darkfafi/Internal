using System;

namespace MVC
{
	public class TimekeeperModel : BaseModel
	{
		public event Action<bool> TimePauseStateChangedEvent;
		public delegate void FrameTickHandler(float deltaTime, float timeScale);

		public float TimeScale = 1f;

		public bool Paused
		{
			get
			{
				return _isPaused;
			}
			set
			{
				if (_isPaused != value)
				{
					_isPaused = value;
					if (TimePauseStateChangedEvent != null)
					{
						TimePauseStateChangedEvent(_isPaused);
					}
				}
			}
		}

		public double SecondsPassedSessionUnscaled
		{
			get; private set;
		}
		public double SecondsPassedSessionScaled
		{
			get; private set;
		}

		private FrameTickHandler _frameTickAction;
		private bool _isPaused = false;

		public void FrameTick(float deltaTime)
		{
			if (!Paused)
			{
				SecondsPassedSessionUnscaled += deltaTime;
				SecondsPassedSessionScaled += deltaTime * TimeScale;

				if (_frameTickAction != null)
				{
					_frameTickAction(deltaTime, TimeScale);
				}
			}
		}

		public void ListenToFrameTick(FrameTickHandler callback)
		{
			_frameTickAction += callback;
		}

		public void UnlistenFromFrameTick(FrameTickHandler callback)
		{
			_frameTickAction -= callback;
		}

		protected override void OnModelDestroy()
		{
			_frameTickAction = null;
		}
	}
}