using UnityEngine;
using System.Collections;

public class PlayerRocky : MonoBehaviour {


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
        if (nView.isMine)
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
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * walkForce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * walkForce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (canJump == true)
            {
                rb.AddForce(new Vector2(0, jumpForce));
            }
        }
        if (Input.GetKey(KeyCode.P))
        {
           GameObject.FindGameObjectWithTag("nManager").GetComponent<NetWorkmanagerCopyRocky>().SpawnPlayerDelay(5);
           Network.Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "floor")
        {
            canJump = true;
        }
        
    }

    void OnCollisionExit2D(Collision2D col)
    {
        canJump = false;
    }
    

    /*public float force;

    public Rigidbody rb;
    public NetworkView nView;
    public Transform bulletSpawn;
    public Transform bullet;
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private NetWorkmanagerCopyRocky nManager;

    void Start()
    {
        force = 5f;

        rb = GetComponent<Rigidbody>();
        nView = GetComponent<NetworkView>();
        nManager =  GameObject.FindGameObjectWithTag("nManager").GetComponent<NetWorkmanagerCopyRocky>();
        /*lastSynchronizationTime = 0f;
        syncDelay = 0f;
        syncTime = 0f;
        syncStartPosition = Vector3.zero;
        syncEndPosition = Vector3.zero;
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

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }


    void Update()
    {
        if (nView.isMine)
        {
            InputMovement();
            if (Input.GetKeyDown("space"))
            {
                Network.Instantiate(bullet, bulletSpawn.position, Quaternion.identity, 0);
            }
            if(Input.GetKeyDown("r"))
            {
                ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            }
        }
        else
        {
            SyncedMovement();
        }
    }

    void InputMovement()
    {
        if (Input.GetKey(KeyCode.W))
            rb.MovePosition(rb.position + Vector3.forward * force * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            rb.MovePosition(rb.position - Vector3.forward * force * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            rb.MovePosition(rb.position + Vector3.right * force * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            rb.MovePosition(rb.position - Vector3.right * force * Time.deltaTime);
    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        rb.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }

    [RPC] void ChangeColorTo(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);

        if (GetComponent<NetworkView>().isMine)
        {
            GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
        }
    }
    */
}
