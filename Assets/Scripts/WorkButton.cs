using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorkButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBidButtonPress);
    }

    void OnBidButtonPress()
    {
        Hand.localInstance.WorkRPC();
    }
}
