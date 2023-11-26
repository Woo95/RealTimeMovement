using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

		switch (signifier)
		{
            case ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER:
                {
					Debug.Log("PTS_CONNECTED_NEW_PLAYER");
                    int playerSeed = int.Parse(csv[1]);
					Vector3 position = Vector3.zero;
					Vector3 velocity = Vector3.zero;

					gameLogic.SpawnMySelf(playerSeed, position, velocity);
				}
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST:
                {
					Debug.Log("PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST");
				}
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_OTHER_PLAYERS:
                {
					Debug.Log("PTS_CONNECTED_OTHER_PLAYERS");
				}
                break;
			/*
			case ServerToClientSignifiers.PTS_PLAYER_MOVE:
				{
					Debug.Log("PTS_PLAYER_MOVE");
				}
				break;
            */
			case ServerToClientSignifiers.PTS_PLAYER_LEFT:
				{
					Debug.Log("PTS_PLAYER_LEFT");
				}
				break;
		}

		//gameLogic.DoSomething();

	}

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
	public const int PTC_CONNECTED_NEW_PLAYER               = 1;
	public const int PTC_CONNECTED_NEW_PLAYER_RECEIVE_LIST  = 2;
	public const int PTC_CONNECTED_OTHER_PLAYERS            = 3;
	public const int PTC_PLAYER_MOVE                        = 4;
	public const int PTC_PLAYER_LEFT                        = 5;
}

static public class ServerToClientSignifiers
{
	public const int PTS_CONNECTED_NEW_PLAYER               = 1;
	public const int PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST  = 2;
	public const int PTS_CONNECTED_OTHER_PLAYERS            = 3;
	public const int PTS_PLAYER_MOVE                        = 4;
	public const int PTS_PLAYER_LEFT                        = 5;
}
#endregion

