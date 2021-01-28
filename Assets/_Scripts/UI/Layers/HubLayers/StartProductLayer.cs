using Nox.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartProductLayer : UILayer
{
	public ItemOption currentOption;

	public TextMeshProUGUI headerTitle;
	public Image backColor;
	public Image footerColor;
	public Image contentLogo;
	public Text contentSlogan;
	public Image contentImage;
	public Image startButtonImage;
	public Image startButtonIcon;
	public Image languageButtonImage;
	public Button startButton;
	public GameObject languageOptionsContainer;
	public FadeLayer fadeLayer;
	public Image languageButtonOverlay;

	public Sprite iconAR;
	public Sprite icon3D;

	public List<string> languageKeys;
	public List<Sprite> languageImages;
	public List<Button> languageOptions;


	public override void OnScreenUp()
	{
		startButton.interactable = false;

		languageButtonOverlay.enabled = false;

		foreach (var option in languageOptions)
			option.gameObject.SetActive(false);

		foreach (string lan in currentOption.item.content.languages)
		{
			if (languageKeys.Contains(lan))
				languageOptions[languageKeys.IndexOf(lan)].gameObject.SetActive(true);
		}

		languageOptionsContainer.SetActive(false);

		Color primary = currentOption.brandOption.brand.primaryColor;
		Color secondary = primary;
		secondary.r += 0.2f;
		secondary.g += 0.2f;
		secondary.b += 0.2f;

		backColor.color = primary;
		footerColor.color = secondary;

		string mode = currentOption.isAR ? "Realidad Aumentada " : "3D ";

		headerTitle.text = mode + currentOption.item.content.prettyName;

		contentImage.sprite = currentOption.item.content.productSmall;
		contentImage.preserveAspect = true;

		startButtonImage.color = primary;
		startButtonIcon.sprite = currentOption.isAR ? iconAR : icon3D;
		startButtonIcon.preserveAspect = true;

		contentLogo.preserveAspect = true;
		contentLogo.sprite = currentOption.item.content.logo;

		contentSlogan.text = currentOption.item.content.slogan;

		languageButtonImage.color = secondary;

		ContentManager.Instance.currentBrand = currentOption.brandOption.brand;
		ContentManager.Instance.language = "";
		ContentManager.Instance.isAR = currentOption.isAR;
	}

	public void OnLanguageSelect()
	{
		languageOptionsContainer.SetActive(!languageOptionsContainer.activeSelf);
		if (languageOptionsContainer.activeSelf)
			languageButtonOverlay.enabled = false;
	}

	public void OnLanguageButton(string languageKey)
	{
		ContentManager.Instance.language = languageKey;
		languageOptionsContainer.SetActive(false);
		languageButtonOverlay.enabled = true;
		languageButtonOverlay.sprite = languageImages[languageKeys.IndexOf(languageKey)];
		startButton.interactable = true;
	}

	public void OnStartVisualizationButton()
	{
		if (ContentManager.Instance.language.Equals(null) || ContentManager.Instance.language.Equals(""))
			return;
		ContentManager.Instance.currentItem = currentOption.item;
		fadeLayer.FadeOut();
		fadeLayer.SetClickable(true);
	}
}
