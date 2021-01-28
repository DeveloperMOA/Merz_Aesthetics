using UnityEngine;
using System.Collections;
using Nox.Core;

public class LoginComposition : SceneComposition
{
	public LoginController controller;
	public UIHandler loginHandler;

	protected override IEnumerator CompositionProcess()
	{
		controller.Initialize(loginHandler);
		loginHandler.Show<LoginLayer>();
		yield return null;
	}
}
