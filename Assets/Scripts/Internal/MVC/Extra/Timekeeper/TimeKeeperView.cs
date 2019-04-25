using UnityEngine;

public class TimekeeperView : MonoBaseView
{
	private TimekeeperModel _model;
	private bool? onApplicationPauseState = null;

	protected override void OnViewReady()
	{
		_model = MVCUtil.GetModel<TimekeeperModel>(this);
	}

	protected void Update()
	{
		if(_model != null)
		{
			_model.FrameTick(Time.deltaTime);
		}
	}

	protected override void OnViewDestroy()
	{
		_model = null;
	}

	private void OnApplicationPause(bool pause)
	{
		OnApplicationFocus(!pause);
	}

	private void OnApplicationFocus(bool focussed)
	{
		if(_model != null)
		{
			if(focussed && onApplicationPauseState.HasValue)
			{
				_model.Paused = onApplicationPauseState.Value;
				onApplicationPauseState = null;
			}
			else if(!focussed && !onApplicationPauseState.HasValue)
			{
				onApplicationPauseState = _model.Paused;
				_model.Paused = true;
			}
		}
	}
}
