using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DimOnInvest : MonoBehaviour
{
    public float dimedAlpha;
    Image image;
    private void Start()
    {
        image = GetComponent<Image>();
        Hand.CallOnLocalPlayerSet(AddPlayerListeners);
    }

    void AddPlayerListeners(GameObject go)
    {
        Hand h = go.GetComponent<Hand>();
        h.OnInvestedChange.AddListener(SetDim);
    }
    public void SetDim(bool invested)
    {
        float dimAmt = invested ? 0 : dimedAlpha;
        image.CrossFadeAlpha(dimAmt, 0.1f, false);
    }
}