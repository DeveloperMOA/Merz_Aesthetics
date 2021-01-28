using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HubComposition : SceneComposition
{
	public HubController hubController;

	protected override IEnumerator CompositionProcess()
	{
		hubController.Initialize();
		yield return null;
		CallCompletedEvent();
	}
}
