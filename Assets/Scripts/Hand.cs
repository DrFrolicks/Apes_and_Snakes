using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events; 

public class Hand : MonoBehaviourPunCallbacks
{

    public static Hand localInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    public float invulnerableTime;

    public UnityEvent OnHit = new UnityEvent(); 

    float lastTimeHit = 0;

    public FloatEvent OnWorthChange = new FloatEvent();
    public BoolEvent OnInvestedChange = new BoolEvent(); 
    
    #region Photon Custom Properties Properties
    public float Worth
    {
        get
        {
            return (float)photonView.Owner.CustomProperties["worth"];
        }
        set
        {
            OnWorthChange.Invoke(Worth);  
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "worth", value},
            }; 
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public bool Invested
    {
        get
        {
            return (bool)photonView.Owner.CustomProperties["invested"];
        }
        set
        {
            if (value == Invested)
                return;

            OnInvestedChange.Invoke(value); 
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "invested", value},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
    #endregion


    public void InitializePlayerProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "worth", 7.25f },
            { "invested", false }
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

    #region Unity Callbacks
    public void OnTriggerEnter2D(Collider2D collision)
    {
       if (!photonView.IsMine)
            return;


       if(collision.CompareTag("Bullet") && Time.time > lastTimeHit + invulnerableTime)
        {
            ApplyDipRPC(); 
        }
    }
    #endregion

    #region RPC

    public void InvestRPC()
    {
        if(Invested)
        {
            print("Error: Already invested"); 
        }

        Invested = true; 
    }
    public void ApplyDipRPC()
    {
        photonView.RPC("RPC_ApplyDip", RpcTarget.All); 
    }

    [PunRPC] 
    void RPC_ApplyDip()
    {
       if(photonView.IsMine)
        {
            Worth = Worth - (Worth * GameManager.inst.DipRate);
            lastTimeHit = Time.time; 
        }
        
        OnHit.Invoke(); 
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
