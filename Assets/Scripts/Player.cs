using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private float walkForce;
	private float jumpForce;
	
	private bool canJump;

	public Rigidbody2D rb;
	public NetworkView nView;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	private SpriteRenderer sRenderer;

	void Awake()
	{
		lastSynchronizationTime = Time.time;
	}

	void Start()
	{
		walkForce = 3f;
		jumpForce = 150f;

		canJump = false;

		rb = GetComponent<Rigidbody2D>();
		nView = GetComponent<NetworkView>();
		sRenderer = GetComponent<SpriteRenderer>();
	}

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(nView.isMine)
		{
			Camera.main.GetComponent<SmoothCamera2D>().target = transform;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting)
		{
			syncPosition = rb.position;
			stream.Serialize(ref syncPosition);
			
			syncVelocity = rb.velocity;
			stream.Serialize(ref syncVelocity);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rb.position;
		}
	}

	void Update()
	{
		if(nView.isMine)
		{
			Movement();
		}
		else 
		{
			SyncedMovement();
		}
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		rb.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	void Movement()
	{
		if(Input.GetKey(KeyCode.A))
		{
			transform.Translate(Vector3.left * walkForce * Time.deltaTime);
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector3.right * walkForce * Time.deltaTime);
		}
		
		if(Input.GetKey(KeyCode.Space))
		{
			if(canJump == true)
			{
				rb.AddForce(new Vector2(0, jumpForce));
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		canJump = true;
	}
	
	void OnCollisionExit2D(Collision2D col)
	{
		canJump = false;
	}
}
