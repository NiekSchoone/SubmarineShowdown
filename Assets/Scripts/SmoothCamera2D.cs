using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour 
{
	public float dampTime;
	private Vector3 velocity;
	public Transform target;

	public Camera camera;

	private float rLimit;
	private float lLimit;
	
	void Start()
	{
		dampTime = 0.15f;

		velocity = Vector3.zero;

		camera = GetComponent<Camera>();

		rLimit = 1;
		lLimit = -1;
	}
	
	void Update () 
	{
		if (target)
		{
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, lLimit, rLimit), transform.position.y, transform.position.z);
		}
		
	}
}
