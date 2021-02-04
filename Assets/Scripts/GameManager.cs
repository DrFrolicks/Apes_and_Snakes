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
    private List<Player> leaderboard; 

    #region Photon Custom Properties Properties 
    public float DipRate
    {
        get
        {
            
            return (float)PhotonNetwork.CurrentRoom.CustomProperties["dip_rate"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "dip_rate", value},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    public float RallyRate
    {
        get
        {

            return (float)PhotonNetwork.CurrentRoom.CustomProperties["rally_rate"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "rally_rate", value},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    #endregion

    public void InitializeRoomProps()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            {"dip_rate", 0.1f},
            { "rally_rate", 0.2f}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            InitializeRoomProps();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
    }




    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        //if (changedProps.ContainsKey("worth"))
        //{
        //    print(targetPlayer.NickName + " now has $" + (float)changedProps["worth"]);
        //}
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    private void OnPlayerConnected(Player player)
    {
        players = PhotonNetwork.PlayerList; 
    }

    private void OnPlayerDisconnected(Player player)
    {
        players = PhotonNetwork.PlayerList; 
    }
    #endregion 

    #region Important RPC 

    #endregion


    #region Private Methods 

    private bool IsPropertiesInitialized()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("volatility"); 
    }

    Player GetRichestPlayer()
    {
        return leaderboard[0];
    }

    void UpdateLeaderboard()
    {
        leaderboard = players.ToList();
        leaderboard.Sort(ComparePlayerWorth);

        string lbString = "";
        foreach (Player p in leaderboard)
        {
            string money = ((float)p.CustomProperties["worth"]).ToMoney(); 
            lbString += $"{p.NickName} {money}\n";
        }

        //leaderBoardDisplay.text = lbString;
    }

    int ComparePlayerWorth(Player p1, Player p2)
    {
        return (int)( ((float)p2.CustomProperties["worth"] - (float)p1.CustomProperties["worth"])  * 100);
    }

    #endregion

    #region Coroutines
    #endregion


}
