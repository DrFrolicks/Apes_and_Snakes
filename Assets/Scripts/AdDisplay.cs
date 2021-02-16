using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements; 

public class AdDisplay : MonoBehaviour
{
    public static AdDisplay inst;  

    public string myGameIDAndroid = "4013779";
    public string myGAmeIDIOS = "4013778";
    public string myVideoPlacement = "video";
    public bool adStarted;

    private bool testMode = true;


    private void Awake()
    {
        if (inst != null)
            inst = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS
    Advertisement.Initialize(myGameIDIOS, testMode);
#else
    Advertisement.Initialize(myGameIDAndroid, testMode); 
#endif
       
    }

    public void PlayAd()
    {
        if (Advertisement.isInitialized && Advertisement.IsReady(myVideoPlacement))
        {
            Advertisement.Show(myVideoPlacement);
        }
    }
}
