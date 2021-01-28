using Nox.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordRecoveryLayer : UILayer
{
	public InputField recoveryEmail;
	public Text recoveryText;

	public override void OnScreenUp()
	{
		recoveryEmail.text = "";
		recoveryText.text = "";
	}
}
