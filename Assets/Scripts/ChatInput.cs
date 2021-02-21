using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class ChatInput : MonoBehaviour
{
    public TextAsset filterWords; 
    
    public ChatDisplay chatDisplay;

    TMP_InputField input;

    private void Start()
    {
        input = GetComponent<TMP_InputField>(); 
    }
    public void SayLocalPlayerRPC(string str)
    {
        if (str != "")
        {
            string[] inputWords = str.Split(); 
            foreach(string word in inputWords)
            {
                if(filterWords.text.Contains(word))
                {
                    str = "***"; 
                }
            }
            chatDisplay.SayRPC(str);
        }


        //Hand.localInstance.GetComponent<ChatDisplay>().SayRPC(str); 
        input.text = "";
    }
}
