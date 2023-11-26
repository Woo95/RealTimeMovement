using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public int m_ClientConnectionID;
	public int m_Seed;
	public Vector3 m_Position;
	public Vector3 m_Velocity;
	public PlayerData(int clientConnectionID, int seed)
	{
		m_ClientConnectionID = clientConnectionID;
		m_Seed = seed;
		m_Position = Vector3.zero;
		m_Velocity = Vector3.zero;
	}
}

public class GameLogic : MonoBehaviour
{
	public int m_PlayerSeed = 1000;
	public List<PlayerData> m_ConnectedPlayers = new List<PlayerData>();

	#region connectedPlayers Functions
	public PlayerData Add(int clientConnectionID)
	{
		PlayerData playerData = new PlayerData(clientConnectionID, m_PlayerSeed++);
		m_ConnectedPlayers.Add(playerData);

		return playerData;
	}
	public void Remove(int clientConnectionID)
	{
		foreach (PlayerData playerData in m_ConnectedPlayers)
		{
			if (playerData.m_ClientConnectionID == clientConnectionID)
			{
				m_ConnectedPlayers.Remove(playerData);
				break;
			}
		}
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
