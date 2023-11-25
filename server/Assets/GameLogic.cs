using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public List<int> m_ConnectedPlayers = new List<int>();

	#region connectedPlayers Functions
	public void Add(int clientConnectionID)
	{
		m_ConnectedPlayers.Add(clientConnectionID);
	}
	public void Remove(int clientConnectionID)
	{
		m_ConnectedPlayers.Remove(clientConnectionID);
	}
	public int Count()
	{
		return m_ConnectedPlayers.Count;
	}
	#endregion

	void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            NetworkServerProcessing.SendMessageToClient("2,Hello client's world, sincerely your network server", 0, TransportPipeline.ReliableAndInOrder);
    }

}
