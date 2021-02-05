using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; 

public class FollowPlayer : MonoBehaviour
{
    public Hand followPlayer; 
    public float speed;
    Vector3 initPos; 

    private void Start()
    {
        initPos = transform.position;
        GameManager.inst.OnRichestChange.AddListener(UpdateFollowPlayer);

    }

    void UpdateFollowPlayer(int richestNum)
    {
        if(richestNum != -1)
        {
            Player rich = PhotonNetwork.CurrentRoom.GetPlayer(richestNum);
            followPlayer = (Hand)(rich.TagObject);
        } else
        {
            followPlayer = null;
            
        }

    }

    // Update is called once per frame
    void Update()
    {
       if(followPlayer != null)
        {
            Vector3 des = followPlayer.transform.Find("Crown Position").transform.position;
            transform.position = Vector3.Lerp(transform.position, des, speed * Time.deltaTime);
        } else
        {
            transform.position = initPos; 
        }
    }
}
