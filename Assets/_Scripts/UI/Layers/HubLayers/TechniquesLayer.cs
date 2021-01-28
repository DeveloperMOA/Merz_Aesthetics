using Nox.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TechniquesLayer : UILayer
{
	public BrandOption currentBrand;
	public StartTechniqueLayer startItemLayer;
	public Image headerImage;
	public TextMeshProUGUI headerTitle;
	public Button backButton;
	public Transform techniqueParent;
	public ItemOption techniquePrefab;
	public Image background;

	public List<ItemOption> options;

	public override void OnScreenStart()
	{
		options = new List<ItemOption>();
	}

	public override void OnScreenUp()
	{
		ContentManager.Instance.isTechnique = true;

		headerImage.color = currentBrand.brand.primaryColor;
		headerTitle.text = "<b>Técnicas de aplicación</b> " + currentBrand.brand.name;
		backButton.gameObject.SetActive(true);
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(Back);

		if (currentBrand.brand.products.Count > 0)
		{
			background.sprite = currentBrand.brand.techniques[0].content.techniquesBackground;
		}

		//Get all technique options
		foreach (var technique in currentBrand.brand.techniques)
		{
			var newTech = Instantiate(techniquePrefab, techniqueParent, false);
			newTech.gameObject.SetActive(true);
			newTech.brandOption = currentBrand;
			newTech.item = technique;
			newTech.card.sprite = technique.content.techCard;
			newTech.mask.sprite = technique.content.techCard;
			options.Add(newTech);
		}

		foreach (var op in options)
		{
			op.ButtonARSelected += OnAROption;
			op.Button3DSelected += On3DOption;
		}

		ContentManager.Instance.OnItemDownloadCompleted += OnDownloadCompleted;
	}

	public override void OnScreenDown()
	{
		foreach (var op in options)
		{
			op.ButtonARSelected -= OnAROption;
			op.Button3DSelected -= On3DOption;
		}

		for (int i = options.Count - 1; i >= 0; i--)
		{
			Destroy(options[i].gameObject);
		}

		options.Clear();

		ContentManager.Instance.OnItemDownloadCompleted -= OnDownloadCompleted;
	}

	public void OnAROption(ItemOption option)
	{
		if (!Caching.IsVersionCached(option.item.path, option.item.version))
		{
			option.ShowDownloadOverlay();
			ContentManager.Instance.DownloadBundle(option.item);
			return;
		}

		startItemLayer.currentOption = option;
		option.isAR = true;
		handler.Show<StartTechniqueLayer>();
	}

	public void On3DOption(ItemOption option)
	{
		if (!Caching.IsVersionCached(option.item.path, option.item.version))
		{
			option.ShowDownloadOverlay();
			ContentManager.Instance.DownloadBundle(option.item);
			return;
		}

		startItemLayer.currentOption = option;
		option.isAR = false;
		handler.Show<StartTechniqueLayer>();
	}

	private void OnDownloadCompleted(BrandItem item)
	{
		foreach (var option in options)
		{
			if (option.item == item)
				option.HideTechniqueDownloadOverlay();
		}
	}
}
