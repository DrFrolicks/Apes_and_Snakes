using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class ChatInput : MonoBehaviour
{
    public ChatDisplay chatDisplay; 
    public void SayLocalPlayerRPC(string str)
    {
        if (str == "")
            return;  

        GetComponent<TMP_InputField>().text = "";
        //Hand.localInstance.GetComponent<ChatDisplay>().SayRPC(str); 
        chatDisplay.SayRPC(str);
    }
}
