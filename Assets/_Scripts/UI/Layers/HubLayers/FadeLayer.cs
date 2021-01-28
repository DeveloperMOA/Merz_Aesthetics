using Nox.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeLayer : UILayer
{
	public float fadeInDuration;
	public float fadeOutDuration;
	public Graphic[] fadeGraphics;

	public UnityEvent fadeInFinished;
	public UnityEvent fadeOutFinished;

	public IEnumerator ScreenFadeOut(float duration)
	{
		float timer = 0;

		Color[] colors = new Color[fadeGraphics.Length];

		for (int i = 0; i < fadeGraphics.Length; i++)
			colors[i] = fadeGraphics[i].color;


		while (timer < duration)
		{
			float t = timer / duration;
			float a = Mathf.Lerp(0, 1f, t);
			SetFade(a);
			timer += Time.deltaTime;
			yield return null;
		}
		SetFade(1f);

		fadeOutFinished.Invoke();
	}

	public void SetFade(float value)
	{
		foreach(var image in fadeGraphics)
		{
			Color c = image.color;
			c.a = value;
			image.color = c;
		}
	}

	public IEnumerator ScreenFadeIn(float duration)
	{
		float timer = 0;

		Color[] colors = new Color[fadeGraphics.Length];

		for (int i = 0; i < fadeGraphics.Length; i++)
			colors[i] = fadeGraphics[i].color;


		while (timer < duration)
		{
			float t = timer / duration;
			float a = Mathf.Lerp(1f,0, t);
			SetFade(a);
			timer += Time.deltaTime;
			yield return null;
		}
		SetFade(0);

		fadeInFinished.Invoke();
	}

	public void FadeIn()
	{
		StartCoroutine(ScreenFadeIn(fadeInDuration));
	}

	public void FadeOut()
	{
		StartCoroutine(ScreenFadeOut(fadeOutDuration));
	}

	public void SetClickable(bool value)
	{
		foreach (var i in fadeGraphics)
			i.raycastTarget = value;
	}
}
