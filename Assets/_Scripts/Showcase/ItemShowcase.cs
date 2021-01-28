using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowcase : MonoBehaviour
{
	public Text title;
	public int refTextSize = 14; 
	public List<ShowcasePage> pages;
	public ShowcaseLocalization localization;

	[NonSerialized]public int currentIndex;

	public event Action<int> PageChangedEvent;	

	public void StartShowcase(ShowcaseLocalization _localization)
	{
		localization = _localization;
		foreach (var page in pages)
			page.gameObject.SetActive(false);
		
		currentIndex = 0;
		title.text = localization.titles[currentIndex];
		pages[currentIndex].gameObject.SetActive(true);
		pages[currentIndex].Display();

		LocalizeTextMesh[] textMeshLoc = GetComponentsInChildren<LocalizeTextMesh>(true);

		foreach (var locText in textMeshLoc)
			locText.Localize(localization.content);
	}

	public void SetDisplayIndex(int index)
	{
		if (index == currentIndex)
			return;

		var currentPage = pages[currentIndex];
		currentPage.Hide();
		currentPage.gameObject.SetActive(false);
		var nextPage = pages[index];
		title.text = localization.titles[index];
		nextPage.gameObject.SetActive(true);
		nextPage.Display();
		currentIndex = index;
		PageChangedEvent?.Invoke(currentIndex);
	}

	public void Next()
	{
		int newIndex = currentIndex + 1;
		if (newIndex >= pages.Count)
			return;
		SetDisplayIndex(newIndex);
	}

	public void Back()
	{
		int newIndex = currentIndex - 1;
		if (newIndex < 0)
			return;
		SetDisplayIndex(newIndex);
	}
}
