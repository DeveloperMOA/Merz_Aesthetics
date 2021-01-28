using System;
using UnityEngine;

public abstract class Handler : MonoBehaviour
{
	public event Action<Handler> AppearCompleted;
	public event Action<Handler> DisappearCompleted;

	public abstract void AppearEffect();
	public abstract void DisappearEffect();

	protected void CallAppearCompleted()
	{
		AppearCompleted?.Invoke(this);
	}

	protected void CallDisappearCompleted()
	{
		DisappearCompleted?.Invoke(this);
	}
}
