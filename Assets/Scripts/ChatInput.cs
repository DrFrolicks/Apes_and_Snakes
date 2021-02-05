using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInput : MonoBehaviour
{
    public void SayLocalPlayerRPC(string str)
    {
        Hand.localInstance.GetComponent<ChatDisplay>().SayRPC(str); 
    }
}
