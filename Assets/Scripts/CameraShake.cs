using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	private Vector3 originPosition;
	private Quaternion originRotation;
	
	public float shake_decay;
	public float shake_intensity;
	
	private bool shaking;

	private Transform _transform;
	
	void OnEnable() 
	{	
		_transform = transform;	
	}
	
	void Update ()
	{	
		if(!shaking)
		{
			return;
		}
		if (shake_intensity > 0f)
		{
			_transform.localPosition = originPosition + Random.insideUnitSphere * shake_intensity;

			_transform.localRotation = new Quaternion(
				originRotation.x + Random.Range (-shake_intensity,shake_intensity),
				originRotation.y + Random.Range (-shake_intensity,shake_intensity),
				originRotation.z + Random.Range (-shake_intensity,shake_intensity),
				originRotation.w + Random.Range (-shake_intensity,shake_intensity));

			shake_intensity -= shake_decay;	
		}else 
		{
			Debug.Log("stopped shaking");
			shaking = false;
			_transform.localPosition = originPosition;
			_transform.localRotation = originRotation;	
		}
	}
	
	public void Shake()
	{
		if(!shaking) 
		{
			originPosition = _transform.localPosition;
			originRotation = _transform.localRotation;	
		}
		shaking = true;
		shake_intensity = 0.01f;
		shake_decay = 0.002f;
	}
}

