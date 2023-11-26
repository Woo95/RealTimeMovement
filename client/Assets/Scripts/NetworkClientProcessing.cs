using System.Text;
using UnityEngine;

static public class NetworkClientProcessing
{
	const bool DEBUG = false;
	#region Send and Receive Data Functions
	static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        if (DEBUG) Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);
        
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
        switch (signifier)
        {
            case ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER:
                {
                    if (DEBUG) Debug.Log("PTS_CONNECTED_NEW_PLAYER");
                    int mySeed = int.Parse(csv[1]);
                    Vector3 position = Vector3.zero;
                    
                    gameLogic.SpawnMySelf(mySeed, position);
                }
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_NEW_PLAYER_RECEIVE_DATA:
                {
                    if (DEBUG) Debug.Log("PTS_CONNECTED_NEW_PLAYER_RECEIVE_LIST"); // @playerSeed:position => @p1:pX^pY^pZ@p2:pX^pY^pZ...
                    string[] allPlayerDatas = csv[1].Split('@');
                    
                    //p1:px^py^pz
                    //p2:px^py^pz
                    //p3:px^py^pz...
                    for (int i=1; i < allPlayerDatas.Length; i++)
                    {
                        string[] eachPlayerData = allPlayerDatas[i].Split(':');
                        
                        int seed = int.Parse(eachPlayerData[0]);
                        Vector3 position = GetVector3Info(eachPlayerData[1], '^');
                        
                        gameLogic.SpawnOthers(seed, position);
                    }
                }
                break;
            case ServerToClientSignifiers.PTS_CONNECTED_PLAYERS_RECEIVE_NEW_PLAYER_DATA:
                {
                    if (DEBUG) Debug.Log("PTS_CONNECTED_PLAYERS_RECEIVE_NEW_PLAYER_DATA");   // recieved newPlayerInfo => PlayerSeed:pX^pY^pZ
                    string[] newJoinedPlayerData = csv[1].Split(':');
                    int seed = int.Parse(newJoinedPlayerData[0]);
                    Vector3 position = GetVector3Info(newJoinedPlayerData[1], '^');
                    
                    gameLogic.SpawnOthers(seed, position);
                }
                break;
                
            case ServerToClientSignifiers.PTS_PLAYER_MOVE:
                {
                    if (DEBUG) Debug.Log("PTS_PLAYER_MOVE");
                    StringBuilder sendMsg = new StringBuilder();
                    int movedPlayerSeed = int.Parse(csv[1]);
                    Vector3 position = GetVector3Info(csv, 2);
                    
                    gameLogic.MovePlayer(movedPlayerSeed, position);
                }
                break;
                
            case ServerToClientSignifiers.PTS_PLAYER_LEFT:
            {
                if (DEBUG) Debug.Log("PTS_PLAYER_LEFT");
                int leftPlayerSeed = int.Parse(csv[1]);
                gameLogic.OtherPlayerLeft(leftPlayerSeed);
            }
            break;
        }
	}

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }
    
    #endregion
    
    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        if (DEBUG) Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        if (DEBUG) Debug.Log("Network Disconnection Event!");
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
    static public Vector3 GetVector3Info(string[] pos, int startIdx = 0)
    {
        Vector3 result = Vector3.zero;
        
        result.Set(float.Parse(pos[startIdx]), float.Parse(pos[startIdx + 1]), float.Parse(pos[startIdx + 2]));
        
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
    public const int PTC_CONNECTED_NEW_PLAYER                       = 1;
    public const int PTC_CONNECTED_NEW_PLAYER_RECEIVE_DATA          = 2;
    public const int PTC_CONNECTED_PLAYERS_RECEIVE_NEW_PLAYER_DATA  = 3;
    public const int PTC_PLAYER_MOVE                                = 4;
    public const int PTC_PLAYER_LEFT                                = 5;
}

static public class ServerToClientSignifiers
{
    public const int PTS_CONNECTED_NEW_PLAYER                       = 1;
    public const int PTS_CONNECTED_NEW_PLAYER_RECEIVE_DATA          = 2;
    public const int PTS_CONNECTED_PLAYERS_RECEIVE_NEW_PLAYER_DATA  = 3;
    public const int PTS_PLAYER_MOVE                                = 4;
    public const int PTS_PLAYER_LEFT                                = 5;
}
#endregion

