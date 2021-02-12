using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using UnityEngine.Events; 

public class ActionButton : MonoBehaviour
{
    public UnityEvent OnInvested, OnSold; 
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
        Hand.localInstance.TransactionRPC(!Hand.localInstance.Invested); 
    }
    
    public void RespondToWorthChange(float worth)
    {
        //print("setting new worth as " + worth); 
        string newString = bm.buttonText.Substring(0, bm.buttonText.IndexOf("$"));
        newString += worth.ToMoney();
        bm.SetButtonText(newString); 
    } 

    public void RespondToInvested(bool invested)
    {
        
        if (!invested)
        {
            bm.SetButtonText($"YOLO - {Hand.localInstance.Worth.ToMoney()}");
            bm.startColor = Color.green;
            OnSold.Invoke(); 
        } else
        {
            bm.SetButtonText($"PULL OUT - {Hand.localInstance.Worth.ToMoney()}"); 
            bm.startColor = Color.red;
            OnInvested.Invoke(); 
        }
    } 
}
