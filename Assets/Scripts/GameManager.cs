using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq; 
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager inst; 
    
    public float SnakeStartingPriceVariability;

    public float BiddingTime; 
    public enum SNAKE_STATES {INFO, BUY, SEll};

    //implementation
    private float currentTimeBid;
    private Player[] players;
    private float snakeCheckInterval = 5f; 

    public void InitializeRoomProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "base_banana_value", 8f},

            { "starting bid", 0f},

            { "top_bidder_num", -1 }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            InitializeRoomProps(); 
        }

        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        StartCoroutine(SnakeCheck()); 
    }

    private void Update()
    {
        if (GetSnakeBidding())
        {
            currentTimeBid += Time.deltaTime;
        }
    }

    public void EndSnakeBidding()
    {
        return; 
    }

    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("bid") && (float)changedProps["bid"] != 0)
        {
            if(PhotonNetwork.IsMasterClient)
                SetTopBidderRPC(GetHighestBidder());

            print(targetPlayer.NickName + " bids for  $" + (float)changedProps["bid"]);
        }

        if (changedProps.ContainsKey("cash"))
        {
            print(targetPlayer.NickName + " now has $" + (float)changedProps["cash"]);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if(propertiesThatChanged.ContainsKey("top_bidder_num") && (int)propertiesThatChanged["top_bidder_num"] != -1)
        {
            Ape topBidder = (Ape)(GetTopBidder().TagObject);
            print(GetTopBidder().NickName + " is now highest Bidder."); 
        }

        if (propertiesThatChanged.ContainsKey("starting_bid") && (float)propertiesThatChanged["starting_bid"] != 0f)
        {
            print("Bidding opens at $" + (float)propertiesThatChanged["starting_bid"]);
        }
    }
    #endregion 

    #region Important RPC 
    public void StartNewSnakeRPC()
    {
        float baseBanVal = (float)PhotonNetwork.CurrentRoom.CustomProperties["base_banana_value"];
        float startingPrice = baseBanVal + Random.Range(SnakeStartingPriceVariability * -1, SnakeStartingPriceVariability);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "snake_state", 1}, //0  info, 1 buy, 2 sell
            { "snake_in_progress", true},
            { "snake_bidding", true},
            { "snake_starting_price", startingPrice}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        return; 
    }
    #endregion


    #region Private Methods 
    private bool GetSnakeBidding()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("snake_bidding"))
            return (bool)PhotonNetwork.CurrentRoom.CustomProperties["snake_bidding"];
        else
            return false;
    }

    private int GetHighestBidder()
    {
        float[] bids = new float[PhotonNetwork.CountOfPlayers];
        List<Player> players = PhotonNetwork.PlayerList.ToList<Player>();
        players.Sort(comparePlayerBids);
        return players[0].ActorNumber;
    }


    int comparePlayerBids(Player p1, Player p2)
    {

        return (int)(((float)p2.CustomProperties["bid"] - (float)p1.CustomProperties["bid"]) * 100);
    }

    void SetTopBidderRPC(int topBidderActorNum)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "top_bidder_num", topBidderActorNum},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
   
    Player GetTopBidder()
    {
        int topBidNum = (int)PhotonNetwork.CurrentRoom.CustomProperties["top_bidder_num"];
        if (topBidNum == -1)
            return null; 
        else
            return PhotonNetwork.CurrentRoom.GetPlayer(topBidNum); 
    }

    public float GetTopBid()
    {
        Player topBidder = GetTopBidder(); 
        if (topBidder == null)
        {
            return 0; 
        } else
        {
            return (float)topBidder.CustomProperties["bid"];
        }
        
    }

    private bool IsPropertiesInitialized()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("snake_state"); 
    }
    #endregion

    #region Coroutines
    IEnumerator SnakeCheck()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient && IsPropertiesInitialized())
            {
                if (!(bool)PhotonNetwork.CurrentRoom.CustomProperties["snake_in_progress"])
                    StartNewSnakeRPC();

                if (currentTimeBid > BiddingTime)
                    EndSnakeBidding();
            }
            yield return new WaitForSeconds(snakeCheckInterval);
        }
    }
    #endregion


}
