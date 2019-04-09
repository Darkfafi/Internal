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
			if(focussed)
			{
				_model.Paused = onApplicationPauseState.HasValue ? onApplicationPauseState.Value : false;
				onApplicationPauseState = null;
			}
			else if(!onApplicationPauseState.HasValue)
			{
				onApplicationPauseState = _model.Paused;
				_model.Paused = true;
			}
		}
	}
}
