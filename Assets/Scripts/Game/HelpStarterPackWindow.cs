using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpStarterPackWindow : MonoBehaviour
{
    public TranslatorScript gold;
    public TranslatorScript exp;
    public TranslatorScript premiumCards;
    public TranslatorScript card2level;
    public TranslatorScript card3level;
    public TranslatorScript disableAds;
    public TranslatorScript premiumDays;
    public TranslatorScript heavyBackbacks;
    public ScrollRect scrollRect;
    public GameObject downButton;
    public GameObject buyButton;

    public void OpenWindow()
    {
        UpdateData();
        gameObject.SetActive(true);
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    private void UpdateData()
    {
        StarterPack starterPack = GameManager.Instance.model.rules.starterPack;
        gold.SetValue(starterPack.gold);
        exp.SetValue(starterPack.exp);
        premiumCards.SetValue(starterPack.premiumCards);
        card2level.SetValue(starterPack.card2level);
        card3level.SetValue(starterPack.card3level);
        disableAds.SetValue(starterPack.disableAds);
        premiumDays.SetValue(starterPack.premiumDays);
        heavyBackbacks.SetValue(starterPack.heavyBackbacks);
    }

    public void ClickDown()
    {
        if (scrollRect.verticalNormalizedPosition > 0.6) scrollRect.verticalNormalizedPosition = 0.5f;
        else scrollRect.verticalNormalizedPosition = 0.0f;
        UpdateButtonView();
    }

    public void UpdateButtonView()
    {
        Debug.Log(scrollRect.verticalNormalizedPosition);
        downButton.SetActive(scrollRect.verticalNormalizedPosition >= 0.1);
        buyButton.SetActive(scrollRect.verticalNormalizedPosition < 0.1);
    }
}