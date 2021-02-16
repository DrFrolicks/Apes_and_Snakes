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
    public float minWorth;  
    public BoolEvent OnMovement = new BoolEvent(); 

    float lastTimeHit = 0;

    public FloatEvent OnWorthChange = new FloatEvent();
    public BoolEvent OnInvestedChange = new BoolEvent();
    public UnityEvent OnDeath = new UnityEvent(); 

    public bool IsInvulnerable
    {
        get
        {
            return Time.time < lastTimeHit + invulnerableTime; 
        }
    }

    PlayerMovement playerMovement; 

    #region Photon Custom Properties Properties
    public float Worth
    {
        get
        {
            if (!photonView.Owner.CustomProperties.ContainsKey("worth"))
            {
                return 1; 
            } else
            {
                return (float)photonView.Owner.CustomProperties["worth"];
            }

        }
        set
        {
            //print("invoking " + value); 
            OnWorthChange.Invoke(value);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "worth", value},
            }; 
            photonView.Owner.SetCustomProperties(props);
        }
    }

    public bool Invested
    {
        get
        {
            if (!photonView.Owner.CustomProperties.ContainsKey("invested"))
            {
                return false; 
            }
            else
            {
                return (bool)photonView.Owner.CustomProperties["invested"];
            }

        }
        set
        {
            OnInvestedChange.Invoke(value); 
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "invested", value},
            };
            photonView.Owner.SetCustomProperties(props);
        }
    }
    #endregion
    
  

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
        {
            Worth = 1;
            Invested = false; 
        } else
        {
            StartCoroutine(InitializeOtherPlayers());
        }


        photonView.Owner.TagObject = this;
        playerMovement = GetComponent<PlayerMovement>(); 
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

        if (!Invested)
            return; 

        if (!photonView.IsMine)
        {
            if (collision.CompareTag("Tendy") && IsInvulnerable)
            {
                Destroy(collision.gameObject);
            }
            return;
        }




       if(collision.CompareTag("Bullet") && IsInvulnerable)
        {
            ApplyMovementRPC(true); 
        } 
       if(collision.CompareTag("Tendy") && IsInvulnerable)
        {
            ApplyMovementRPC(false);
            Destroy(collision.gameObject); 
        } 
    }

    #endregion

    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (!targetPlayer.IsLocal && targetPlayer.ActorNumber == photonView.Owner.ActorNumber)
        {
            if(changedProps.ContainsKey("worth"))
            {
                OnWorthChange.Invoke((float)changedProps["worth"]); 
            }        
            if(changedProps.ContainsKey("invested"))
            {
                OnInvestedChange.Invoke((bool)changedProps["invested"]); 
            }
        }
    }

    #endregion

    #region RPC

    public void TransactionRPC(bool invest)
    {
        if(invest)
        {
            if (Invested)
            {
                print("Error: Already invested");
                return;
            }
            transform.position = GameManager.inst.spawnPos.position;
            
        } else
        {
            if (!Invested)
            {
                print("Error: Already sold");
                return; 
            }
        }

        Invested = invest;
    }


    public void ApplyMovementRPC(bool dip)
    {
        photonView.RPC("RPC_ApplyMovement", RpcTarget.All, dip); 
    }

    [PunRPC] 
    void RPC_ApplyMovement(bool dip)
    {
       if(photonView.IsMine)
        {
            if (dip)
            {
                Worth = Mathf.Clamp(Worth - (Worth * GameManager.inst.DipRate), minWorth, float.MaxValue);
            } else
            {
                Worth = Worth + (Worth * GameManager.inst.RallyRate);
            }
            
            lastTimeHit = Time.time; 
        }

        OnMovement.Invoke(dip); 
    }

    #endregion

    #region Private Methods
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

    IEnumerator InitializeOtherPlayers()
    {
        while(!photonView.Owner.CustomProperties.ContainsKey("invested"))
        {
            yield return new WaitForEndOfFrame(); 
        }
        print("invoke invested with " + Invested); 
        OnInvestedChange.Invoke(Invested);
        OnWorthChange.Invoke(Worth);
    }
    #endregion


}
