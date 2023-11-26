using UnityEngine;
using UnityEngine.UI;

public class UI_Gameplay : MonoBehaviour
{
	#region singletone
	public static UI_Gameplay instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(instance);
		}
	}
	#endregion

	public Text connectedPlayer;

	private void Start()
	{
	}

	public void SetConnectedPlayers(int count)
	{
		connectedPlayer.text = count.ToString();
	}
}