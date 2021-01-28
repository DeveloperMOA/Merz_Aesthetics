using Nox.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartTechniqueLayer : UILayer
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
	public Image pacientButtonImage;
	public Button startButton;
	public GameObject languageOptionsContainer;
	public GameObject pacientOptionsContainer;
	public FadeLayer fadeLayer;
	public Text pacientButtonText;
	public Image languageButtonOverlay;
	public Image pacientButtonOverlay;
	public Sprite iconAR;
	public Sprite icon3D;

	public List<string> languageKeys;
	public List<Sprite> languageImages;
	public List<Button> languageOptions;
	public List<Sprite> patientSprites;

	private bool pacientFlag;

	public override void OnScreenUp()
	{
		pacientFlag = false;
		startButton.interactable = false;

		pacientButtonText.enabled = true;
		pacientButtonOverlay.enabled = false;
		languageButtonOverlay.enabled = false;

		foreach (var option in languageOptions)
			option.gameObject.SetActive(false);

		foreach (string lan in currentOption.item.content.languages)
		{
			if (languageKeys.Contains(lan))
				languageOptions[languageKeys.IndexOf(lan)].gameObject.SetActive(true);
		}

		pacientOptionsContainer.SetActive(false);
		languageOptionsContainer.SetActive(false);

		Color primary = currentOption.brandOption.brand.primaryColor;
		Color secondary = currentOption.brandOption.brand.primaryColor;
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
		pacientButtonImage.color = secondary;

		footerColor.color = secondary;

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

	public void OnPatientSelect()
	{
		pacientOptionsContainer.SetActive(!pacientOptionsContainer.activeSelf);
		if (pacientOptionsContainer.activeSelf)
		{
			pacientButtonOverlay.enabled = false;
			pacientButtonText.enabled = true;
		}
	}

	public void OnLanguageButton(string languageKey)
	{
		ContentManager.Instance.language = languageKey;
		languageOptionsContainer.SetActive(false);
		languageButtonOverlay.enabled = true;
		languageButtonOverlay.sprite = languageImages[languageKeys.IndexOf(languageKey)];
		if(pacientFlag)
			startButton.interactable = true;
	}

	public void OnPacientButton(bool isMale)
	{
		ContentManager.Instance.isMale = isMale;
		pacientOptionsContainer.SetActive(false);
		pacientFlag = true;
		pacientButtonText.enabled = false;
		pacientButtonOverlay.enabled = true;
		pacientButtonOverlay.sprite = isMale ? patientSprites[0] : patientSprites[1];
		if(!ContentManager.Instance.language.Equals(null) && !ContentManager.Instance.language.Equals(""))
			startButton.interactable = true;
	}

	public void OnStartVisualizationButton()
	{
		if (ContentManager.Instance.language.Equals(null) || ContentManager.Instance.language.Equals("") || !pacientFlag)
			return;
		ContentManager.Instance.currentItem = currentOption.item;
		fadeLayer.FadeOut();
		fadeLayer.SetClickable(true);
	}
}
