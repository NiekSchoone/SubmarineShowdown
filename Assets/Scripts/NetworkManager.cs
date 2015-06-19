using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{

	private const string typeName = "NetworkingTest";
	private const string gameName = "NetworkingTestRoom";
	public string IPString = "IP Goes Here";
	
	private HostData[] hostList;
	
	public GameObject playerPrefab;

	public int idCounter;

	private bool amISpawningAPlayer;

	void Start()
	{
		MasterServer.ipAddress = "127.0.0.1";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "127.0.0.1";
		Network.natFacilitatorPort = 50005;

		idCounter = 0;

		amISpawningAPlayer = false;

		RefreshHostList();
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int idCounterOut = 0;

		if(stream.isWriting)
		{
			idCounterOut = idCounter;
			stream.Serialize(ref idCounterOut);
		}else
		{
			stream.Serialize(ref idCounterOut);
			idCounter = idCounterOut;
		}
	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 150, 50), "Start Server"))
			{
				MasterServer.ipAddress = "127.0.0.1";
				Network.natFacilitatorIP = "127.0.0.1";
				StartServer();
			}
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh hosts"))
			{
				/*for (int i = 0; i < 255; i++) 
				{
					MasterServer.ipAddress = "172.17.56." + i;
					Network.natFacilitatorIP = "172.17.56." + i;
					RefreshHostList();
				}*/
				MasterServer.ipAddress = IPString;
				Network.natFacilitatorIP = IPString;
				Debug.Log("masterIP" + MasterServer.ipAddress);
				RefreshHostList();
			}
			
			IPString = GUI.TextField(new Rect(100, 220, 200, 20), IPString, 25);
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
					{
						JoinServer(hostList[i]);
					}
				}
			}
		}
	}
	
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server is initialized");
		SpawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server joined");
		SpawnPlayer();
	}
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
		}
	}
	
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	private void SpawnPlayer()
	{
		Debug.Log("a player has spawned?");
		GameObject newPlayer = Network.Instantiate(playerPrefab, new Vector3(Random.Range(-7.5f, 7), 5f, 0f), Quaternion.identity, 0) as GameObject;
		idCounter++;
		newPlayer.GetComponent<Player>().playerID = idCounter;
	}

	public void SpawnPlayerDelay(float delay)
	{
		if(amISpawningAPlayer == false)
		{
			Invoke("SpawnPlayer", delay);
			amISpawningAPlayer = true;
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
