using System.Collections.Generic;
using UnityEngine;

public class ShowcasePage : MonoBehaviour
{
	public List<Handler> handlers;

	public void Display()
	{
		foreach (var handler in handlers)
			handler.AppearEffect();
	}

	public void Hide()
	{
		foreach (var handler in handlers)
			handler.DisappearEffect();
	}
}
