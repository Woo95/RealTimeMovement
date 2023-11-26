using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

static public class NetworkServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        switch (signifier)
        {
            /*
            case ClientToServerSignifiers.PTC_CONNECTED_PLAYER:
                {

                }
                break;
            */
            case ClientToServerSignifiers.PTC_PLAYER_MOVE:
                {
                    StringBuilder sendMsg = new StringBuilder();
                    sendMsg.Append(ServerToClientSignifiers.PTS_PLAYER_MOVE); 
                    sendMsg.Append(",");
                    sendMsg.Append(csv[1]);   // Seed
					sendMsg.Append(",");
                    sendMsg.Append(csv[2]);   // Position
                    sendMsg.Append(",");
					sendMsg.Append(csv[3]);   // Velocity
					foreach (PlayerData playerData in gameLogic.m_ConnectedPlayers)
					{
						SendMessageToClient(sendMsg.ToString(), playerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
					}
				}
                break;
		}
        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

	#endregion

	#region Connection Events

	static public void ConnectionEvent(int clientConnectionID)
	{
		Debug.Log("Client connection, ID == " + clientConnectionID);
		StringBuilder sendMsg = new StringBuilder();

		#region newJoinedPlayer
		PlayerData newJoinedPlayerData = gameLogic.Add(clientConnectionID);

		sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER)
               .Append(",")
               .Append(newJoinedPlayerData.m_Seed);
		SendMessageToClient(sendMsg.ToString(), newJoinedPlayerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);

		sendMsg.Clear(); sendMsg.Length = 0;
		sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER_SEND_LIST)
               .Append(",")
               .Append(gameLogic.m_ConnectedPlayers.Count)
               .Append(",");
        foreach (PlayerData playerData in gameLogic.m_ConnectedPlayers) // @playerSeed:position:velocity => @p1:px^py^pz:px^py^pz
        {
            sendMsg.Append("@")
                   .Append(playerData.m_Seed)
                   .Append(":")
                   .Append(playerData.m_Position.x).Append("^").Append(playerData.m_Position.y).Append("^").Append(playerData.m_Position.z)
                   .Append(":")
                   .Append(playerData.m_Velocity.x).Append("^").Append(playerData.m_Velocity.y).Append("^").Append(playerData.m_Velocity.z);
		}
		SendMessageToClient(sendMsg.ToString(), newJoinedPlayerData.m_ClientConnectionID, TransportPipeline.ReliableAndInOrder);
		#endregion

		#region otherPlayers
		sendMsg.Clear(); sendMsg.Length = 0;
        sendMsg.Append(ServerToClientSignifiers.PTS_CONNECTED_OTHER_PLAYERS)
               .Append(",")
               .Append(newJoinedPlayerData.m_Seed)
               .Append(":")
               .Append(newJoinedPlayerData.m_Position.x).Append("^").Append(newJoinedPlayerData.m_Position.y).Append("^").Append(newJoinedPlayerData.m_Position.z)
               .Append(":")
               .Append(newJoinedPlayerData.m_Velocity.x).Append("^").Append(newJoinedPlayerData.m_Velocity.y).Append("^").Append(newJoinedPlayerData.m_Velocity.z);
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
        Debug.Log("Client disconnection, ID == " + clientConnectionID);
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
	public const int PTC_CONNECTED_NEW_PLAYER           = 1;
	public const int PTC_CONNECTED_NEW_PLAYER_SEND_LIST = 2;
	public const int PTC_CONNECTED_OTHER_PLAYERS        = 3;
	public const int PTC_PLAYER_MOVE                    = 4;
}

static public class ServerToClientSignifiers
{
	public const int PTS_CONNECTED_NEW_PLAYER           = 1;
	public const int PTS_CONNECTED_NEW_PLAYER_SEND_LIST = 2;
	public const int PTS_CONNECTED_OTHER_PLAYERS        = 3;
	public const int PTS_PLAYER_MOVE                    = 4;
}
#endregion

