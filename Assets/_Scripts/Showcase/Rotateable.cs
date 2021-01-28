using System;
using UnityEngine;


public enum Axis
{
	X,
	Y,
	Z
}

//Class that allows a 3D object with a collider to be rotated with user input
public class Rotateable : MonoBehaviour
{
	public bool ignore;
	public float speed = 0.5f;
	public Axis axis = Axis.Y;

	private bool isRotating;

	private Vector2 lastPosition;

	public event Action<Quaternion> RotateableMovedEvent;
	public event Action<Transform> RotateableActivatedEvent;

	private bool startFlag;
	private Vector2 startPosition;

   /* private void Start()
    {
        if (transform.name == "RSK")
        {
            Transform _skin = transform.GetChild(0);
            Transform cuadritoAnimado = _skin.transform.GetChild(0);
            Transform jntRorot = cuadritoAnimado.transform.GetChild(0);
            Transform jntBody = jntRorot.transform.GetChild(0);
            Debug.Log("Nombre del Gameobject = " + cuadritoAnimado.name);
            Debug.Log("Nombre del Gameobject = " + jntBody.name);
            cuadritoAnimado.position = Vector3.zero;
            jntBody.position = Vector3.zero;
        }
    }*/

    private void OnEnable()
	{
        
       
        if (ignore)
			return;
		RotateableActivatedEvent?.Invoke(transform);
		TouchInputHandler.Instance.TouchStartEvent += OnTouchStart;
		TouchInputHandler.Instance.TouchEndedEvent += OnTouchEnd;
		TouchInputHandler.Instance.TouchMovedEvent += OnTouchMove;
	}

	private void OnDisable()
	{
		if (TouchInputHandler.Instance == null || ignore)
			return;

		TouchInputHandler.Instance.TouchStartEvent -= OnTouchStart;
		TouchInputHandler.Instance.TouchEndedEvent -= OnTouchEnd;
		TouchInputHandler.Instance.TouchMovedEvent -= OnTouchMove;
	}

	private void OnTouchStart(Vector2 position)
	{
		startFlag = true;
		startPosition = position;
	}

	private void OnTouchMove(Vector2 position)
	{
		if (!isRotating)
			return;

		float delta = position.x - lastPosition.x;

		switch (axis)
		{
			case Axis.X:
				transform.Rotate(new Vector3(-delta * speed,0, 0));
				break;
			case Axis.Y:
				transform.Rotate(new Vector3(0, -delta * speed, 0));
				break;
			case Axis.Z:
				transform.Rotate(new Vector3(0, 0, -delta * speed));
				break;
		}
		
		RotateableMovedEvent?.Invoke(transform.rotation);

		lastPosition = position;
	}

	private void OnTouchEnd(Vector2 position)
	{
		isRotating = false;
	}

	private void FixedUpdate()
	{
		if(startFlag)
		{
			RaycastHit hit;
			Ray ray = TouchInputHandler.Instance.CameraRay(startPosition);
			if(Physics.Raycast(ray, out hit, 10f))
			{
				isRotating = hit.transform != null && hit.transform == transform;
				lastPosition = startPosition;
			}
			startFlag = false;
		}
	}
}
