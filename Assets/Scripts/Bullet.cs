using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public Rigidbody2D rb2D;

	private float maxSpeed;

	public int bulletID;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D>();

		maxSpeed = 10f;
	}

	void FixedUpdate() 
	{
		rb2D.AddForce(transform.up * 100);
		rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxSpeed);

		if(transform.position.x <= -10 || transform.position.x >= 10 || transform.position.y <= -8 || transform.position.y >= 10)
		{
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Player")
		{
			if(bulletID != col.gameObject.GetComponent<Player>().playerID)
			{
				col.gameObject.GetComponent<Player>().killMeBool = true;
				Destroy(this.gameObject);
			}
		}else if (col.gameObject.tag != "Player")
		{
			Destroy(this.gameObject);
		}


	}
}
