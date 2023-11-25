using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public int m_ClientConnectionID;
	public int m_Seed;
	public PlayerData(int clientConnectionID, int seed)
	{
		m_ClientConnectionID = clientConnectionID;
		m_Seed = seed;
	}
}

public class GameLogic : MonoBehaviour
{
	public int m_PlayerSeed = 1000;
	public List<PlayerData> m_ConnectedPlayers = new List<PlayerData>();

	#region connectedPlayers Functions
	public void Add(int clientConnectionID)
	{
		m_ConnectedPlayers.Add(new PlayerData(clientConnectionID, m_PlayerSeed++));
	}
	public void Remove(int clientConnectionID)
	{
		foreach (PlayerData playerData in m_ConnectedPlayers)
		{
			if (playerData.m_ClientConnectionID == clientConnectionID)
			{
				m_ConnectedPlayers.Remove(playerData);
			}
		}
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
