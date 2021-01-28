using Nox.Core;
using UnityEngine;
using UnityEngine.UI;

public class NewsPost : MonoBehaviour
{
	public Text title;
	public Text description;
	public Image image;
	public LayoutElement descriptionLayout;
	public LayoutElement postLayout;
	public UIHandler bodyHandler;
	public PostLayer postLayer;
	public string link;

	public void Start()
	{
		float preferredDescriptionHeight = Mathf.Ceil(description.preferredHeight) + 10f;
		descriptionLayout.minHeight = preferredDescriptionHeight;
		descriptionLayout.preferredHeight = preferredDescriptionHeight;

		float fixedElements = 75f + 75f + 347f;
		postLayout.minHeight = postLayout.preferredHeight = fixedElements + preferredDescriptionHeight;
	}

	public void OnSeeMore()
	{
		postLayer.SetPost(this);
		bodyHandler.Show<PostLayer>();
	}
}
