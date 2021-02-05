using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using UnityEngine.Events; 
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager inst;
   
    public Transform spawnPos; 

    //implementation
    private Player[] players;
    private List<Player> leaderboard;

    public UnityEvent OnLeaderboardChange = new UnityEvent();
    public IntEvent OnRichestChange = new IntEvent(); 
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

    public int RichestPlayerNum
    {
        get
        {

            return (int)PhotonNetwork.CurrentRoom.CustomProperties["richest"];
        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "richest", value},
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
            { "rally_rate", 0.2f},
            { "richest", -1 }
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
        }else
        {
            StartCoroutine(InitializeOtherGM());
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", spawnPos.position, Quaternion.identity);
        players = PhotonNetwork.PlayerList;
    }


    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("worth"))
        {
            //print(targetPlayer.NickName + " now has $" + (float)changedProps["worth"]);

            if(PhotonNetwork.IsMasterClient)
                UpdateLeaderboard(); 

        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey("richest"))
            OnRichestChange.Invoke((int)propertiesThatChanged["richest"]); 
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        players = PhotonNetwork.PlayerList; 
    }

    public override void OnPlayerEnteredRoom(Player player)
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

    void UpdateLeaderboard()
    {
        if (players == null)
            return; 

        leaderboard = players.ToList();
        leaderboard.Sort(ComparePlayerWorth);

        string lbString = "";
        foreach (Player p in leaderboard)
        {
            string money = ((float)p.CustomProperties["worth"]).ToMoney(); 
            lbString += $"{p.NickName} {money}\n";
        }

        OnLeaderboardChange.Invoke();
        //leaderBoardDisplay.text = lbString;

        //get richest 
        int richestIndex = 0;
        while (!leaderboard[richestIndex].CustomProperties.ContainsKey("invested") ||
            !(bool)leaderboard[richestIndex].CustomProperties["invested"])
        {

            richestIndex++;
            if (richestIndex >= leaderboard.Count)
            {
                RichestPlayerNum = -1;
                return;
            }
                
        }
        RichestPlayerNum = leaderboard[richestIndex].ActorNumber; 
    }

    int ComparePlayerWorth(Player p1, Player p2)
    {
        return (int)( ((float)p2.CustomProperties["worth"] - (float)p1.CustomProperties["worth"])  * 100);
    }

    #endregion

    #region Coroutines

    IEnumerator InitializeOtherGM()
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("richest"))
        {
            yield return new WaitForEndOfFrame();
        }
        OnRichestChange.Invoke(RichestPlayerNum);
    }
    #endregion


}
