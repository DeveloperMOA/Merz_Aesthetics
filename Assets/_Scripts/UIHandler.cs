using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox.Core
{
	public class UIHandler : MonoBehaviour
	{
		private void Awake()
		{
			screenStack = new Stack<UILayer>();
			availableScreens = new Dictionary<Type, UILayer>();
			UILayer[] screens = GetComponentsInChildren<UILayer>(true);
			for (int i = 0; i < screens.Length; i++)
			{
				Type screenType = screens[i].GetType();
				availableScreens.Add(screenType, screens[i]);

				if(!screens[i].overrideActivation)
					screens[i].gameObject.SetActive(false);
				screens[i].handler = this;
			}

			foreach (UILayer s in availableScreens.Values)
				s.OnScreenStart();
		}

		private Dictionary<Type, UILayer> availableScreens;
		private Stack<UILayer> screenStack;

		public Type GetCurrentScreenType()
		{
			return screenStack.Count > 0 ? screenStack.Peek().GetType() : null;
		}

		public void Show<T>() where T : UILayer
		{
			Type screenType = typeof(T);
			Show(screenType);
		}

		private void Show(Type screenType)
		{
			UILayer newScreen = availableScreens[screenType];
			if (screenStack.Count != 0 && newScreen.hidePrevious)
				TakeScreenDown(screenStack.Peek());
			screenStack.Push(newScreen);
			StartCoroutine(PutScreenUp(newScreen));
		}

		public void DelayedShow<T>(float delay) where T : UILayer
		{
			Type screenType = typeof(T);
			DelayedShow(screenType, delay);
		}

		private void DelayedShow(Type screenType, float delay)
		{
			UILayer newScreen = availableScreens[screenType];
			//Check if the stack is not empty
			if (screenStack.Count != 0 && newScreen.hidePrevious)
				TakeScreenDown(screenStack.Peek());
			screenStack.Push(newScreen);
			StartCoroutine(DelayedUp(newScreen, delay));
		}

		public void Hide()
		{
			TakeScreenDown(screenStack.Pop());
			if (screenStack.Count == 0) return;
			StartCoroutine(PutScreenUp(screenStack.Peek()));
		}

		public void HideUntil(Type screenType)
		{
			while(screenStack.Peek().GetType() != screenType)
			{
				TakeScreenDown(screenStack.Pop());
				if (screenStack.Count == 0) return;
				StartCoroutine(PutScreenUp(screenStack.Peek()));
			}
		}

		public void DelayedHide(float delay)
		{
			StartCoroutine(DelayedDown(screenStack.Pop(), delay));
			if (screenStack.Count == 0) return;
			StartCoroutine(PutScreenUp(screenStack.Peek()));
		}

		public bool ScreenIsUp(Type screenType)
		{
			return availableScreens[screenType].gameObject.activeSelf;
		}

		public void Clear()
		{
			int count = screenStack.Count;
			for (int i = 0; i < count; i++)
				TakeScreenDown(screenStack.Pop());
		}

		private void TakeScreenDown(UILayer screen)
		{
			if(!screen.overrideActivation)
				screen.gameObject.SetActive(false);
			screen.OnScreenDown();
		}

		private IEnumerator PutScreenUp(UILayer screen)
		{
			if(!screen.overrideActivation)
				screen.gameObject.SetActive(true);
			screen.OnScreenUp();
			yield return null; //Wait a frame so when screens run OnScreenUpAndReady they can actually use coroutines
			screen.OnScreenUpAndReady();
		}

		private IEnumerator DelayedUp(UILayer screen, float delay)
		{
			yield return new WaitForSeconds(delay);
			StartCoroutine(PutScreenUp(screen));
		}

		private IEnumerator DelayedDown(UILayer screen, float delay)
		{
			yield return new WaitForSeconds(delay);
			TakeScreenDown(screen);
		}
	}
}