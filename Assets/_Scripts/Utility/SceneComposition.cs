using Nox.Core;
using System;
using System.Collections;

/// <summary>
/// Class used for scene initialization and dependency solving
/// </summary>
public abstract class SceneComposition : Singleton<SceneComposition>
{
	public event Action CompositionCompletedEvent;

	void Start()
	{
		StartCoroutine(CompositionProcess());
	}

	void OnDestroy()
	{
		CompositionDestroy();
	}

	protected virtual IEnumerator CompositionProcess() { yield return null;}
	protected virtual void CompositionDestroy(){}

	protected void CallCompletedEvent()
	{
		CompositionCompletedEvent?.Invoke();
	}
}