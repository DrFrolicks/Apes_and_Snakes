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

    public float transactionTime, downTime; 

    //implementation
    private float curPhaseTime; 
    private Player[] players;
    private float snakeCheckInterval = 5f; 

    public float TradeValue
    {
        get
        {
            return (float)PhotonNetwork.CurrentRoom.CustomProperties["trade_value"];
        } 

        set
        {
            curPhaseTime = 0; 

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "trade_value", value}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    public void InitializeRoomProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "base_value", 8f},
            { "trade_value", 0f}
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
        if (IsPropertiesInitialized())
        {
            curPhaseTime += Time.deltaTime;
        }
    }


    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("cash"))
        {
            print(targetPlayer.NickName + " now has $" + (float)changedProps["cash"]);
        }

        if (changedProps.ContainsKey("bananas"))
        {
            print(targetPlayer.NickName + " banana count: " + (int)changedProps["bananas"]);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey("trade_value"))
        {
            if(TradeValue == 0) 
            {
                print("There are no snakes."); 
            } else if (TradeValue < 0)
            {
                print("Selling bananas for $" + TradeValue);
            } else if (TradeValue > 0)
            {
                print("Buying bananas for $" + TradeValue); 
            }
        
        }
    }
    #endregion 

    #region Important RPC 
    public void StartNewSnakeRPC()
    {
        curPhaseTime = 0; //this is set twice to 0, once extra early to avoid next-frame bugs when updates did not occur
        float baseBanVal = (float)PhotonNetwork.CurrentRoom.CustomProperties["base_value"];
        float startingPrice = baseBanVal + Random.Range(SnakeStartingPriceVariability * -1, SnakeStartingPriceVariability);
        
        TradeValue = startingPrice * -1; 
       
    }

    public void StopSnakeRPC()
    {
        curPhaseTime = 0; 
        TradeValue = 0; 
    }
    #endregion


    #region Private Methods 


    public bool GetSnakePresent()
    {
        return TradeValue != 0;
    }

    private bool IsPropertiesInitialized()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("trade_value"); 
    }
    #endregion

    #region Coroutines
    IEnumerator SnakeCheck()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient && IsPropertiesInitialized())
            {
                if (!GetSnakePresent() && curPhaseTime > downTime)
                {
                    StartNewSnakeRPC();
              
                } else if (GetSnakePresent() &&  curPhaseTime > transactionTime)
                {
                    StopSnakeRPC();
                }
            }
            yield return new WaitForSeconds(snakeCheckInterval);
        }
    }
    #endregion


}
