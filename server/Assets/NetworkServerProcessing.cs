using UnityEngine;
using System.Text;

static public class NetworkServerProcessing
{
    const bool DEBUG = false;
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        if (DEBUG) Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        switch (signifier)
        {
			/*
            case ClientToServerSignifiers.PTC_CONNECTED_NEW_PLAYER:
                {

                }
                break;
            case ClientToServerSignifiers.PTC_CONNECTED_NEW_PLAYER_RECEIVE_LIST:
                {

                }
                break;
            case ClientToServerSignifiers.PTC_CONNECTED_OTHER_PLAYERS:
                {

                }
                break;
            */
			case ClientToServerSignifiers.PTC_PLAYER_MOVE:
                {
                    StringBuilder sendMsg = new StringBuilder();
                    sendMsg.Append(ServerToClientSignifiers.PTS_PLAYER_MOVE) // Protocal,PlayerSeed,posX,posY,posZ
                           .Append(",")
                           .Append(csv[1])                                                          // Seed
                           .Append(",")
                           .Append(csv[2]).Append(",").Append(csv[3]).Append(",").Append(csv[4]);   // Position
					foreach (PlayerData playerData in gameLogic.m_ConnectedPlayers)
					{
						SendMessageToClient(sendMsg.ToString(), playerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
					}

                    int MovedPlayerSeed = int.Parse(csv[1]);
                    PlayerData foundPlayerData = gameLogic.Search(MovedPlayerSeed);
                    foundPlayerData.SetData(csv[2], csv[3], csv[4]);
				}
                break;
            /*
            case ClientToServerSignifiers.PTC_PLAYER_LEFT:
                {

                }
                break;
            */
		}
    }
    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

	#endregion

	#region Connection Events
	static public void ConnectionEvent(int clientConnectionID)
	{
		if (DEBUG) Debug.Log("Client connection, ID == " + clientConnectionID);
		StringBuilder sendMsg = new StringBuilder();

		#region newJoinedPlayer
		PlayerData newJoinedPlayerData = gameLogic.Add(clientConnectionID);

		sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER)
               .Append(",")
               .Append(newJoinedPlayerData.m_Seed);
		SendMessageToClient(sendMsg.ToString(), newJoinedPlayerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);

        sendMsg.Clear(); sendMsg.Length = 0;
		sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST)
               .Append(",");
        foreach (PlayerData playerData in gameLogic.m_ConnectedPlayers) // @playerSeed:position => @p1:px^py^pz@p2:px^py^pz
		{
            if (playerData == newJoinedPlayerData)
                continue;

            sendMsg.Append("@")
                   .Append(playerData.m_Seed)
                   .Append(":")
                   .Append(playerData.m_Position.x).Append("^").Append(playerData.m_Position.y).Append("^").Append(playerData.m_Position.z);
		}
        if (gameLogic.m_ConnectedPlayers.Count > 1)
		    SendMessageToClient(sendMsg.ToString(), newJoinedPlayerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
		#endregion

		#region otherPlayers
		sendMsg.Clear(); sendMsg.Length = 0;
        sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_OTHER_PLAYERS)     // Protocal,PlayerSeed:position
               .Append(",")
               .Append(newJoinedPlayerData.m_Seed)
               .Append(":")
               .Append(newJoinedPlayerData.m_Position.x).Append("^").Append(newJoinedPlayerData.m_Position.y).Append("^").Append(newJoinedPlayerData.m_Position.z);
		foreach (PlayerData playerData in gameLogic.m_ConnectedPlayers)
		{
            if (playerData == newJoinedPlayerData)
                continue;
			
            SendMessageToClient(sendMsg.ToString(), playerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
		}
        #endregion
    }
	static public void DisconnectionEvent(int clientConnectionID)
    {
		if (DEBUG) Debug.Log("Client disconnection, ID == " + clientConnectionID);
		PlayerData leftPlayerData = gameLogic.Remove(clientConnectionID);

        StringBuilder sendMsg = new StringBuilder();
        sendMsg.Append(ServerToClientSignifiers.PTS_PLAYER_LEFT)
               .Append(",")
               .Append(leftPlayerData.m_Seed);
        foreach(PlayerData playerData in gameLogic.m_ConnectedPlayers)
        {
			SendMessageToClient(sendMsg.ToString(), playerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
		}
	}
    #endregion

    #region Setup
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    static public void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    static public NetworkServer GetNetworkServer()
    {
        return networkServer;
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

