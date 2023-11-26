using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public Player m_prefabPlayer;
	public Player m_prefabOthers;
	public List<PlayerData> m_ConnectedPlayers = new List<PlayerData>();
	void Start()
	{
		NetworkClientProcessing.SetGameLogic(this);
	}
	public void SpawnMySelf(int mySeed, Vector3 position, Vector3 velocity)
    {
		Player mySelf = Instantiate(m_prefabPlayer, position, Quaternion.identity);
		PlayerData myData = new PlayerData(mySelf, mySeed, true);

		mySelf.InitData(myData, velocity);

	}
    public void SpawnOthers(int otherSeed, Vector3 position, Vector3 velocity)
    {
		Player other = Instantiate(m_prefabOthers, position, Quaternion.identity);
		PlayerData otherData = new PlayerData(other, otherSeed, true);

		other.InitData(otherData, velocity);
	}
}
