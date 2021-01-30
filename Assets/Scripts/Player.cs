using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events; 

public class Player : MonoBehaviourPunCallbacks
{

    public static Player localInstance;

    public UnityEvent OnBidStart = new UnityEvent();
    public UnityEvent OnBidEnd = new UnityEvent();
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    private void Awake()
    {
        if (localInstance == null)
        {
            if (!PhotonNetwork.IsConnected || photonView.AmOwner)
            {
                SetLocalPlayer();
            }
        }
    }

    public void InitializePlayerProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "cash", 0f },
            { "bananas", 0},
            { "bid", 0f }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props); 
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey("snake_bidding") && (bool)propertiesThatChanged["snake_bidding"])
        {
            if((int)propertiesThatChanged["snake_state"] == (int)GameManager.SNAKE_STATES.BUY &&
                (int)propertiesThatChanged["snake_state"] == (int)GameManager.SNAKE_STATES.SEll)
            {
                OnBidStart.Invoke(); 
            }
        }
    }


    public void BidHarderRPC()
    {
        float newBid = 0;

        if (GetBid() == 0)
            newBid = (float)PhotonNetwork.CurrentRoom.CustomProperties["snake_starting_price"];
        else
            newBid = GetBid() + 1; 

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "bid", newBid }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        print(newBid); 
    }

    private float GetBid()
    {
        return (float)photonView.Owner.CustomProperties["bid"]; 
    }

    #region Unity Event Methods
    /// <summary>
    /// Executes the given function when the player loads, with the player's gameobject as a parameter. 
    /// </summary>
    /// <param name="func"></param>
    public static void CallOnLocalPlayerSet(UnityAction<GameObject> func)
    {
        if (localInstance == null)
        {
            OnLocalPlayerSet.AddListener(func);
        }
        else
        {
            func(localInstance.gameObject);
        }
    }
    #endregion


    #region Custom Methods
    /// <summary>
    /// set itself as LocalPlayerInstance
    /// invokes OnLocalPlayerSet event
    /// </summary>
    void SetLocalPlayer()
    {
        localInstance = this;
        OnLocalPlayerSet.Invoke(gameObject);
        gameObject.name = "Local Player";
    }
    #endregion
}
