using CCGKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyStarterPackWindow : MonoBehaviour, IBuyHandler
{
    public TranslatorScript cost;
    public OpenBackpackWindow openBackpackWindow;
    public AudioClip openSound;

    private Model model;

    public void OpenWindow()
    {
        model = GameManager.Instance.model;
        UpdateValues();
        gameObject.SetActive(true);
        GameManager.Instance.PlaySound(null, openSound);
    }

    private void UpdateValues()
    { cost.SetValue(model.rules.goods[Model.GOOD_STARTER_PACK].cost); }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    public void Buy()
    { Process("starter_pack"); }

    public void Process(string id)
    {
       // GameManager.Instance.myIAPManager.Process(id, this);
    }

    public void Complete(string id)
    {
        Debug.Log(id);
        Debug.Log("Buy complete");
        Debug.Log(model.starterPackResult);
        openBackpackWindow.StarterPack();
        CloseWindow();
    }
}