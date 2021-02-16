using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun; 
public class RoomCodeDisplay : MonoBehaviour
{
    TextMeshProUGUI text; 
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = string.Format(text.text, PhotonNetwork.CurrentRoom.Name); 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
