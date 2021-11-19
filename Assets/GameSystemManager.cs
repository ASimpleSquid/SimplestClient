using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject LogIn, Username, Password, Generate, Join, UserID, PassID, Info, GameRoom, Message, Send, Dropdown, Chatbox, SendPrefix, Observe, Replay, ddlPlayer;
    //,btnPlay
    public GameObject networkedClient;
    List<string> preFixMsg = new List<string> { "hello", "test", "bye", "call you later" };
    //static GameObject instance;
    // Start is called before the first frame update
    void Start()
    {
        //instance = this.gameObject;
        GameObject[] allobjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allobjects)
        {
            if (go.name == "Join")
            {
                Join = go;
            }
            else if (go.name == "Info")
            {
                Info = go;
            }
            else if (go.name == "LogIn")
            {
                LogIn = go;
            }
            else if (go.name == "Username")
            {
                Username = go;
            }
            else if (go.name == "Password")
            {
                Password = go;
            }
            else if (go.name == "Generate")
            {
                Generate = go;
            }
            else if (go.name == "UserID")
            {
                UserID = go;
            }
            else if (go.name == "PassID")
            {
                PassID = go;
            }
            else if (go.name == "Replay")
            {
                Replay = go;
            }
            //else if (go.name == "gameBoard")
            //    gameBoard = go;
            else if (go.name == "Message")
                Message = go;
            else if (go.name == "Send")
                Send = go;
            else if (go.name == "Dropdown")
                Dropdown = go;
            else if (go.name == "Chatbox")
                Chatbox = go;
            else if (go.name == "SendPrefix")
                SendPrefix = go;
            else if (go.name == "Observe")
                Observe = go;
            else if (go.name == "ddlPlayer")
            {
                ddlPlayer = go;
            }



        }
        LogIn.GetComponent<Button>().onClick.AddListener(LogInPressed);
        Join.GetComponent<Button>().onClick.AddListener(JoinPressed);
        Observe.GetComponent<Button>().onClick.AddListener(ObserveButtonPressed);
        Replay.GetComponent<Button>().onClick.AddListener(ReplayButtonPressed);
        Send.GetComponent<Button>().onClick.AddListener(SendButtonPressed);
        SendPrefix.GetComponent<Button>().onClick.AddListener(SendPrefButtonPressed);
        Generate.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

        ChangeState(GameStates.LoginMenu);

        Dropdown.GetComponent<Dropdown>().AddOptions(preFixMsg);


    }
    public void updateChat(string msg)
    {
        Chatbox.GetComponent<Text>().text += msg + "\n";
    }
    public void updateUserName(string name)
    {
        //Info.GetComponent<Text>().text = name;
    }
    public void LoadPlayer()
    {
        //ddlPlayer.GetComponent<Dropdown>().
    }
    public void ReplayButtonPressed()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SendPrefButtonPressed()
    {
        string msg = ClientToServerSignifiers.SendPrefixMsg + "," + Dropdown.GetComponent<Dropdown>().options[Dropdown.GetComponent<Dropdown>().value].text.ToString();
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("send" + msg);
    }
    public void SendButtonPressed()
    {
        string msg = ClientToServerSignifiers.SendMsg + "," + Message.GetComponent<InputField>().text;
        Debug.Log("msg:" + msg);
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("send" + msg);
    }
    public void LogInPressed()
    {
        Debug.Log("button");
        string p = Password.GetComponent<InputField>().text;
        string n = Username.GetComponent<InputField>().text;
        string msg;
        if (Generate.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.Login + "," + n + "," + p;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
    }
    public void JoinPressed()
    {
        string msg = ClientToServerSignifiers.JoinGammeRoomQueue + "";
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        ChangeState(GameStates.TicTacToe);
    }
    public void ObserveButtonPressed()
    {
        string msg = ClientToServerSignifiers.JoinAsObserver + "";
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        ChangeState(GameStates.Observer);
    }
    public void CreateToggleChanged(bool newValue)
    {

    }
    public void PlayButtonPressed()
    {
        string msg = ClientToServerSignifiers.PlayGame + "";
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        //load tictactoe
        ChangeState(GameStates.Running);
    }
    public void ChangeState(int newState)
    {
        
        Join.SetActive(false);
        Generate.SetActive(false);
        Password.SetActive(false);
        Username.SetActive(false);
        UserID.SetActive(false);
        PassID.SetActive(false);
        //btnPlay.SetActive(false);
        //txtMsg, btnSend, ddlMsg, chatBox, btnSendPrefixMsg
        Message.SetActive(false);
        Send.SetActive(false);
        Dropdown.SetActive(false);
        Chatbox.SetActive(false);
        SendPrefix.SetActive(false);
        Observe.SetActive(false);
        if (newState == GameStates.LoginMenu)
        {
            LogIn.SetActive(true);
            Generate.SetActive(true);
            Password.SetActive(true);
            Username.SetActive(true);
            UserID.SetActive(true);
            PassID.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            Join.SetActive(true);


            Message.SetActive(true);
            Send.SetActive(true);
            Dropdown.SetActive(true);
            Chatbox.SetActive(true);
            SendPrefix.SetActive(true);
            Observe.SetActive(true);
        }
        else if (newState == GameStates.WaitingInQueue)
        {
        }
        else if (newState == GameStates.WaitingForPlayer)
        {
            Info.GetComponent<Text>().text = "waiting for player";
        }
        else if (newState == GameStates.TicTacToe)
        {
            //btnPlay.SetActive(true);
            Message.SetActive(true);
            Send.SetActive(true);
            Dropdown.SetActive(true);
            Chatbox.SetActive(true);
            SendPrefix.SetActive(true);
        }
        else if (newState == GameStates.Observer)
        {
            Chatbox.SetActive(true);
        }
    }

}

static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueue = 3;
    public const int TicTacToe = 4;
    public const int WaitingForPlayer = 5;
    public const int Running = 6;
    public const int Observer = 7;
}