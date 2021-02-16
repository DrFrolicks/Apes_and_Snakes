using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class EarningsDisplay : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>(); 
        if(PlayerPrefs.HasKey("Earnings"))
        {
            textMesh.text = PlayerPrefs.GetFloat("Earnings").ToMoney(); 
        } else
        {
            PlayerPrefs.SetFloat("Earnings", 0.0f); 
        }
       
    }

}
