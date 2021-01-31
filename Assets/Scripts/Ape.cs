using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events; 

public class Ape : MonoBehaviourPunCallbacks
{

    public static Ape localInstance;

    public UnityEvent OnBidStart = new UnityEvent();
    public UnityEvent OnBidEnd = new UnityEvent();
    public float workPayout; 
    
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    

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


    private void Awake()
    {
        if (localInstance == null)
        {
            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
                SetLocalPlayer();
            }
        }
        
    }

    private void Start()
    {
        InitializePlayerProps();
        photonView.Owner.TagObject = this; 
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        return; 
    }


    public void BidHarderRPC()
    {
        float newBid = GameManager.inst.GetTopBid() + 1;
        if (GetCash() < newBid)
        {
            print(photonView.Owner.NickName + " insufficient funds at  $" + GetCash());
            return;
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "bid", newBid }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public float GetBid()
    {
        ExitGames.Client.Photon.Hashtable map = photonView.Owner.CustomProperties;
        if(map.ContainsKey("bid"))
        {
            return (float)map["bid"]; 
        } else
        {
            return -1; //not yet intialized 
        }   
    }

    public void WorkRPC()
    {
        SetCashRPC(GetCash() + workPayout); 
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

    #region Helper Methods 

    public float GetCash()
    {
        return (float)photonView.Owner.CustomProperties["cash"]; 
    }

    public void SetCashRPC(float cashVal)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "cash", cashVal}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion
}
