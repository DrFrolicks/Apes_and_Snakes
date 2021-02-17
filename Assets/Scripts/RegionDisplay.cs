using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro; 
public class RegionDisplay : MonoBehaviourPunCallbacks
{
    TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>(); 
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        text.text ="Region: "+ PhotonNetwork.CloudRegion; 
    }
}
