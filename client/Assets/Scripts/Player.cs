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

	GameObject character;

	Vector2 characterPositionInPercent;
	Vector2 characterVelocityInPercent;
	const float CharacterSpeed = 0.25f;
	float DiagonalCharacterSpeed;

	public void InitData(PlayerData playerData, Vector3 velocity)
    {
		m_PlayerData = playerData;

		DiagonalCharacterSpeed = Mathf.Sqrt(CharacterSpeed * CharacterSpeed + CharacterSpeed * CharacterSpeed) / 2f;

		Sprite circleTexture = Resources.Load<Sprite>("Circle");

		character = new GameObject("Character");

		character.AddComponent<SpriteRenderer>();
		character.GetComponent<SpriteRenderer>().sprite = circleTexture;
	}

	void Update()
	{

		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)
			|| Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
		{
			characterVelocityInPercent = Vector2.zero;

			if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
			{
				characterVelocityInPercent.x = DiagonalCharacterSpeed;
				characterVelocityInPercent.y = DiagonalCharacterSpeed;
			}
			else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
			{
				characterVelocityInPercent.x = -DiagonalCharacterSpeed;
				characterVelocityInPercent.y = DiagonalCharacterSpeed;
			}
			else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
			{
				characterVelocityInPercent.x = DiagonalCharacterSpeed;
				characterVelocityInPercent.y = -DiagonalCharacterSpeed;
			}
			else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
			{
				characterVelocityInPercent.x = -DiagonalCharacterSpeed;
				characterVelocityInPercent.y = -DiagonalCharacterSpeed;
			}
			else if (Input.GetKey(KeyCode.D))
				characterVelocityInPercent.x = CharacterSpeed;
			else if (Input.GetKey(KeyCode.A))
				characterVelocityInPercent.x = -CharacterSpeed;
			else if (Input.GetKey(KeyCode.W))
				characterVelocityInPercent.y = CharacterSpeed;
			else if (Input.GetKey(KeyCode.S))
				characterVelocityInPercent.y = -CharacterSpeed;
		}

		characterPositionInPercent += (characterVelocityInPercent * Time.deltaTime);

		Vector2 screenPos = new Vector2(characterPositionInPercent.x * (float)Screen.width, characterPositionInPercent.y * (float)Screen.height);
		Vector3 characterPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
		characterPos.z = 0;
		character.transform.position = characterPos;
	}
}
