using System.Collections.Generic;
using UnityEngine;


//Class that synchronizes multiple rotateables inside a visualization
public class CompoundRotateable : MonoBehaviour
{
	public Vector3 startingRotation;

	public List<Rotateable> rotateables;

	private Quaternion compoundRotation;

	private void Start()
	{
		compoundRotation = Quaternion.Euler(startingRotation);
		foreach (var r in rotateables)
		{
			if(!r.ignore)
				r.transform.rotation = compoundRotation;
			r.RotateableMovedEvent += OnRotateableMoved;
			r.RotateableActivatedEvent += OnRotateableActivated;
		}
	}

	private void OnRotateableMoved(Quaternion rotation)
	{
		compoundRotation = rotation;
	}

	private void OnRotateableActivated(Transform t)
	{
		t.rotation = compoundRotation;
	}
}
