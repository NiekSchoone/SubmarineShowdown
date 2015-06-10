using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{

	private const string typeName = "TypeName";
	private const string gameName = "RoomName";
	
	private HostData[] hostList;
	
	public GameObject playerPrefab;
	
	void Start()
	{
		MasterServer.ipAddress = "127.0.0.1";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "127.0.0.1";
		Network.natFacilitatorPort = 50005;
	}
	
	void OnGUI()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button(new Rect(100,100, 150, 50), "Start Server"))
			{
				StartServer();
			}
			if(GUI.Button(new Rect(100,250,250,100), "Refresh hosts"))
			{
				RefreshHostList();
			}
			if(hostList != null)
			{
				for(int i = 0; i < hostList.Length; i++)
				{
					if(GUI.Button(new Rect(400,100 + (110 * i), 300, 100), hostList[i].gameName))
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
		if(msEvent == MasterServerEvent.HostListReceived)
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
		Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	}
}
