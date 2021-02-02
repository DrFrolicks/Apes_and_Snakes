using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack; 

public class ActionButton : MonoBehaviour
{
    
    ButtonManager bm; 
    // Start is called before the first frame update
    void Start()
    {
        bm = GetComponent<ButtonManager>();
        bm.clickEvent.AddListener(OnButtonPress);
        Hand.CallOnLocalPlayerSet(AddPlayerListeners); 
    }
    void AddPlayerListeners(GameObject go)
    {
        Hand h = go.GetComponent<Hand>(); 
        h.OnInvestedChange.AddListener(RespondToInvested);
        h.OnWorthChange.AddListener(RespondToWorthChange);
    }
    void OnButtonPress()
    {
        if(!Hand.localInstance.Invested)
        {
            print("trie"); 
            Hand.localInstance.InvestRPC(); 
        } else
        {
            return;
        }
    }
    
    public void RespondToWorthChange(float worth)
    {
        print("trying to respond to worth"); 
        string newString = bm.buttonText.Substring(0, bm.buttonText.IndexOf("$"));
        newString += Hand.localInstance.Worth.ToMoney();
        bm.SetButtonText(newString); 
    } 

    public void RespondToInvested(bool invested)
    {
        print("trying to resond to invested"); 
        if (!invested)
        {
            bm.SetButtonText($"YOLO - {Hand.localInstance.Worth.ToMoney()}");
            bm.startColor = Color.green; 
        } else
        {
            print("tried to pull out ");
            bm.SetButtonText($"PULL OUT - {Hand.localInstance.Worth.ToMoney()}"); 
            bm.startColor = Color.red; 
        }
    } 
}
