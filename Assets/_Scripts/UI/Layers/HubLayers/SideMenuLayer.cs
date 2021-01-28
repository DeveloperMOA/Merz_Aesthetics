using Nox.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuLayer : UILayer
{
	public AnimationCurve curve;
	public RectTransform menuRect;
	public Button closeOverlay;
	public Transform brandsParent;
	public BrandOption brandPrefab;
	public UIHandler bodyHandler;

	private Vector2 initialPosition;
	private bool open;
	private bool isInTransition;

	public override void OnScreenStart()
	{
		open = false;
		closeOverlay.gameObject.SetActive(false);
		initialPosition = menuRect.anchoredPosition;
	}

	public List<BrandOption> GenerateBrandOptions()
	{
		var list = new List<BrandOption>();


		foreach(var b in ContentManager.Instance.data.brands)
		{
			var option = Instantiate(brandPrefab,brandsParent,false);
			option.brand = b;
			option.UpdateElements();
			option.gameObject.SetActive(true);
			list.Add(option);
		}

		return list;
	}

	public void ShowMenu()
	{
		if (isInTransition)
			return;

		if (!open)
		{
			closeOverlay.gameObject.SetActive(true);
			StartCoroutine(InAnimation());
		}
	}

	public void OnNavigationTutorialButton()
	{
		handler.Show<NavTutorialLayer>();
	}

	public void OnARTutorialButton()
	{
		handler.Show<ARTutorialLayer>();
	}

	public void On3DTutorialButton()
	{
		handler.Show<Tutorial3DLayer>();
	}

	public void HideMenu()
	{
		if (isInTransition)
			return;

		StartCoroutine(OutAnimation());
	}

	public void InstantlyHideMenu()
	{
		open = false;
		menuRect.anchoredPosition = initialPosition;
		closeOverlay.gameObject.SetActive(false);
	}

	public void OnNewsButton()
	{
		if (isInTransition)
			return;

		bodyHandler.HideUntil(typeof(NewsLayer));
		StartCoroutine(OutAnimation());
	}

	private IEnumerator InAnimation()
	{
		isInTransition = true;
		float duration = curve.keys[curve.keys.Length - 1].time;
		float timer = 0;

		while(timer < duration)
		{
			float xPos = -curve.Evaluate(timer) * initialPosition.x;
			menuRect.anchoredPosition = new Vector2(initialPosition.x + xPos, initialPosition.y);

			timer += Time.deltaTime;
			yield return null;
		}
		open = true;
		isInTransition = false;
	}

	private IEnumerator OutAnimation()
	{
		isInTransition = true;
		float duration = curve.keys[curve.keys.Length - 1].time;
		float timer = duration;
		while(timer > 0)
		{
			float xPos = -curve.Evaluate(timer)*initialPosition.x;
			menuRect.anchoredPosition = new Vector2(initialPosition.x + xPos, initialPosition.y);

			timer -= Time.deltaTime;
			yield return null;
		}
		isInTransition = false;
		open = false;
		menuRect.anchoredPosition = initialPosition;
		closeOverlay.gameObject.SetActive(false);
	}
}
