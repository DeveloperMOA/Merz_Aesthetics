using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICarousel : MonoBehaviour
{
	public bool snap;
	public float lerpSpeed = .02f;
	public RectTransform viewport;
	public GameObject viewportMask;
	public RectTransform content;
	public RectTransform indicator;
	public CarouselMarker marker;
	public float childWidth;
	public float childSeparation;
	public Color selectedMarkerColor;
	public Color defaultMarkerColor;
	public List<CarouselMarker> markers;

	private int index;
	private float width;
	private float childDelta;
	private float crossThreshold;
	private float initialX;
	private int count;
	private bool isActive;

	private bool validTouch;
	private Vector2 startPoint;
	private Vector2 lastPoint;

	public event Action<int> IndexChangedEvent;
	public event Action<bool> EndReachedEvent;

	private void Awake()
	{
		marker.button.onClick.AddListener(() => { OnMarkerButton(0); });
	}
	/// <summary>
	/// Returns true if the setup was successful
	/// </summary>
	/// <returns></returns>
	public bool Setup(int itemCount)
	{
		markers = new List<CarouselMarker>();
		markers.Add(marker);

		count = itemCount;
		if (itemCount == 0)
			return false;

		width = (itemCount * childWidth) + ((itemCount - 1) * childSeparation);
		childDelta = childWidth + childSeparation;
		crossThreshold = childDelta * 0.2f;
		index = 0;
		initialX = width * 0.5f - childWidth * 0.5f;
		content.anchoredPosition = new Vector2(initialX, 0);

		marker.image.color = selectedMarkerColor;
		
		for(int i = 1; i<itemCount; i++)
		{
			int markerIndex = i;
			var newMarker = Instantiate(marker);
			newMarker.transform.SetParent(indicator, false);
			newMarker.image.color = defaultMarkerColor;
			newMarker.button.onClick.AddListener(() => {OnMarkerButton(markerIndex);});

			markers.Add(newMarker);
		}
		isActive = true;

		return true;
	}

	private void OnMarkerButton(int markerIndex)
	{
		SetIndex(markerIndex,false);
	}

	public void RegisterTouch()
	{
		TouchInputHandler.Instance.TouchStartEvent += OnTouchStart;
		TouchInputHandler.Instance.TouchMovedEvent += OnTouchMoved;
		TouchInputHandler.Instance.TouchEndedEvent += OnTouchEnded;
	}

	public void UnRegisterTouch()
	{
		TouchInputHandler.Instance.TouchStartEvent -= OnTouchStart;
		TouchInputHandler.Instance.TouchMovedEvent -= OnTouchMoved;
		TouchInputHandler.Instance.TouchEndedEvent -= OnTouchEnded;
	}

	public void Clear()
	{
		isActive = false;
		for (int i = markers.Count - 1; i >= 1; i--)
			Destroy(markers[i].gameObject);
		markers.RemoveRange(1, markers.Count - 1);
	}

	//Lerp content towards index position
	public void Update()
	{
		if (!isActive)
			return;

		Vector2 contentPos = content.anchoredPosition;

		float targetX = initialX - childDelta * index;


		float x = snap ? targetX : Mathf.Lerp(contentPos.x, targetX, lerpSpeed);

		contentPos.x = x;

		content.anchoredPosition = contentPos;
	}

	public void ForcePositionUpdate()
	{
		Vector2 contentPos = content.anchoredPosition;
		float targetX = initialX - childDelta * index;
		contentPos.x = targetX;
		content.anchoredPosition = contentPos;
	}

	public void Next()
	{
		SetIndex(index + 1,false);
	}

	public void Previous()
	{
		SetIndex(index - 1,false);
	}

	private void OnTouchStart(Vector2 position)
	{
		//Check if the touch is inside the viewport and not over a something raycastable (e.g. buttons,inputfields,etc.)
		Vector2 localPoint;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, position, null, out localPoint))
		{
			validTouch = viewport.rect.Contains(localPoint) && IsPointerOverMask(position);
			if(validTouch)
			{
				startPoint = position;
				lastPoint = position;
				isActive = false;
			}
		}
	}

	private void OnTouchMoved(Vector2 position)
	{
		if (validTouch && !snap)
		{
			Vector2 delta = position - lastPoint;
			Vector2 contentPos = content.anchoredPosition;
			contentPos.x += delta.x;
			content.anchoredPosition = contentPos;
			lastPoint = position;

			//Check if we should move the index
			float startDelta = position.x - startPoint.x;
			if(Mathf.Abs(startDelta) > crossThreshold)
			{
				SetIndex(index - (int)Mathf.Sign(startDelta),true);
				//End this touch
				validTouch = false;
				isActive = true;
			}
		}
	}

	private void OnTouchEnded(Vector2 position)
	{
		validTouch = false;
		isActive = true;
	}

	public void SetIndex(int newIndex, bool isTouch)
	{
		if(newIndex >= count)
		{
			EndReachedEvent?.Invoke(isTouch);
			return;
		}	

		markers[index].image.color = defaultMarkerColor;

		index = Mathf.Clamp(newIndex, 0, count - 1);

		markers[index].image.color = selectedMarkerColor;

		IndexChangedEvent?.Invoke(index);
	}

	public bool IsPointerOverMask(Vector2 position)
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(position.x, position.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0 && results[0].gameObject == viewportMask;
	}
}
