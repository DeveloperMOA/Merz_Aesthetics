using System.Collections.Generic;
using UnityEngine;

public class Navigation3D : MonoBehaviour
{
	public List<int> indexes;
	public List<Collider> colliderButtons;
	public List<SpriteRenderer> indicators;
	public ItemShowcase itemShowcase;


	private void OnEnable()
	{
		TouchInputHandler.Instance.TouchStartEvent += OnTouchStart;
		itemShowcase.PageChangedEvent += OnPageChanged;
	}

	private void OnDisable()
	{
		if (TouchInputHandler.Instance == null)
			return;

		TouchInputHandler.Instance.TouchStartEvent -= OnTouchStart;
		itemShowcase.PageChangedEvent -= OnPageChanged;
	}

	private void Start()
	{
		foreach (var ind in indicators)
			ind.enabled = false;
		if(indicators.Count > 0)
			indicators[0].enabled = true;
	}

	private void OnTouchStart(Vector2 position)
	{
		RaycastHit hit;
		Ray ray = TouchInputHandler.Instance.CameraRay(position);
		if (Physics.Raycast(ray, out hit, 10f))
		{
			if(colliderButtons.Contains(hit.collider))
				itemShowcase.SetDisplayIndex(indexes[colliderButtons.IndexOf(hit.collider)]);
		}
	}

	private void OnPageChanged(int page)
	{
		if(indexes.Contains(page))
		{
			foreach (var ind in indicators)
				ind.enabled = false;
			if(indicators.Count > 0)
				indicators[indexes.IndexOf(page)].enabled = true;
		}
	}
}
