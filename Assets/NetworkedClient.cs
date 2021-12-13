using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    GameObject gameSystemManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allobjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allobjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
            {
                gameSystemManager = go;
            }
        }
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.0.32", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        //chk message 
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            Debug.Log("Login successful");
            gameSystemManager.GetComponent<GameSystemManager>().updateUserName(csv[1]);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.LoginFailed)
            Debug.Log("Login Failed");
        else if (signifier == ServerToClientSignifiers.AccountCreationComplete)
        {
            Debug.Log("account creation successful");
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.AccountCreationFailed)
            Debug.Log("Account creation failed");
        else if (signifier == ServerToClientSignifiers.OpponentPlay)
        {
            //waiting for other player
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
        //else if (signifier == ServerToClientSignifiers.JoinedPlayAsOpponent)
        //{
        //    //waiting for other player
        //    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.);
        //}
        else if (signifier == ServerToClientSignifiers.GameStart)
        {
            Debug.Log("Opponent player: " + csv[1]);
            //showing list of player
            //2 opponent player
            List<string> otherPlayerList = new List<string>();
            if (csv.Length > 2)
            {
                otherPlayerList.Add(csv[1]);
                otherPlayerList.Add(csv[2]);
            }
            //gameSystemManager.GetComponent<GameSystemManager>().LoadPlayer(otherPlayerList);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
        else if (signifier == ServerToClientSignifiers.ReceiveMsg)
        {
            Debug.Log("rece" + csv[1]);
            gameSystemManager.GetComponent<GameSystemManager>().updateChat(csv[1]);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
        else if (signifier == ServerToClientSignifiers.EnterObserver)
        {
            gameSystemManager.GetComponent<GameSystemManager>().updateChat("An Observer has joined" + csv[1]);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}
public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;
    public const int Login = 2;
    public const int JoinGammeRoomQueue = 3;
    public const int PlayGame = 4;
    public const int SendMsg = 5;
    public const int SendPrefixMsg = 6;
    public const int JoinAsObserver = 7;
}
public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;
    public const int OpponentPlay = 5;
    public const int GameStart = 6;
    public const int ReceiveMsg = 7;
    public const int EnterObserver = 8;

}
