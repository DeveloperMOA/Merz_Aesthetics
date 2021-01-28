using UnityEngine;

namespace Nox.Core
{
	public abstract class UILayer : MonoBehaviour
	{
		public UIHandler handler { protected get; set; }

		public bool overrideActivation;
		//Should the previous screen get hidden when showing this screen?
		public bool hidePrevious;

		/// <summary>
		/// Callback method executed for every screen.
		/// </summary>
		public virtual void OnScreenStart() { }

		//REMINDER: Never use ScreenManager.Show inside OnScreenUp, because if Hide is used on that other screen, this method it will repromt it
		//making an ugly loop
		//Gets called every time a screen is shown
		public virtual void OnScreenUp() { }

		//Gets called one frame after OnScreenUp so coroutines can run
		public virtual void OnScreenUpAndReady() { }
		//Gets called every time a screen gets hidden
		public virtual void OnScreenDown() { }

		//Gets called right before the screen manager loads a new scene
		public virtual void OnBeforeLoadScene() { }

		// Many screens have a back button so a method for that is included in this class to avoid rewriting it
		// in every child class
		public virtual void Back() { handler.Hide(); }
	}
}