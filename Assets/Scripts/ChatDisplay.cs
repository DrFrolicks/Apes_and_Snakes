using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun; 
public class ChatDisplay : MonoBehaviourPun
{
    const float duration = 2;
    public TextMeshPro textMesh;
    float lastSpeakTime = 0; 
   
    
    public void SayRPC(string str)
    {
        if(GetComponent<Hand>().Invested)
            photonView.RPC("RPC_Say", RpcTarget.All, str); 
    }

    [PunRPC]
    void RPC_Say(string str)
    {
        textMesh.text = str;
        lastSpeakTime = Time.time;
    }

    private void Update()
    {
        if (lastSpeakTime < Time.time + duration)
            textMesh.text = ""; 
    }

}
