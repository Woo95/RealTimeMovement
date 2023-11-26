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
                    int mySeed = int.Parse(csv[1]);
					Vector3 position = Vector3.zero;
					Vector3 velocity = Vector3.zero;

					gameLogic.SpawnMySelf(mySeed, position, velocity);
				}
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST:
                {
					Debug.Log("PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST"); // @playerSeed:position:velocity => @p1:pX^pY^pZ:vX^vY^vZ@p2:pX^pY^pZ:vX^vY^vZ...
                    string[] allPlayerDatas = csv[1].Split('@');

                    //p1:px^py^pz:px^py^pz
                    //p2:px^py^pz:px^py^pz
                    //p3:px^py^pz:px^py^pz...
                    Debug.Log(allPlayerDatas[0]);
					for (int i=1; i < allPlayerDatas.Length; i++)
                    {
						Debug.Log(allPlayerDatas[i]);
						string[] eachPlayerData = allPlayerDatas[i].Split(':');

						int seed = int.Parse(eachPlayerData[0]);
						Vector3 position = GetVector3Info(eachPlayerData[1], '^');
						Vector3 velocity = GetVector3Info(eachPlayerData[2], '^');

						gameLogic.SpawnOthers(seed, position, velocity);
					}
				}
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_OTHER_PLAYERS:
                {
					Debug.Log("PTS_CONNECTED_OTHER_PLAYERS");   // recieved newPlayerInfo => PlayerSeed:pX^pY^pZ:vX^vY^vZ
                    string[] newJoinedPlayerData = csv[1].Split(':');
                    int seed = int.Parse(newJoinedPlayerData[0]);
                    Vector3 position = GetVector3Info(newJoinedPlayerData[1], '^');
					Vector3 velocity = GetVector3Info(newJoinedPlayerData[2], '^');

                    gameLogic.SpawnOthers(seed, position, velocity);
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
					int leftPlayerSeed = int.Parse(csv[1]);
					gameLogic.OtherPlayerLeft(leftPlayerSeed);
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

	#region Parse Events
	static public Vector3 GetVector3Info(string msg, char splitChar, int startIdx = 0)
    {
        Vector3 result = Vector3.zero;

		string[] pos = msg.Split(splitChar);
		result.Set(float.Parse(pos[startIdx]), float.Parse(pos[startIdx+1]), float.Parse(pos[startIdx+2]));

		return result;
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

