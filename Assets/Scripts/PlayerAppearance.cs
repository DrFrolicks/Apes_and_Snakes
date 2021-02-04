using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 
public class PlayerAppearance : MonoBehaviourPun
{
    public TextMeshPro nameTag;
    public TextMeshPro movementDisplay; 

    public GameObject sprites;

    float flashSeconds;
    Hand hand; 

    // Start is called before the first frame update
    void Start()
    {
        nameTag.text = photonView.Owner.NickName;
        hand = GetComponent<Hand>();
        hand.OnMovement.AddListener(CallFlash);
        hand.OnMovement.AddListener(ShowMovement);
        hand.OnInvestedChange.AddListener(RespondToInvestedChange); 
        flashSeconds = hand.invulnerableTime;

        if (!photonView.IsMine)
            SetOpacity(0.5f);


        //default to uninvested appearance
        RespondToInvestedChange(false); 
        
        
    }

    void ShowMovement(bool dip) 
    {
        movementDisplay.enabled = true;
        float percent = 0;
        char symbol = '+'; 
        if (dip)
        {
            percent = GameManager.inst.DipRate;
            symbol = '-';
            movementDisplay.color = Color.red; 
        } else
        {
            percent = GameManager.inst.RallyRate; 
            movementDisplay.color = Color.green; 
        }
        percent = Mathf.RoundToInt(percent * 100);
        movementDisplay.text = symbol + percent.ToString();
        Invoke("TurnOffMovementDisplay", hand.invulnerableTime); 
    }

    void SetOpacity(float opacity)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer r in renderers)
        {
            Color c = r.color;
            c.a = opacity;
            r.color = c; 
        }
    }

    void RespondToInvestedChange(bool invested)
    {
        print("appearance received invested as " + invested);
        sprites.SetActive(invested);
        nameTag.enabled = invested; 
    }
    void TurnOffMovementDisplay()
    {
        movementDisplay.enabled = false; 
    }

    void CallFlash(bool _dip)
    {
        StartCoroutine(Flash(flashSeconds)); 
    }

    IEnumerator Flash(float seconds)
    {
        //print("started"); 
        float startTime = Time.time; 
        
        while (Time.time < startTime + seconds)
        {
            sprites.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            sprites.SetActive(false); 
            yield return new WaitForSeconds(0.05f);
        }
        if (hand.Invested)
            sprites.SetActive(true);
        else
            sprites.SetActive(false); 
        
    }

}
