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
    public Transform diamondSprite;
    public GameObject sprites;

    public float minDiamondScale, maxDiamondScale;
    public float maxScaleWorth; 

    public BoolEvent OnInvestChange = new BoolEvent(); 
    float flashSeconds;
    Hand hand;

    private void Awake()
    {
        hand = GetComponent<Hand>();
        hand.OnMovement.AddListener(CallFlash);
        hand.OnMovement.AddListener(ShowMovement);
        hand.OnInvestedChange.AddListener(RespondToInvestedChange);
        hand.OnWorthChange.AddListener(RespondToWorthChange); 

        
    }
    // Start is called before the first frame update
    void Start()
    {
        nameTag.text = photonView.Owner.NickName + " $1";
        
        flashSeconds = hand.invulnerableTime;

        if (!photonView.IsMine)
        {
            print("nickname is " + photonView.Owner.NickName); 
            SetOpacity(0.5f);
        }
            
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
        movementDisplay.text = symbol + percent.ToString() + "%";
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
    
    void RespondToWorthChange(float worth)
    {
        //enlarge diamonds
        diamondSprite.localScale = Vector3.one * Mathf.Lerp(minDiamondScale, maxDiamondScale, worth / maxScaleWorth);

        //update worth display
        string newString = nameTag.text.Substring(0, nameTag.text.IndexOf("$"));
        newString += worth.ToMoney(false);
        nameTag.text = newString;
    }

    void RespondToInvestedChange(bool invested)
    {
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
