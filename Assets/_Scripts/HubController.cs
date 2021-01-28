using Nox.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubController : MonoBehaviour
{
	public UIHandler bodyHandler;
	public UIHandler fixedHandler;
	public UIHandler fadeHandler;
	public FadeLayer fadeLayer;
	public TechniquesLayer techniquesLayer;
	public ProductLayer productsLayer;
	public SideMenuLayer sideMenuLayer;
	public PostLayer postLayer;
	public Button sideMenuButton;
	

	public NewsPost postPrefab;
	public Transform newsScrollContent;

	public List<BrandOption> brandOptions;

	private News news;

	private List<NewsPost> newsPosts;

	public void Initialize()
	{
		newsPosts = new List<NewsPost>();
		if(ContentManager.Instance != null)
		{
			var content = ContentManager.Instance.data;
			news = content.news;

			postPrefab.gameObject.SetActive(false);
			for (int i = 0; i<news.articles.Count; i++)
			{
				var article = news.articles[i];
				NewsPost newPost = Instantiate(postPrefab);
				newPost.gameObject.SetActive(true);
				newPost.transform.SetParent(newsScrollContent, false);
				newPost.title.text = article.title;
				newPost.description.text = article.description;
				newPost.postLayer = postLayer;
				newPost.bodyHandler = bodyHandler;
				newPost.link = article.link;
				if(article.sprite != null)
					newPost.image.sprite = article.sprite;
				newsPosts.Add(newPost);
			}

			fadeHandler.Show<FadeLayer>();
			fadeLayer.SetFade(0);
			fadeLayer.SetClickable(false);
		}


		brandOptions = sideMenuLayer.GenerateBrandOptions();

		foreach (BrandOption b in brandOptions)
		{
			b.BrandTechniqueSelected += OnTechniqueButton;
			b.BrandProductSelected += OnProductButton;
		}

		bodyHandler.Show<NewsLayer>();
		fixedHandler.Show<SideMenuLayer>();

		if (ContentManager.Instance.productsFlag)
		{
			ContentManager.Instance.productsFlag = false;
			productsLayer.currentBrand = brandOptions.First(b => b.brand.EqualsBrand(ContentManager.Instance.currentBrand));
			bodyHandler.Show<ProductLayer>();
		}
		else if (ContentManager.Instance.techniquesFlag)
		{
			ContentManager.Instance.techniquesFlag = false;
			techniquesLayer.currentBrand = brandOptions.First(b => b.brand.EqualsBrand(ContentManager.Instance.currentBrand));
			bodyHandler.Show<TechniquesLayer>();
		}
		else if(ContentManager.Instance.startProductFlag)
		{
			ContentManager.Instance.startProductFlag = false;
			productsLayer.currentBrand = brandOptions.First(b => b.brand.EqualsBrand(ContentManager.Instance.currentBrand));
			bodyHandler.Show<ProductLayer>();
			if (ContentManager.Instance.isAR)
				productsLayer.OnAROption(productsLayer.options.First(op => op.item.name.Equals(ContentManager.Instance.currentItem.name)));
			else
				productsLayer.On3DOption(productsLayer.options.First(op => op.item.name.Equals(ContentManager.Instance.currentItem.name)));
		}
		else if(ContentManager.Instance.startTechniqueFlag)
		{
			ContentManager.Instance.startTechniqueFlag = false;
			techniquesLayer.currentBrand = brandOptions.First(b => b.brand.EqualsBrand(ContentManager.Instance.currentBrand));
			bodyHandler.Show<TechniquesLayer>();
			if (ContentManager.Instance.isAR)
				techniquesLayer.OnAROption(techniquesLayer.options.First(op => op.item.name.Equals(ContentManager.Instance.currentItem.name)));
			else
				techniquesLayer.On3DOption(techniquesLayer.options.First(op => op.item.name.Equals(ContentManager.Instance.currentItem.name)));
		}
		
		if(!PlayerPrefs.HasKey("Tutorial"))
		{
			PlayerPrefs.SetInt("Tutorial", 0);
			fixedHandler.Show<NavTutorialLayer>();
		}

		AnalyticsManager.Instance.ResetArticleTracking();
		AnalyticsManager.Instance.LoginEvent();
	}

	public void OnProfileButton()
	{
		if (bodyHandler.GetCurrentScreenType() == typeof(ProfileLayer))
			return;
		bodyHandler.Show<ProfileLayer>();
	}

	public void OnTechniqueButton(BrandOption brandOption)
	{
		//Immediatly hide the side menu
		sideMenuLayer.InstantlyHideMenu();

		//If is the same layer then ignore it
		if (techniquesLayer.currentBrand == brandOption && bodyHandler.GetCurrentScreenType() == typeof(TechniquesLayer))
			return;


		//Set technique layer stuff with brand option
		techniquesLayer.currentBrand = brandOption;

		//Avoid stacking on techniques and product layers
		if (bodyHandler.GetCurrentScreenType() == typeof(ProductLayer) || bodyHandler.GetCurrentScreenType() == typeof(TechniquesLayer))
			bodyHandler.Hide();
		bodyHandler.Show<TechniquesLayer>();
	}

	public void OnProductButton(BrandOption brandOption)
	{
		sideMenuLayer.InstantlyHideMenu();
		if (productsLayer.currentBrand == brandOption && bodyHandler.GetCurrentScreenType() == typeof(ProductLayer))
			return;

		productsLayer.currentBrand = brandOption;
		if (bodyHandler.GetCurrentScreenType() == typeof(ProductLayer) || bodyHandler.GetCurrentScreenType() == typeof(TechniquesLayer))
			bodyHandler.Hide();
		bodyHandler.Show<ProductLayer>();
	}

    public void OnLogOutButton()
	{
		PlayerPrefs.DeleteAll();
		Caching.ClearCache();
		SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
	}

	public void GoToShowcase()
	{
		AnalyticsManager.Instance.VisualizationEvent();
		SceneManager.LoadScene(3);
	}

	private void OnEnable()
	{
		ContentManager.Instance.CorruptBundleEvent += OnLogOutButton;
	}

	private void OnDisable()
	{
		ContentManager.Instance.CorruptBundleEvent -= OnLogOutButton;
	}
}



