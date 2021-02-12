using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//todo scrapaped delete later
public class ToggleOnInvested : MonoBehaviour
{
    public bool enabledWhenInvested; 
    private void Start()
    {
        Hand.CallOnLocalPlayerSet(AddPlayerListeners);
    }

    void AddPlayerListeners(GameObject go)
    {
        Hand h = go.GetComponent<Hand>();
    }
}
