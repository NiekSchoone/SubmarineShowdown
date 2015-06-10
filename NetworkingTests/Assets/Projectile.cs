using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    Transform transform;
    Rigidbody rb;
    float spawnTime;
    float destroyMeCooldown = 5f;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.forward);
        //transform.Translate(transform.forward*Time.deltaTime);
        Debug.Log(transform.position);
        if(Time.time > spawnTime + destroyMeCooldown && GetComponent<NetworkView>().isMine)
        {
            Network.Destroy(this.gameObject);
        }
    }
}
