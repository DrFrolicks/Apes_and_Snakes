using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq; 
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager inst;
    public Transform spawnPos; 

    //implementation
    private Player[] players;

    public void InitializeRoomProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "volatility", 1},
            { "gain_frequency", 0 }
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

        PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
    }




    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("worth"))
        {
            print(targetPlayer.NickName + " now has $" + (float)changedProps["worth"]);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }
    #endregion 

    #region Important RPC 

    #endregion


    #region Private Methods 

    private bool IsPropertiesInitialized()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("volatility"); 
    }
    #endregion

    #region Coroutines
    #endregion


}
