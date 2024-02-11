using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using ServerSide;
#if STEAM
using Steamworks;
using static Hub;

public class TestSteamShop : MonoBehaviour, IPointerClickHandler
{
    public int purchaseID = 0;
    public string discription = "some purchase";
    public Text valueText;

    bool inProcess = false;

    void Start()
    {
        if(hub.steam_prices.Count>=purchaseID - 1)
        if(valueText!=null)
        valueText.text = hub.steam_prices[purchaseID - 1];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inProcess)
            return;

        inProcess = true;    
        StartCoroutine(SendPurchaseRequest());
    }

    IEnumerator SendPurchaseRequest()
    {
        SteamUser.OnMicroTxnAuthorizationResponse += OnPurchaseFinished;
        return ServerData.Instance.SteamPurchase(purchaseID);
    }

    private void OnPurchaseFinished(AppId appid, ulong orderid, bool success)
    {

	    if(success)
	    { StartCoroutine(SendPurchaseCmplRequest(orderid)); }
	    else { }

        inProcess = false;
    }

    IEnumerator SendPurchaseCmplRequest(ulong orderid)
    { return ServerData.Instance.CompliteSteamPurchase(orderid); }
}
#endif