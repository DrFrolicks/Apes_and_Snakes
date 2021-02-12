using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun; 
public class ChatDisplay : MonoBehaviourPun
{
    const float duration = 2;
    const string messageFormat = "/"{0}/" - {1}"
    public TextMeshProUGUI textMesh;
    float lastSpeakTime = 0; 
   
    
    public void SayRPC(string str)
    {
        //todo chat filter
        photonView.RPC("RPC_Say", RpcTarget.All, str); 
    }

    [PunRPC]
    void RPC_Say(string str)
    {

        textMesh.text.;
        lastSpeakTime = Time.time;
    }

    private void Update()
    {
        if (lastSpeakTime + duration < Time.time)
            textMesh.text = ""; 
    }

}
