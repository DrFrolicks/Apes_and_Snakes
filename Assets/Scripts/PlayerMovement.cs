using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun; 

public class PlayerMovement : MonoBehaviourPun
{
    public float speed;

    Transform leftCorner, rightCorner; 
    Rigidbody2D rb; 
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
            leftCorner = GameObject.Find("LEFT_CORNER_BOUNDARY").transform;
            rightCorner = GameObject.Find("RIGHT_CORNER_BOUNDARY").transform; 
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return; 

        Vector2 input = new Vector2(FloatingJoystick.inst.Horizontal, FloatingJoystick.inst.Vertical);


        Vector2 newPosition = rb.position + input.normalized * speed * Time.deltaTime;

        //clamp position
        rb.MovePosition(new Vector2(Mathf.Clamp(newPosition.x, leftCorner.position.x, rightCorner.position.x),
            Mathf.Clamp(newPosition.y, leftCorner.position.y, rightCorner.position.y))); 
        
    }
}
