using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun; 

public class Snake : MonoBehaviourPun
{
    public float volleyInterval, snakeInterval;
    public int volleysPerSnake;
    public float snakeAttackDelay;
    public float rallySpawnRange;
    
    public Transform shotPatterns;
    public Transform spawnPosition;
    public GameObject RallyPrefab;
    public BoxCollider2D snakeSpawnArea;


    Bounds _snakeSpawnArea; 
    UbhShotCtrl shotCtrl;
    // Start is called before the first frame update
    void Start()
    {
        shotCtrl = GetComponent<UbhShotCtrl>();
        LoadShotPatterns();

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(SnakeBehavior());

        _snakeSpawnArea = snakeSpawnArea.bounds; 
    }

    public void SpawnRPC()
    {
        photonView.RPC("RPC_Spawn", RpcTarget.AllViaServer, RandomPointInBounds(_snakeSpawnArea));
    }

    [PunRPC]
    void RPC_Spawn(Vector2 spawnPosition)
    {
        ToggleChildSprites(true);
        StartCoroutine(MoveTo(snakeAttackDelay / 5f, spawnPosition));
    }


    public void ShootRPC(int patternIndex)
    {
        photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer, patternIndex);
    }

    [PunRPC]
    void RPC_Shoot(int patternIndex)
    {
        shotCtrl.Shoot(patternIndex);
    }


    public void DieRPC()
    {
        photonView.RPC("RPC_Die", RpcTarget.AllViaServer); 
    }

    [PunRPC]
    void RPC_Die()
    {
        ToggleChildSprites(false);
        SpawnRallies();
        transform.position = spawnPosition.position;
    }

    public void LoadShotPatterns()
    {
        List<ShotInfo> patterns = new List<ShotInfo>(shotPatterns.childCount + 1); 
        for(int i = 0; i < shotPatterns.childCount; i++)
        {
            patterns.Add(null); 
            patterns[i] = new ShotInfo(shotPatterns.GetChild(i).GetComponent<UbhBaseShot>(), volleyInterval); 
        }
        shotCtrl.m_shotList = patterns; 
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
        int spawnNum = Mathf.Clamp((int)(PhotonNetwork.CountOfPlayers * 0.75f),1, int.MaxValue); 
        for(int i = 0; i < spawnNum; i++)
        {
            Vector2 spawnPos = transform.position + (Vector3)(Random.insideUnitCircle * rallySpawnRange);
            Instantiate(RallyPrefab, spawnPos, Quaternion.identity);
        }
    }
    public static Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2 (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
    #region Coroutines 
    IEnumerator SnakeBehavior()
    {
        while (true)
        {
            SpawnRPC();
            yield return new WaitForSeconds(snakeAttackDelay);

            for (int i = 0; i < volleysPerSnake; i++)
            {
                ShootRPC(Random.Range(0, shotPatterns.childCount));
                yield return new WaitForSeconds(volleyInterval);
            }

            DieRPC();
            yield return new WaitForSeconds(snakeInterval);
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
