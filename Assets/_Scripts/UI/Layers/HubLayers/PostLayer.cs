using Nox.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PostLayer : UILayer
{
	public Button backButton;
	public Text title;
	public Text description;
	public LayoutElement descriptionLayout;
	public Image image;
	public string link;

	public List<Image> images;
	public List<Text> texts;

	private NewsPost post;

	public override void OnScreenStart()
	{
		images = GetComponentsInChildren<Image>().ToList();
		texts = GetComponentsInChildren<Text>().ToList();

		SetVisibility(false);
	}

	private void SetVisibility(bool value)
	{
		float opacity = value ? 1f : 0;

		foreach (var i in images)
		{
			Color c = i.color;
			c.a = opacity;
			i.color = c;
			i.raycastTarget = value;
		}

		foreach (var t in texts)
		{
			Color c = t.color;
			c.a = opacity;
			t.color = c;
			t.raycastTarget = value;
		}
	}

	public void SetPost(NewsPost _post)
	{
		post = _post;
		title.text = post.title.text;
		description.text = post.description.text;
		image.sprite = post.image.sprite;
		link = post.link;

		float preferredDescriptionHeight = Mathf.Ceil(description.preferredHeight) + 10f;
		descriptionLayout.minHeight = preferredDescriptionHeight;
		descriptionLayout.preferredHeight = preferredDescriptionHeight;
	}

	public override void OnScreenUp()
	{
		backButton.gameObject.SetActive(true);
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(Back);

		SetVisibility(true);

		AnalyticsManager.Instance.ViewArticleEvent(post.title.text);
	}

	public void OnVisitButton()
	{
		if (link != null && !link.Equals(""))
		{
			AnalyticsManager.Instance.VisitArticleEvent(post.title.text,link);
			Application.OpenURL(link);
		}
		else
			MobileMessage.ShowAlert("Open URL failed", "Invalid URL");
		
	}

	public override void OnScreenDown()
	{
		SetVisibility(false);
	}
}
