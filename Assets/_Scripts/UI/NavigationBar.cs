using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBar : MonoBehaviour
{
	public RectTransform indicator;
	public List<int> indexes;
	public List<RectTransform> buttons;

	public ShowcaseController controller;

	public void OnPageChanged(int page)
	{
		//Find minimum index
		int miniumIndex = 0;
		for(int i = 1; i<indexes.Count; i++)
		{
			if (indexes[i] > page)
				break;
			miniumIndex = i;
		}

		indicator.anchoredPosition = buttons[miniumIndex].anchoredPosition;
	}

	public void OnNavigationButton(int button)
	{
		indicator.anchoredPosition = buttons[button].anchoredPosition;
		controller.itemShowcase.SetDisplayIndex(indexes[button]);
	}

	public void OnSettingsButton()
	{
		controller.OnSettingsButton();
	}

}
