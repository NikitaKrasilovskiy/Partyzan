using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopWindowScript : MonoBehaviour
{
    public GameObject types;
    public List<GameObject> parts;
    
    public CreatureCardViewUI[] cards;

    public BackPuckController openBackpackWindow;

    public GameObject buyComplete;
    public GameObject noMoney;

    public TranslatorScript bonusDescription;
    public TranslatorScript bonusDescription2;

    public BuyStarterPackWindow buyStarterPackWindow;
    public DeckSelectorWindow deckSelectorWindow;
    public TasksGeneralWindow tasksGeneralWindow;
    public CardsUpgrade cardsUpgrade;
    public HomeScene homeScene;

    Model model;

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        Back();
        model = GameManager.Instance.model;
        updateData();
    }

    private void updateData()
    {
        Rules rules = model.rules;
        bonusDescription.SetValue(rules.commonParams.premiumCardBonusFarm);
        bonusDescription.SetValue2(rules.commonParams.premiumCardBonusExp);
        bonusDescription.UpdateText();
        bonusDescription2.SetValue(rules.commonParams.premiumCardBonusFarm);
        bonusDescription2.SetValue2(rules.commonParams.premiumCardBonusExp);
        bonusDescription2.UpdateText();
    }

    public void CloseWindow()
    {
        if (types.activeInHierarchy)
            gameObject.SetActive(false);
        else if ((parts[6].activeInHierarchy) || (parts[7].activeInHierarchy))
            SelectType(1);
        else
            Back();
    }

    public void SelectType(int n)
    {
        if (n==4)
        {
            buyStarterPackWindow.OpenWindow();
            return;
        }
        if (n==5)
        { tasksGeneralWindow.OpenWindow(); }
        for (int i=0;i<parts.Count;i++)
        { if (parts[i]!=null) parts[i].SetActive(i==n); }
        types.SetActive(n>=parts.Count);
        if (n==6)
        {
            cards[0].updateCard(model.rules.cardTypes[0], 2, 0);
            cards[1].updateCard(model.rules.cardTypes[0], 3, 0);
        }
        else if (n==7)
        {
            cards[2].updateCard(model.rules.cardTypes[1], 3, 1);
            cards[3].updateCard(model.rules.cardTypes[2], 3, 1);
            cards[4].updateCard(model.rules.cardTypes[3], 3, 1);
            cards[5].updateCard(model.rules.cardTypes[4], 3, 1);
            cards[6].updateCard(model.rules.cardTypes[5], 3, 1);      
        }
    }

    public void Back()
    {
        foreach (GameObject go in parts)
        { if (go!=null) go.SetActive(false); }

        types.SetActive(true);
    }

    public void Process(string id)
    {
       // GameManager.Instance.myIAPManager.Process(id,this);
    }

    public void Buy(int goodID)
    {
        switch(goodID)
        {
            case Model.GOOD_GOLD1000:
            case Model.GOOD_GOLD350:
            case Model.GOOD_GOLD50:
            case Model.GOOD_GOLD_PACK4:
            case Model.GOOD_GOLD_PACK5:
            case Model.GOOD_GOLD_PACK6:
            break;

            default:
            Rules rules = GameManager.Instance.model.rules;
            User user = GameManager.Instance.model.user;
            Good good = rules.goods[goodID];

            if (user.gold < good.cost) {
                noMoney.SetActive(true);
                return;
            }
            break;
        }

        StartCoroutine(BuyProcess(goodID));
    }

    IEnumerator BuyProcess(int goodID)
    {
        yield return GameManager.Instance.model.buy(goodID);

        switch (model.buyResult.result)
        {
            case Model.GOOD_GOLD1000:
            case Model.GOOD_GOLD350:
            case Model.GOOD_GOLD50:
            case Model.GOOD_GOLD_PACK4:
            case Model.GOOD_GOLD_PACK5:
            case Model.GOOD_GOLD_PACK6:
            break;

            case Model.GOOD_BACKPACK3:
                openBackpackWindow.BuyBackpack(BackPuckController.BACKBACK_LEVEL_3);
            break;

            case Model.GOOD_BACKPACK2:
                openBackpackWindow.BuyBackpack(BackPuckController.BACKBACK_LEVEL_2);
            break;

            case Model.GOOD_BACKPACK1:
                openBackpackWindow.BuyBackpack(BackPuckController.BACKBACK_LEVEL_1);
            break;

            case Model.GOOD_VIP30:
            case Model.GOOD_VIP15:
            case Model.GOOD_VIP3:
            case Model.GOOD_VIP_V4:
            case Model.GOOD_VIP_V5:
            case Model.GOOD_VIP_V6:
            break;
        }

        if (model.buyResult.result != -1)
        {
            buyComplete.SetActive(true);
            CloseWindow();
            CloseWindow();
        }

        if (homeScene != null) homeScene.updateText();
    }

    public void OpenGoldBuy()
    {
        OpenWindow();
        SelectType(0);
    }

    public void OpenBackpacksBuy()
    {
        OpenWindow();
        SelectType(2);
    }

    public void ShowFromShop(int id)
    { cardsUpgrade.ShowFromShop(id); }
}