using Nox.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductLayer : UILayer
{
	public BrandOption currentBrand;
	public UICarousel carousel;
	public StartProductLayer startProductLayer;
	public Image headerImage;
	public TextMeshProUGUI headerTitle;
	public Button backButton;
	public ItemOption productPrefab;
	public Transform productsParent;
	public Image background;

	public List<ItemOption> options;

	public override void OnScreenStart()
	{
		options = new List<ItemOption>();
	}

	public override void OnScreenUp()
	{
		ContentManager.Instance.isTechnique = false;

		headerImage.color = currentBrand.brand.primaryColor;
		headerTitle.text = "<b>Información de productos</b> " + currentBrand.brand.name;
		backButton.gameObject.SetActive(true);
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(Back);

		if(currentBrand.brand.products.Count > 0)
		{
			background.sprite = currentBrand.brand.products[0].content.productsBackground;
		}

		foreach(var product in currentBrand.brand.products)
		{
			var newProduct = Instantiate(productPrefab,productsParent,false);
			newProduct.gameObject.SetActive(true);
			newProduct.brandOption = currentBrand;
			newProduct.item = product;
			newProduct.card.sprite = product.content.productBig;
			newProduct.slogan.text = product.content.slogan;
			options.Add(newProduct);
		}
		
		foreach (var op in options)
		{
			op.ButtonARSelected += OnAROption;
			op.Button3DSelected += On3DOption;
		}

		carousel.Setup(options.Count);
		carousel.RegisterTouch();

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

		carousel.UnRegisterTouch();
		carousel.Clear();

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

		startProductLayer.currentOption = option;
		option.isAR = true;
		handler.Show<StartProductLayer>();
	}

	public void On3DOption(ItemOption option)
	{
		if (!Caching.IsVersionCached(option.item.path,option.item.version))
		{
			option.ShowDownloadOverlay();
			ContentManager.Instance.DownloadBundle(option.item);
			return;
		}

		startProductLayer.currentOption = option;
		option.isAR = false;
		handler.Show<StartProductLayer>();
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
