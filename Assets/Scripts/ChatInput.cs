using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class ChatInput : MonoBehaviour
{

    public void SayLocalPlayerRPC(string str)
    {
        GetComponent<TMP_InputField>().text = ""; 
        Hand.localInstance.GetComponent<ChatDisplay>().SayRPC(str); 
    }
}
