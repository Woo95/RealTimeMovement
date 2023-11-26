using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public Player m_prefabPlayer;
	public Player m_prefabOthers;
	public List<Player> m_ConnectedPlayers = new List<Player>();
	void Start()
	{
		NetworkClientProcessing.SetGameLogic(this);
	}
	public void SpawnMySelf(int mySeed, Vector3 position)
    {
		Player mySelf = Instantiate(m_prefabPlayer, position, Quaternion.identity);
		PlayerData myData = new PlayerData(mySelf, mySeed, true);

		mySelf.InitData(myData);

		m_ConnectedPlayers.Add(mySelf);
	}
    public void SpawnOthers(int otherSeed, Vector3 position)
    {
		Player other = Instantiate(m_prefabOthers, position, Quaternion.identity);
		PlayerData otherData = new PlayerData(other, otherSeed, false);

		other.InitData(otherData);

		m_ConnectedPlayers.Add(other);
	}
	public void OtherPlayerLeft(int leftPlayerSeed)
	{
		foreach (Player player in m_ConnectedPlayers)
		{
			if (player.m_PlayerData.m_Seed == leftPlayerSeed)
			{
				m_ConnectedPlayers.Remove(player);
				player.RemoveData();
				break;
			}
		}
	}

	public void MovePlayer(int movedPlayerSeed, Vector3 targetPos)
	{
		foreach (Player player in m_ConnectedPlayers)
		{
			if (player.m_PlayerData.m_isMe)
				continue;

			if (player.m_PlayerData.m_Seed == movedPlayerSeed)
			{
				player.MoveOtherPlayer(targetPos);
			}
		}
	}
}
