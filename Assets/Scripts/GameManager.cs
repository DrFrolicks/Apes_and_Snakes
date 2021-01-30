using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class GameManager : MonoBehaviourPunCallbacks
{
    
    public float SnakeStartingPriceVariability;

    public float BiddingTime; 
    public enum SNAKE_STATES {INFO, BUY, SEll};

    //implementation
    private float currentTimeBid; 
    public void InitializeRoomProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "base_banana_value", 8f},

            { "snake_state", 0}, //0 info , 1 buy, 2 sell
            { "snake_in_progress", false},
            { "snake_bidding", false},
            { "snake_starting_price", 0}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }


    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            InitializeRoomProps(); 
        }

        PhotonNetwork.LocalPlayer.TagObject = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity); 
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            ///TODO multiple snakes started 
            if (!(bool)PhotonNetwork.CurrentRoom.CustomProperties["snake_in_progress"])
                StartNewSnakeRPC();

            if (currentTimeBid > BiddingTime)
                EndSnakeBidding(); 
        }


        if (GetSnakeBidding())
        {
            currentTimeBid += Time.deltaTime;
        }
    }

    public void EndSnakeBidding()
    {
        return; 
    }

    public void StartNewSnakeRPC()
    {
        float baseBanVal = (float)PhotonNetwork.CurrentRoom.CustomProperties["base_banana_value"];
        float startingPrice = baseBanVal + Random.Range(SnakeStartingPriceVariability * -1, SnakeStartingPriceVariability);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "snake_state", 1}, //0 info , 1 buy, 2 sell
            { "snake_in_progress", true},
            { "snake_bidding", true},
            { "snake_starting_price", startingPrice}
        };
        print("bid open"); 
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        return; 
    }

    private bool GetSnakeBidding()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("snake_bidding"))
            return (bool)PhotonNetwork.CurrentRoom.CustomProperties["snake_bidding"];
        else
            return false;
    }

}
