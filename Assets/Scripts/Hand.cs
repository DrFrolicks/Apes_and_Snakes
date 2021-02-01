using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events; 

public class Hand : MonoBehaviourPunCallbacks
{

    public static Hand localInstance;
    public float workPayout; 
    
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    public float Worth
    {
        get
        {
            return (float)photonView.Owner.CustomProperties["worth"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "worth", value},
            }; 
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }


    public void InitializePlayerProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "cash", 0f },
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


    public void WorkRPC()
    {
        Worth = (Worth + workPayout); 
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
