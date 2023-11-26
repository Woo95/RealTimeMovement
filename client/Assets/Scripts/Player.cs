using System.Text;
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
	Boundary m_Boundary;

	public PlayerData m_PlayerData;
	GameObject m_PlayerObject;

	Vector3 m_Velocity;
	const float m_Speed = 5.0f;
	Vector2 m_MoveDirection;

	Vector3 m_TargetPosition, m_TargetVelocity;

	const int SEND_DATA_PER_SECOND = 50;
	const float DELAY_TIME_INTERVAL = 1.0f / SEND_DATA_PER_SECOND;
	float m_NextSendTime;

	public void InitData(PlayerData playerData, Vector3 velocity)
    {
		m_PlayerObject = gameObject;
		m_PlayerObject.SetActive(true);

		m_PlayerData = playerData;
		m_Velocity = velocity;

		m_TargetPosition = transform.position;

		SetBoundary();
	}
	public void RemoveData()
	{
		if (m_PlayerObject != null)
		{
			Destroy(gameObject);
		}
	}
	public void SetBoundary()
	{
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
	}
	public void SendMoveToServer()
	{
		Vector3 position = transform.position;
		Vector3 velocity = m_Velocity;
		StringBuilder sendMsg = new StringBuilder();    // Protocal,PlayerSeed,pX,pY,pZ,vX,vY,vZ
		sendMsg.Append(ClientToServerSignifiers.PTC_PLAYER_MOVE)
			   .Append(',')
			   .Append(m_PlayerData.m_Seed)
			   .Append(',')
			   .Append(position.x).Append(',').Append(position.y).Append(',').Append(position.z)
			   .Append(',')
			   .Append(velocity.x).Append(',').Append(velocity.y).Append(',').Append(velocity.z);
		
		NetworkClientProcessing.SendMessageToServer(sendMsg.ToString(), TransportPipeline.ReliableAndInOrder);
	}
	public void MoveOtherPlayer(Vector3 targetPos, Vector3 targetVelocity)
	{
		m_TargetPosition = targetPos;
		m_TargetVelocity = targetVelocity;
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

			if (hInput != 0 || vInput != 0)
			{
				if (Time.time > m_NextSendTime)
				{
					m_NextSendTime = Time.time + DELAY_TIME_INTERVAL;
					SendMoveToServer();
				}
			}
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_Speed * Time.deltaTime);
		}
	}
}