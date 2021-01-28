using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
	public static AnalyticsManager Instance
	{
		get => instance;
	}

	private static AnalyticsManager instance;

	public bool useAnalytics;

	private bool visualizationFlag;
	private float visualizationTimer;

	private List<string> viewedArticles;
	private List<float> articleTimers;

	private void Awake()
	{
		if (instance == null)
		{
			viewedArticles = new List<string>();
			articleTimers = new List<float>();
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void VisualizationEvent()
	{
		if (!useAnalytics || ContentManager.Instance.currentItem == null || ContentManager.Instance.currentBrand == null)
			return;

		Brand brand = ContentManager.Instance.currentBrand;
		BrandItem item = ContentManager.Instance.currentItem;

		Dictionary<string, object> data = new Dictionary<string, object>();

		string normalizedBrand = RemoveDiacritics(brand.name);
		data.Add("Brand", normalizedBrand);

		string normalizedName = RemoveDiacritics(item.content.prettyName);
		data.Add("Item", normalizedName);
		data.Add("Type", ContentManager.Instance.isTechnique ? "Technique" : "Product");
		if (ContentManager.Instance.isTechnique)
			data.Add("Pacient", ContentManager.Instance.isMale ? "Male" : "Female");
		data.Add("Mode", ContentManager.Instance.isAR ? "AR" : "3D");
		data.Add("Language", ContentManager.Instance.language);


		Analytics.CustomEvent("VisualizationStarted",data);
	}

	public void ViewArticleEvent(string title)
	{
		if (!useAnalytics)
			return;

		if (!viewedArticles.Contains(title))
		{
			viewedArticles.Add(title);
			articleTimers.Add(60f);
		}

		int index = viewedArticles.IndexOf(title);
		if(articleTimers[index] >= 60f)
		{
			articleTimers[index] = 0;

			Dictionary<string, object> data = new Dictionary<string, object>();
			string normalizedTitle = RemoveDiacritics(title);
			data.Add("Title", normalizedTitle);
			Analytics.CustomEvent("ArticleView",data);
		}
	}

	public void ResetArticleTracking()
	{
		viewedArticles.Clear();
		articleTimers.Clear();
	}

	public void VisitArticleEvent(string title, string link)
	{
		if (!useAnalytics)
			return;

		Dictionary<string, object> data = new Dictionary<string, object>();
		string normalizedTitle = RemoveDiacritics(title);
		data.Add("Title", normalizedTitle);
		data.Add("URL", link);
		Analytics.CustomEvent("ArticleVisit", data);
	}

	public void StartVisualizationTimer()
	{
		if (visualizationFlag || !useAnalytics)
			return;

		visualizationFlag = true;
		visualizationTimer = 0;
	}

	public void StopVisualizationTimer(int step)
	{
		if (!visualizationFlag || !useAnalytics || ContentManager.Instance.currentItem == null || ContentManager.Instance.currentBrand == null)
			return;

		visualizationFlag = false;

		Brand brand = ContentManager.Instance.currentBrand;
		BrandItem item = ContentManager.Instance.currentItem;

		Dictionary<string, object> data = new Dictionary<string, object>();

		string normalizedBrand = RemoveDiacritics(brand.name);
		data.Add("Brand", normalizedBrand);
		string normalizedName = RemoveDiacritics(item.content.prettyName);
		data.Add("Item", normalizedName);
		data.Add("Type", ContentManager.Instance.isTechnique ? "Technique" : "Product");
		if (ContentManager.Instance.isTechnique)
			data.Add("Pacient", ContentManager.Instance.isMale ? "Male" : "Female");
		data.Add("Mode", ContentManager.Instance.isAR ? "AR" : "3D");
		data.Add("Language", ContentManager.Instance.language);
		data.Add("Time", Mathf.Round(visualizationTimer));
		data.Add("Step", "Step" + step);

		Analytics.CustomEvent("VisualizationTime", data);
	}

	public void LoginEvent()
	{
		if (!useAnalytics || CredentialsManager.Instance.data == null || CredentialsManager.Instance.data.user == null)
			return;

		Dictionary<string, object> data = new Dictionary<string, object>();

		DateTime date = DateTime.Now;
		data.Add("Hour", "h" + date.Hour);
		data.Add("Day", date.DayOfWeek.ToString());
		data.Add("Device",SystemInfo.deviceModel);
		data.Add("Management","G" + CredentialsManager.Instance.data.user.gerente);

		Analytics.CustomEvent("Login",data);
	}

	private static string RemoveDiacritics(string text)
	{
		var normalizedString = text.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder();

		foreach (var c in normalizedString)
		{
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}

		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	private void Update()
	{
		if (visualizationFlag)
			visualizationTimer += Time.deltaTime;

		for (int i = 0; i < articleTimers.Count; i++)
			articleTimers[i] += Time.deltaTime;
	}
}
