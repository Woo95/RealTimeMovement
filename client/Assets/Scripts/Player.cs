using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
	private Boundary m_Boundary;

	public PlayerData m_PlayerData;

	GameObject m_PlayerObject;

	const float m_Speed = 5.0f;

	Vector2 characterPositionInPercent;
	Vector2 characterVelocityInPercent;

	Vector2 m_MoveDirection;

	public void InitData(PlayerData playerData, Vector3 velocity)
    {
		m_PlayerObject = gameObject;
		m_PlayerObject.SetActive(true);
		m_PlayerData = playerData;

		#region Set Boundary
		Camera camera = Camera.main;
		Vector3 pos = camera.transform.position;
		SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
		Vector2 playerSize = playerSpriteRenderer.bounds.size * 0.5f;

		m_Boundary.min = new Vector2(pos.x - camera.orthographicSize * camera.aspect, 
									 pos.y - camera.orthographicSize);
		m_Boundary.max = new Vector2(pos.x + camera.orthographicSize * camera.aspect,
									 pos.y + camera.orthographicSize);

		m_Boundary.min += playerSize;
		m_Boundary.max -= playerSize;
		#endregion
	}

	void Update()
	{
		if (m_PlayerData.m_isMe)
		{
			float hInput = Input.GetAxisRaw("Horizontal");
			float vInput = Input.GetAxisRaw("Vertical");
			m_MoveDirection.Set(hInput, vInput);

			transform.Translate(m_MoveDirection * m_Speed * Time.deltaTime);

			#region Boundary Checker
			Vector3 position = transform.position;
			position.x = Mathf.Clamp(position.x, m_Boundary.min.x, m_Boundary.max.x);
			position.y = Mathf.Clamp(position.y, m_Boundary.min.y, m_Boundary.max.y);

			transform.position = position;
			#endregion
		}
	}
}