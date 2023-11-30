using System.Text;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public Player m_Player;
	public int m_Seed;
	public bool m_IsMe;
	public PlayerData(Player player, int seed, bool isMe)
	{
		m_Player = player;
		m_Seed = seed;
		m_IsMe = isMe;
	}
}

public class Player : MonoBehaviour
{
	Boundary m_Boundary;
	
	public PlayerData m_PlayerData;
	GameObject m_PlayerObject;
	
	const float m_Speed = 5.0f;
	
	Vector3 m_TargetPosition;
	Vector2 m_TargetInputKey;

	#region Send Move Data Variables - Type A
	const int SEND_DATA_PER_SECOND = 10;
	const float DELAY_TIME_INTERVAL = 1.0f / SEND_DATA_PER_SECOND;
	float m_NextSendTime;
	#endregion

	#region Send Move Data Variables - Type B
	float hInputBackup, vInputBackup;
	#endregion

	#region Data Handler
	public void InitData(PlayerData playerData)
	{
		m_PlayerData = playerData;

		m_PlayerObject = gameObject;
		m_PlayerObject.SetActive(true);

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
	#endregion

	#region Move Handler
	#region Type A
	public void SendMoveToServerTypeA()
	{
		Vector3 position = transform.position;
		StringBuilder sendMsg = new StringBuilder();    // Protocal,PlayerSeed,pX,pY,pZ
		sendMsg.Append(ClientToServerSignifiers.PTC_PLAYER_MOVE)
			   .Append(',')
			   .Append(m_PlayerData.m_Seed)
			   .Append(',')
			   .Append(position.x).Append(',').Append(position.y).Append(',').Append(position.z);
		
		NetworkClientProcessing.SendMessageToServer(sendMsg.ToString(), TransportPipeline.ReliableAndInOrder);
	}
	public void SetMovePosition(Vector3 targetPos)
	{
		m_TargetPosition = targetPos;
	}
	#endregion

	#region Type B
	public void SendMoveToServerTypeB(Vector3 inputVector)
	{
		Vector3 position = transform.position;
		StringBuilder sendMsg = new StringBuilder();    // Protocal,PlayerSeed,pX,pY,pZ
		sendMsg.Append(ClientToServerSignifiers.PTC_PLAYER_MOVE2)
			   .Append(',')
			   .Append(m_PlayerData.m_Seed)
			   .Append(',')
			   .Append(position.x).Append(',').Append(position.y).Append(',').Append(position.z)
			   .Append(',')
			   .Append(inputVector.x).Append(',').Append(inputVector.y);

		NetworkClientProcessing.SendMessageToServer(sendMsg.ToString(), TransportPipeline.ReliableAndInOrder);
	}
	public void SetMovePosition(Vector3 targetPos, Vector2 inputKey)
	{
		m_TargetPosition = targetPos;
		m_TargetInputKey = inputKey;
	}
	#endregion
	#endregion

	void Update()
	{
		if (m_PlayerData.m_IsMe)
		{
			float hInput = Input.GetAxisRaw("Horizontal");
			float vInput = Input.GetAxisRaw("Vertical");
			
			Vector3 inputVector = new Vector3(hInput, vInput, 0);

			transform.position += inputVector.normalized * m_Speed * Time.deltaTime;

			#region Send Move Data - Type A (Sending data 10 times/s)
			/*
			if (hInput != 0 || vInput != 0)
			{
				if (Time.time > m_NextSendTime)
				{
					m_NextSendTime = Time.time + DELAY_TIME_INTERVAL;
					SendMoveToServer();
				}
			}
			*/
			#endregion

			#region Send Move Data - Type B (Sending data only once on the keyDown & keyUp)
			if (hInput != hInputBackup || vInput != vInputBackup)
			{
				SendMoveToServerTypeB(inputVector);
			}
			hInputBackup = hInput;
			vInputBackup = vInput;
			#endregion
		}
		else
		{
			#region Type A
			//transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_Speed * Time.deltaTime);   // Type A
			#endregion

			#region Type B
			float hInput = m_TargetInputKey.x;
			float vInput = m_TargetInputKey.y;

			Vector3 inputVector = new Vector3(hInput, vInput, 0);

			transform.position += inputVector.normalized * m_Speed * Time.deltaTime;
			#endregion
		}
		#region Boundary Checker
		Vector3 position = transform.position;
		position.x = Mathf.Clamp(position.x, m_Boundary.min.x, m_Boundary.max.x);
		position.y = Mathf.Clamp(position.y, m_Boundary.min.y, m_Boundary.max.y);

		transform.position = position;
		#endregion
	}
}