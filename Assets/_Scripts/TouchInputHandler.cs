using Nox.Core;
using System;
using UnityEngine;

public class TouchInputHandler : Singleton<TouchInputHandler>
{
	public Camera activeCamera;

	public event Action<Vector2> TouchStartEvent;
	public event Action<Vector2> TouchMovedEvent;
	public event Action<Vector2> TouchEndedEvent;

	private void Update()
	{

#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
			TouchStartEvent?.Invoke(Input.mousePosition);
		else if (Input.GetMouseButton(0))
			TouchMovedEvent?.Invoke(Input.mousePosition);
		else if (Input.GetMouseButtonUp(0))
			TouchEndedEvent?.Invoke(Input.mousePosition);
#endif

		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
				TouchStartEvent?.Invoke(t.position);
			else if (t.phase == TouchPhase.Moved)
				TouchMovedEvent?.Invoke(t.position);
			else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
				TouchEndedEvent?.Invoke(t.position);
		}
	}

	public Ray CameraRay(Vector3 screenPosition)
	{
		return activeCamera.ScreenPointToRay(screenPosition);
	}
}
