using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private float walkForce;
	private float jumpForce;
	
	private bool canJump;

	public Rigidbody2D rb2D;
	public NetworkView nView;

	public GameObject bulletPrefab;
	public bool killMeBool = false;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector2 syncStartPosition = Vector2.zero;
	private Vector2 syncEndPosition = Vector2.zero;

	private AudioSource source;

	public int playerID;
	
	void Awake()
	{
		lastSynchronizationTime = Time.time;
	}

	void Start()
	{
		walkForce = 3f;
		jumpForce = 150f;

		canJump = false;

		rb2D = GetComponent<Rigidbody2D>();
		nView = GetComponent<NetworkView>();
		source = GetComponent<AudioSource>();
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
		Vector3 syncPosition = Vector2.zero;
		Vector3 syncVelocity = Vector2.zero;
		bool killMeBoolOut= false;
		if (stream.isWriting)
		{
			syncPosition = rb2D.position;
			stream.Serialize(ref syncPosition);
			killMeBoolOut = killMeBool;
			stream.Serialize(ref killMeBoolOut);
			syncVelocity = rb2D.velocity;
			stream.Serialize(ref syncVelocity);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref killMeBoolOut);

			killMeBool = killMeBoolOut;
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rb2D.position;
		}
	}

	void Update()
	{
		if(nView.isMine)
		{
			Movement();
			Shooting();

			if(killMeBool)
			{
				KillMe(3);
			}
		}
		else 
		{
			SyncedMovement();
		}
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		rb2D.position = Vector2.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	void Movement()
	{
		if(Input.GetKey(KeyCode.A))
		{
			transform.Translate(Vector2.left * walkForce * Time.deltaTime);
		}
		
		if(Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector2.right * walkForce * Time.deltaTime);
		}
		
		if(Input.GetKey(KeyCode.Space))
		{
			if(canJump == true)
			{
				rb2D.AddForce(new Vector2(0, jumpForce));
			}
		}
	}

	void Shooting()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector2 shootDirection = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
			shootDirection = shootDirection-new Vector2(transform.position.x,transform.position.y);
			float angle = Mathf.Atan2(shootDirection.y,shootDirection.x) * Mathf.Rad2Deg - 90;

			Camera.main.GetComponent<CameraShake>().Shake();

			source.Play();

			GameObject newBullet = Network.Instantiate(bulletPrefab, new Vector2(transform.position.x, transform.position.y) + (shootDirection.normalized / 2), Quaternion.Euler(0,0,angle), 0) as GameObject;
			newBullet.GetComponent<Bullet>().bulletID = playerID;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "floor")
		{
			canJump = true;
		}
	}
	
	void OnCollisionExit2D(Collision2D col)
	{
		canJump = false;
	}

	public void KillMe(float killDelay)
	{
		GameObject.FindGameObjectWithTag("nManager").GetComponent<NetworkManager>().SpawnPlayerDelay(killDelay);
		Network.Destroy(this.gameObject);
	}
}
