using System;
using UnityEngine;
using UnityEngine.UI;

public class BrandOption : MonoBehaviour
{
	public Brand brand;

	public Image header;
	public Text title;

	public LayoutElement layout;

	private bool toggle;

	public event Action<BrandOption> BrandTechniqueSelected;
	public event Action<BrandOption> BrandProductSelected;

	public void UpdateElements()
	{
		header.color = brand.primaryColor;
		title.text = brand.name + " +";
	}

	public void OnTechniqueButton()
	{
		ContentManager.Instance.currentBrand = brand;
		BrandTechniqueSelected?.Invoke(this);
	}

	public void OnProductButton()
	{
		ContentManager.Instance.currentBrand = brand;
		BrandProductSelected?.Invoke(this);
	}

	public void OnToggle()
	{
		toggle = !toggle;
		int value = toggle ? 190 : 77;
		layout.minHeight = layout.preferredHeight = value;
	}

}
