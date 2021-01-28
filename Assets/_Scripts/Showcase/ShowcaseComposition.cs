using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Nox.Core;
using UnityEngine.SceneManagement;

public class ShowcaseComposition : SceneComposition
{
	public bool useAssetBundles;
	public ShowcaseController controller;
	public GameObject arSession;
	public GameObject arSessionOrigin;
	public GameObject cam3D;
	public GameObject planeButton;
	public GameObject placementIndicator;
	public FadeLayer fadeLayer;
	public UIHandler fadeHandler;

	public TextAsset localizationAsset;

	private bool isAR;
	private ShowcaseLocalization loc;

	protected override IEnumerator CompositionProcess()
	{
		fadeHandler.Show<FadeLayer>();
		fadeLayer.SetFade(1f);
		fadeLayer.SetClickable(true);
		//Check if we do AR or 3D

		isAR = ContentManager.Instance.isAR;
		placementIndicator.SetActive(false);
		arSession.SetActive(isAR);
		arSessionOrigin.SetActive(isAR);
		cam3D.SetActive(!isAR);
		planeButton.SetActive(isAR);


		if (useAssetBundles)
		{
			yield return StartCoroutine(ContentManager.Instance.LoadAssetBundle());

			if (ContentManager.Instance.loadedBundle != null)
			{
				localizationAsset = ContentManager.Instance.LoadLocalizationAsset();
				loc = JsonConvert.DeserializeObject<ShowcaseLocalization>(localizationAsset.text);
				GameObject barPrefab = ContentManager.Instance.LoadNavigationBar();
				controller.Initialize(isAR, loc, ContentManager.Instance.InstantiateItemShowcase().GetComponent<ItemShowcase>(), barPrefab);
				yield return StartCoroutine(fadeLayer.ScreenFadeIn(fadeLayer.fadeInDuration));

			}
			else
			{
				SceneManager.LoadScene(2); //Go to menu scene
			}
		}
		else
		{
			controller.Initialize(isAR, JsonConvert.DeserializeObject<ShowcaseLocalization>(localizationAsset.text), null,null);
			yield return StartCoroutine(fadeLayer.ScreenFadeIn(fadeLayer.fadeInDuration));
		}
		yield return null;
	}

	public void TurnOffFadeClick()
	{
		fadeLayer.SetClickable(false);
	}
}
