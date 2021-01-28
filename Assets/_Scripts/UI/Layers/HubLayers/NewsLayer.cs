using Nox.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsLayer : UILayer
{
	public Image headerImage;
	public TextMeshProUGUI headerText;
	public GameObject backButton;
	public Color newsColor;

	public override void OnScreenUp()
	{
		headerImage.color = newsColor; 
		headerText.text = "News Feed";
		backButton.SetActive(false);
	}
}
