using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using TMPro; 
public class Launcher : MonoBehaviourPunCallbacks
{
    public UnityEvent OnConnectedSuccess = new UnityEvent();
    public TMP_InputField roomCode;

    public TextAsset filterWords;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit(); 
        }
    }
    #region  Photon PUN Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        OnConnectedSuccess.Invoke();
       
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 6; 

        int roomID = (int)(Random.value * 1000); 
        PhotonNetwork.CreateRoom($"{roomID}",roomOptions);
    }
    
    public override void OnJoinedRoom()
    {
        print($"My current room name is {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Failed to join room, joining random room instead");
        roomCode.text = "";
        PhotonNetwork.JoinRandomRoom(); 
        
    }


    public override void OnCreatedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //todo show this error
        print("Create room failed. " + message); 
    }

    /// <summary>
    /// currently prints rooms
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("EXISTING ROOMS");
        foreach (RoomInfo roomInfo in roomList)
        {
            print(roomInfo.Name);
        }
        print("END ROOM LIST");
        
    }

    #endregion

    #region Public Methods 


    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this 
    /// instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRoom()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            if(roomCode.text == "")
            {
                PhotonNetwork.JoinRandomRoom();
            } else
            {
                PhotonNetwork.JoinRoom(roomCode.text); 
            }
        }
    }

    public void SetLocalPlayerDisplayName(String name)
    {
        if (filterWords.text.Contains(name))
        {
            name = "***";
        }

        PhotonNetwork.NickName = name; 
        PlayerPrefs.SetString("Name", name);
    }
    #endregion
}
