using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events; 

public class Snake : MonoBehaviourPun
{
    public static Snake inst; 

    //behavior parameters
    public float attackChance; //if snake's not attacking its moving
    public float moveTime;
    public float snakeCheckInterval;

    public Transform shotPatterns;
    public Transform spawnPosition;
    public GameObject RallyPrefab;
    public BoxCollider2D snakeSpawnArea;

    public enum SNAKE_STATE {NULL, MOVE, ATTTACK};

    public UnityEvent OnHit = new UnityEvent(); 

    Bounds _snakeSpawnArea; 
    UbhShotCtrl shotCtrl;
    bool shootingMAST;


    int startIndex = 0; 
    #region Photon Custom Properties 
    public int State
    {
        get
        {
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("state"))
            {
                return -1;
            }
            else
            {
                return (int)PhotonNetwork.CurrentRoom.CustomProperties["state"];
            }

        }
        set
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "state", value},
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        inst = this;
        if (PhotonNetwork.IsMasterClient)
            State = (int)SNAKE_STATE.NULL; 

    }
    // Start is called before the first frame update
    void Start()
    {
        shotCtrl = GetComponent<UbhShotCtrl>();
        LoadShotPatterns();

        StartCoroutine(SnakeBehavior());

        _snakeSpawnArea = snakeSpawnArea.bounds;
        shootingMAST = false; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("collided"); 
        if (other.CompareTag("Player"))
        {
            print("touched a player"); 
            OnHit.Invoke(); 
            SpawnRallies();
        }
    }
    #endregion-

    #region RPC
    public void MoveRPC()
    {
        State = (int)SNAKE_STATE.MOVE; 
        photonView.RPC("RPC_Move", RpcTarget.AllViaServer, RandomPointInBounds(_snakeSpawnArea));
    }

    [PunRPC]
    void RPC_Move(Vector2 spawnPosition)
    {
        
        ToggleChildSprites(true);
        StartCoroutine(MoveTo(moveTime, spawnPosition));
    }


    public void ShootRPC(int patternIndex)
    {
        State = (int)SNAKE_STATE.ATTTACK; 
        photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer, patternIndex);
    }

    [PunRPC]
    void RPC_Shoot(int patternIndex)
    {
        shotCtrl.Shoot(patternIndex);
    }


    #endregion

    #region Helper Functions
    public void LoadShotPatterns()
    {
        List<ShotInfo> patterns = new List<ShotInfo>(shotPatterns.childCount + 1); 
        for(int i = 0; i < shotPatterns.childCount; i++)
        {
            patterns.Add(null); 
            patterns[i] = new ShotInfo(shotPatterns.GetChild(i).GetComponent<UbhBaseShot>(), 0f); 
        }
        shotCtrl.m_shotList = patterns; 
    }

    public void OnShotFinished()
    {
        shootingMAST = false; 
    }

    void ToggleChildSprites(bool visible)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(); 
        foreach (SpriteRenderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    void SpawnRallies()
    {
        int spawnNum = Mathf.Clamp((int)(PhotonNetwork.CountOfPlayers * 0.5f),1, int.MaxValue);
        float rotation = 360f / spawnNum;
        Vector3 spawnDirection = Vector3.down;
        
        for (int i = 0; i < spawnNum; i++)
        {
            Vector3 spawnPos = spawnDirection + transform.position; 
            Instantiate(RallyPrefab, spawnPos, Quaternion.identity);
            spawnDirection = Quaternion.Euler(0, 0, rotation) * spawnDirection;
        }
    }

    public static Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2 (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
    #endregion

    #region Coroutines 
    IEnumerator SnakeBehavior()
    {
        while (true)
        {
            if(!PhotonNetwork.IsMasterClient || State == -1)
            {
                shootingMAST = false; 
                yield return new WaitForSeconds(snakeCheckInterval); 
                continue; 
            }

            if (State == (int)SNAKE_STATE.NULL)
            {
                MoveRPC();
                yield return new WaitForSeconds(moveTime * 1.25f);
            }
            else //state must be move or attack 
            {
                int timeWaited = 0; 
                while(shootingMAST)
                {
                    //failsafe
                    timeWaited++;
                    if (timeWaited > 20)
                        break; 

                    yield return new WaitForSeconds(1f); 
                }


                float rng = Random.value; //move die or attack
                
                if (rng > attackChance) //move
                {
                    MoveRPC();
                    yield return new WaitForSeconds(moveTime * 1.25f);
                }
                else  //attack
                {
                    shootingMAST = true;
                    int patIndex = Random.Range(0, shotPatterns.childCount);
                    //print(startIndex + "current shot"); 
                    ShootRPC(patIndex);
                    startIndex++; 
                }
            }
                  
        }
    }

    IEnumerator MoveTo(float smoothTime, Vector2 targetPosition)
    {
        float startTime = Time.time;
        Vector2 velocity = Vector2.zero;
        float lowSmoothTime = smoothTime * 0.7f; 
        while (Time.time < startTime + smoothTime)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, lowSmoothTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

}
