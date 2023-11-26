using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public Player m_Player;
	public int m_Seed;
	public bool m_isMe;
	public PlayerData(Player player, int seed, bool isMe)
	{
		m_Player = player;
		m_Seed = seed;
		m_isMe = isMe;
	}
}

public class Player : MonoBehaviour
{
	public PlayerData m_PlayerData;

	GameObject m_Character;

	const float m_Speed = 5.0f;

	Vector2 characterPositionInPercent;
	Vector2 characterVelocityInPercent;

	Vector2 m_MoveDirection;

	public void InitData(PlayerData playerData, Vector3 velocity)
    {
		m_Character = gameObject;
		m_Character.SetActive(true);

		m_PlayerData = playerData;
	}

	void Update()
	{
		if (m_PlayerData.m_isMe)
		{
			float hInput = Input.GetAxisRaw("Horizontal");
			float vInput = Input.GetAxisRaw("Vertical");
			m_MoveDirection.Set(hInput, vInput);

			transform.Translate(m_MoveDirection * m_Speed * Time.deltaTime);
		}
	}
}