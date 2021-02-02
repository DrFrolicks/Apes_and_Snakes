using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun; 

public class Snake : MonoBehaviourPun
{
    public Transform shotPatterns;
    public float waitSeconds; 
    UbhShotCtrl shotCtrl;

    // Start is called before the first frame update
    void Start()
    {
        shotCtrl = GetComponent<UbhShotCtrl>();
        LoadShotPatterns();

        StartCoroutine(SnakeBehavior()); 
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
    
    public void LoadShotPatterns()
    {
        List<ShotInfo> patterns = new List<ShotInfo>(shotPatterns.childCount + 1); 
        for(int i = 0; i < shotPatterns.childCount; i++)
        {
            patterns.Add(null); 
            patterns[i] = new ShotInfo(shotPatterns.GetChild(i).GetComponent<UbhBaseShot>(), waitSeconds); 
        }
        shotCtrl.m_shotList = patterns; 
    }

    IEnumerator SnakeBehavior()
    {
        while (true)
        {
            ShootRPC(Random.Range(0, shotPatterns.childCount)); 
            yield return new WaitForSeconds(waitSeconds); 
        }
    }
}
