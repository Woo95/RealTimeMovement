using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public int m_ClientConnectionID;
	public int m_Seed;
	public Vector3 m_Position;
	public Vector2 m_InputKey;
	public PlayerData(int clientConnectionID, int seed)
	{
		m_ClientConnectionID = clientConnectionID;
		m_Seed = seed;
		m_Position = Vector3.zero;
		m_InputKey = Vector2.zero;
	}

	public void SetData(string posX, string posY, string posZ)
	{
		m_Position.Set(float.Parse(posX), float.Parse(posY), float.Parse(posZ));
	}
	public void SetData(string posX, string posY, string posZ, string hInputKey, string vInputKey)
	{
		m_Position.Set(float.Parse(posX), float.Parse(posY), float.Parse(posZ));
		m_InputKey.Set(float.Parse(hInputKey), float.Parse(vInputKey));
	}
}

public class GameLogic : MonoBehaviour
{
	public int m_PlayerSeed = 1000;
	public List<PlayerData> m_ConnectedPlayers = new List<PlayerData>();

	void Start()
	{
		NetworkServerProcessing.SetGameLogic(this);
	}

	#region connectedPlayers Functions
	public PlayerData Add(int clientConnectionID)
	{
		PlayerData playerData = new PlayerData(clientConnectionID, m_PlayerSeed++);
		m_ConnectedPlayers.Add(playerData);

		return playerData;
	}
	public PlayerData Remove(int clientConnectionID)
	{
		PlayerData removedPlayerData = null;
		foreach (PlayerData playerData in m_ConnectedPlayers)
		{
			if (playerData.m_ClientConnectionID == clientConnectionID)
			{
				removedPlayerData = playerData;
				m_ConnectedPlayers.Remove(removedPlayerData);
				break;
			}
		}
		return removedPlayerData;
	}
	public PlayerData Search(int playerSeed)
	{
		PlayerData foundPlayerData = null;
		foreach (PlayerData playData in m_ConnectedPlayers)
		{
			if (playData.m_Seed == playerSeed)
			{
				foundPlayerData = playData;
				break;
			}
		}
		return foundPlayerData;
	}
	#endregion
}
