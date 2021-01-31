using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events; 

public class Ape : MonoBehaviourPunCallbacks
{

    public static Ape localInstance;
    public float workPayout; 
    
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    public float Cash
    {
        get
        {
            return (float)photonView.Owner.CustomProperties["cash"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "cash", value }
            }; 
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public int Bananas
    {
        get
        {
            return (int)photonView.Owner.CustomProperties["bananas"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "bananas", value }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }


    public void InitializePlayerProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "cash", 0f },
            { "bananas", 0}
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
        if(photonView.IsMine)
            InitializePlayerProps();

        photonView.Owner.TagObject = this; 
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        return; 
    }

    public void TradeRPC()
    {
        float tradeVal = GameManager.inst.TradeValue; 

        if(tradeVal == 0)
        {
            print("Action failed. No snakes present.");
        } else if(tradeVal < 0) //buy
        {
            print("Trying to buy...");

            if(Cash > Mathf.Abs(tradeVal))
            {
                Bananas += 1;
                Cash += tradeVal;
                print("bought banana"); 
            } else
            {
                print("Action failed. Insufficient funds.");
            }

        } else if (tradeVal > 0) //sell
        {
            return;  //todo
        }
    }


    public void WorkRPC()
    {
        Cash = (Cash + workPayout); 
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
