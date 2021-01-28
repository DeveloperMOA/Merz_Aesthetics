using System;
using UnityEngine;
using UnityEngine.UI;

//Class used for both product and techniques options
public class ItemOption : MonoBehaviour
{
	public BrandItem item;

	public bool isAR;

	public BrandOption brandOption;
	public Image card;
	public GameObject downloadOverlay;
	public Image downloadIndicator;

	public Button buttonAR;
	public Button button3D;
	public Image mask;
	public Text slogan;

	public Image[] buttonFills;

	public event Action<ItemOption> ButtonARSelected;
	public event Action<ItemOption> Button3DSelected;

	[NonSerialized] public bool hasDownloadCallback;

	private void Start()
	{
		if (item.isDownloading)
			ShowDownloadOverlay();

		foreach (var image in buttonFills)
			image.color = brandOption.brand.primaryColor;

		buttonAR.interactable = ContentManager.Instance.ARSupport;

		if(!buttonAR.interactable)
		{
			var graphics = buttonAR.GetComponentsInChildren<Graphic>();
			foreach (var g in graphics)
			{
				Color c = g.color;
				c.a = 0.5f;
				g.color = c;
			}
		}
	}

	public void OnARButton()
	{
		ButtonARSelected?.Invoke(this);
	}

	public void On3DButton()
	{
		Button3DSelected?.Invoke(this);
	}

	public void ShowDownloadOverlay()
	{
		downloadOverlay.SetActive(true);
	}

	public void HideTechniqueDownloadOverlay()
	{
		downloadOverlay.SetActive(false);
	}

	private void Update()
	{
		if(item.isDownloading)
		{
			downloadIndicator.fillAmount = item.downloadProgress;
		}
	}
}
