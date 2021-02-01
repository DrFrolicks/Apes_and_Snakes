using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 
public class PlayerAppearance : MonoBehaviourPun
{
    public TextMeshPro nameTag;
    // Start is called before the first frame update
    void Start()
    {
        nameTag.text = photonView.Owner.NickName; 
    }

}
