using Nox.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ShowcaseController : MonoBehaviour
{
	public UIHandler showcaseUIHandler;

	public ItemShowcase itemShowcase;
	public GameObject placementIndicator;
	
	public Camera camera3D;
	public Camera cameraAR;

	public ARSessionOrigin arOrigin;
	public ARRaycastManager arRaycastManager;

	[Header("UI")]
	public Button backButton;
	public Button nextButton;
	public Image headerBack;
	public Image refBack;
	public Image footerBack;
	public Text refText;
	public Text title;
	public Button settings;
	public RectTransform footerOrganizer;
	[Tooltip("For Test Showcase only")]
	public GameObject barPrefabOverride;

	private Pose placementPose;
	private List<ARRaycastHit> hits;
	private bool placementPoseIsValid;
	private bool fixedPosition;
	private int pageCount;
	private bool isAR;

	public void ItemShowcaseHome()
	{
		itemShowcase.SetDisplayIndex(0);
	}

	public void Initialize(bool AR, ShowcaseLocalization localization, ItemShowcase _itemShowcase, GameObject barPrefab)
	{
		if(_itemShowcase != null)
			itemShowcase = _itemShowcase;

		if (barPrefab != null)
		{
			settings.gameObject.SetActive(false);
			//Setup custom navigation bar
			GameObject bar = Instantiate(barPrefab, footerOrganizer, false);
			bar.transform.SetSiblingIndex(1);

			var navigation = bar.GetComponent<NavigationBar>();
			navigation.controller = this;
			itemShowcase.PageChangedEvent += navigation.OnPageChanged;
		}
		else if(barPrefabOverride != null)
        {
			settings.gameObject.SetActive(false);
			//Setup custom navigation bar
			GameObject bar = Instantiate(barPrefabOverride, footerOrganizer, false);
			bar.transform.SetSiblingIndex(1);

			var navigation = bar.GetComponent<NavigationBar>();
			navigation.controller = this;
			itemShowcase.PageChangedEvent += navigation.OnPageChanged;
		}
		else
			settings.gameObject.SetActive(true);

		isAR = AR;
		hits = new List<ARRaycastHit>();
		showcaseUIHandler.Show<HeaderLayer>();
		showcaseUIHandler.Show<FooterLayer>();

		headerBack.color = ContentManager.Instance.currentBrand.primaryColor;
		footerBack.color = ContentManager.Instance.currentBrand.primaryColor;
		refBack.color = ContentManager.Instance.currentBrand.primaryColor;

		backButton.gameObject.SetActive(false);
		pageCount = itemShowcase.pages.Count;
		itemShowcase.title = title;
		itemShowcase.PageChangedEvent += OnPageChanged;
		itemShowcase.StartShowcase(localization);
		refText.text = localization.content["ref"];
		refText.fontSize = itemShowcase.refTextSize;

		TouchInputHandler.Instance.activeCamera = isAR ? cameraAR : camera3D;

		if(AnalyticsManager.Instance != null)
			AnalyticsManager.Instance.StartVisualizationTimer();
	}

	public void OnPageChanged(int index)
	{
		if (index == 0)
		{
			backButton.gameObject.SetActive(false);
			nextButton.gameObject.SetActive(true);
			if (showcaseUIHandler.ScreenIsUp(typeof(ReferencesLayer)))
				showcaseUIHandler.Hide();

		}
		else if (index == pageCount - 1)
		{
			backButton.gameObject.SetActive(true);
			nextButton.gameObject.SetActive(false);
			showcaseUIHandler.Show<ReferencesLayer>();
		}
		else
		{
			backButton.gameObject.SetActive(true);
			nextButton.gameObject.SetActive(true);
			if (showcaseUIHandler.ScreenIsUp(typeof(ReferencesLayer)))
				showcaseUIHandler.Hide();
		}
	}

	public void OnNextButton()
	{
		itemShowcase.Next();
	}

	public void OnBackButton()
	{
		itemShowcase.Back();
	}

	public void OnSettingsButton()
	{
		ContentManager.Instance.UnloadCurrentBundleAndDependencies();
		if (ContentManager.Instance.isTechnique)
			ContentManager.Instance.startTechniqueFlag = true;
		else
			ContentManager.Instance.startProductFlag = true;

		AnalyticsManager.Instance.StopVisualizationTimer(itemShowcase.currentIndex);
		SceneManager.LoadScene(2); //Go to menu scene
	}

	public void OnCrossButton()
	{
		ContentManager.Instance.UnloadCurrentBundleAndDependencies();
		if (ContentManager.Instance.isTechnique)
			ContentManager.Instance.techniquesFlag = true;
		else
			ContentManager.Instance.productsFlag = true;

		AnalyticsManager.Instance.StopVisualizationTimer(itemShowcase.currentIndex);
		SceneManager.LoadScene(2); //Go to menu scene
	}

	public void OnRepositionAR()
	{
		fixedPosition = false;
		itemShowcase.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!isAR)
			return;

		if (fixedPosition)
			return;


		UpdatePlacementPose();
		UpdatePlacementIndicator();

		if(placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			PlaceObject();
		}
	}

	private void UpdatePlacementIndicator()
	{
		if(placementPoseIsValid)
		{
			placementIndicator.SetActive(true);
			placementIndicator.transform.SetPositionAndRotation(placementPose.position,placementPose.rotation);
		}
		else
		{
			placementIndicator.SetActive(false);
		}
	}

	private void UpdatePlacementPose()
	{
		Vector2 screenPoint = cameraAR.ViewportToScreenPoint(new Vector3(0.5f,0.5f,0.5f));
		hits.Clear();
		arRaycastManager.Raycast(screenPoint,hits,TrackableType.Planes);

		placementPoseIsValid = hits.Count > 0;

		if(placementPoseIsValid)
		{
			placementPose = hits[0].pose;

			Vector3 camForward = cameraAR.transform.forward;
			Vector3 camBearing = new Vector3(camForward.x, 0, camForward.z).normalized;
			placementPose.rotation = Quaternion.LookRotation(camBearing);
		}
	}

	private void PlaceObject()
	{
		placementIndicator.SetActive(false);
		fixedPosition = true;
		itemShowcase.gameObject.SetActive(true);
		itemShowcase.gameObject.transform.SetPositionAndRotation(placementPose.position,placementPose.rotation);
	}
}
