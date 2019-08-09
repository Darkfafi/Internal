using UnityEngine;

[ExecuteInEditMode]
public class SafeArea : MonoBehaviour
{
	[SerializeField]
	private RectTransform _targetRectTransform = null;

#if UNITY_EDITOR
	[SerializeField]
	private Rect _debugRectTransform = new Rect();

	[SerializeField]
	private bool _useDebugRectTransform = false;

	private bool _debugMode = false;
#endif

	private Canvas _parentCanvas = null;

	protected void Awake()
	{
		UpdateParentCanvas(_targetRectTransform);
		ApplySafeArea(Screen.safeArea);
	}

#if UNITY_EDITOR
	protected void Update()
	{
		if(_useDebugRectTransform != _debugMode)
		{
			_debugMode = _useDebugRectTransform;
			_debugRectTransform = Screen.safeArea;
		}

		if(_debugMode)
		{
			ApplySafeArea(_debugRectTransform);
		}
	}

	protected void OnDrawGizmos()
	{
		if(_useDebugRectTransform)
		{
			Gizmos.DrawGUITexture(_debugRectTransform, Texture2D.whiteTexture);
		}
	}
#endif

	protected void OnBeforeTransformParentChanged()
	{
		UpdateParentCanvas(_targetRectTransform);
	}

	private void ApplySafeArea(Rect safeArea)
	{
		if(_parentCanvas == null)
		{
			if(!UpdateParentCanvas(_targetRectTransform))
			{
				return;
			}
		}

		Vector2 anchorMin = safeArea.position;
		Vector2 anchorMax = safeArea.position + safeArea.size;
		anchorMin.x /= _parentCanvas.pixelRect.width;
		anchorMin.y /= _parentCanvas.pixelRect.height;
		anchorMax.x /= _parentCanvas.pixelRect.width;
		anchorMax.y /= _parentCanvas.pixelRect.height;
		_targetRectTransform.anchorMin = anchorMin;
		_targetRectTransform.anchorMax = anchorMax;
	}

	private bool UpdateParentCanvas(Transform target)
	{
		if(target == null)
			return false;

		_parentCanvas = target.GetComponent<Canvas>();
		if(_parentCanvas == null && target.parent != null)
		{
			return UpdateParentCanvas(target.parent);
		}

		return false;
	}
}