using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CardsUpgrade : MonoBehaviour
{
    public CreatureCardViewUI card;
    public GameObject DescriptionWithUpgrade;
    public GameObject BuyPremiumCard;
    public GameObject ActionTypes;

    public GameObject buyComplete;
    public GameObject noMoney;

    public GameObject level2;
    public GameObject level3;
    public GameObject level2Button;
    public GameObject level3Button;
    public GameObject upgradeButton;

    public TranslatorScript level2Text;
    public TranslatorScript level3Text;
    public TextMeshProUGUI level2Cost;
    public TextMeshProUGUI level3Cost;

    public TranslatorScript Name;
    public TranslatorScript level1Level;
    public TranslatorScript level1Damage;
    public TranslatorScript level1HP;
    public TranslatorScript level1Ability;

    public TranslatorScript level2Damage;
    public TranslatorScript level2HP;
    public TranslatorScript level2Ability;
    public Image level2AbilityImage;

    public TranslatorScript level3Damage;
    public TranslatorScript level3HP;
    public TranslatorScript level3Ability;
    public Image level3AbilityImage;

    public List<Sprite> abilities;

    public bool canUpgrade;

    public HomeScene homeScene;

    public TranslatorScript premiumAbility;
    public Image premiumAbilityImage;
    public TranslatorScript premiumCost;
    public TranslatorScript premiumDescription;
    public GameObject premiumBuyButton;
    public TextMeshProUGUI premiumName;

    public TranslatorScript sellCost;
    public GameObject sellResultWindow;
    public TranslatorScript sellResultText;
    public Transform sellButton;

    private DeckPart deckPart;
    private int id;

    public AudioSource audioSource;

    public AudioClip upgradeSound;

    public TutorialScript tutorial;

    [SerializeField]
    SellResult sellResult;

    [SerializeField]
    GameObject tape;

    void Start()
    { Assert.IsNotNull(homeScene); }

    void Update()
    {
        if(BuyPremiumCard.activeSelf && ActionTypes.activeSelf)
           BuyPremiumCard.SetActive(false);
    }

    private SelectCardWindow selectCardWindow;

    public void OpenWindow(DeckPart deckPart, int id, SelectCardWindow selectCardWindow = null)
    {
        this.selectCardWindow = selectCardWindow;
        this.deckPart = deckPart;
        this.id = id;
        OpenVariantsWindow();
        if (tutorial!=null)
        {
            switch (GameManager.Instance.model.getTutorialScene())
            {
                case 10:
                    SwitchToUpgrade();
                    tutorial.SwitchToNextTutorialScene();
                break;

                case 16:
                    tutorial.SwitchToNextTutorialScene();
                break;
            }
        }
    }

    public void updateTutorialUI()
    {
        Debug.Log("Tutorial Scene: " + GameManager.Instance.model.getTutorialScene());
        switch (GameManager.Instance.model.getTutorialScene())
        {
            case 12:
                tutorial.dark.SetParent(DescriptionWithUpgrade.transform, true);
                tutorial.dark.SetAsLastSibling();
                level2Button.transform.SetAsLastSibling();
                level3Button.transform.SetAsLastSibling();
            break;

            case 13:
                tutorial.dark.SetParent(DescriptionWithUpgrade.transform, true);
                tutorial.dark.SetAsLastSibling();
            break;

            case 14:
                CloseWindow();
            break;

            case 17:
                tutorial.dark.SetParent(ActionTypes.transform, true);
                tutorial.dark.SetAsLastSibling();
                sellButton.SetAsLastSibling();
            break;

            case 18:
                tutorial.dark.SetParent(ActionTypes.transform, true);
                tutorial.dark.SetAsLastSibling();
            break;
        }
    }

    public void SwitchToUpgrade()
    { OpenWinowUpgrade(); }

    public void SellCard()
    { StartCoroutine(SellCardProcess()); }

    private IEnumerator SellCardProcess()
    {
        Model model = GameManager.Instance.model;

        if (selectCardWindow != null) yield return selectCardWindow.saveDeckProcess();
        yield return model.sell(0,deckPart,id);

        if (model.sellResult.result != -1)
        {
            if (selectCardWindow != null)
            {
                selectCardWindow.CloseWindow();
                selectCardWindow.OpenWindow();
            }

            CloseWindow();
            if (tutorial != null && tutorial.gameObject.activeSelf)
            {
                switch (GameManager.Instance.model.getTutorialScene())
                {
                    case 17:
                    sellResultWindow.SetActive(false);
                    tutorial.SwitchToNextTutorialScene();
                    break;
                }
            }
            else
            { sellResult.ShowResult(model.sellResult.goldDelta); }
        }
    }

    private void OpenWinowUpgrade()
    {
        gameObject.SetActive(true);
        DescriptionWithUpgrade.SetActive(true);
        BuyPremiumCard.SetActive(false);
        ActionTypes.SetActive(false);
        updateCard();
    }

    private void OpenVariantsWindow()
    {
        gameObject.SetActive(true);
        DescriptionWithUpgrade.SetActive(false);
        BuyPremiumCard.SetActive(false);
        ActionTypes.SetActive(true);
        updateCard();
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    private int goodID;
    private int cardID;

    public void ShowFromShop(int id)
    {
        switch(id)
        {
            case 0: ShowPremium(Model.GOOD_CARD1, 1, true); break;
            case 1: ShowPremium(Model.GOOD_CARD2, 2, true); break;
            case 2: ShowPremium(Model.GOOD_CARD3, 3, true); break;
            case 3: ShowPremium(Model.GOOD_CARD4, 4, true); break;
            case 4: ShowPremium(Model.GOOD_CARD5, 5, true); break;
            case 5: ShowPremium(Model.GOOD_LEADER2, 0, true); break;
            case 6: ShowPremium(Model.GOOD_LEADER3, 0, true); break;
        }
    }

    private void ShowPremium(int goodID, int cardID, bool canBuy)
    {
        gameObject.SetActive(true);
        DescriptionWithUpgrade.SetActive(false);
        BuyPremiumCard.SetActive(true);
        ActionTypes.SetActive(false);
        Rules rules = GameManager.Instance.model.rules;
        CardType cardType = rules.cardTypes[cardID];
        if(tape!=null)
        tape.SetActive(true);
        switch (goodID)
        {
            case Model.GOOD_CARD1:
            case Model.GOOD_CARD2:
            case Model.GOOD_CARD3:
            case Model.GOOD_CARD4:
            case Model.GOOD_CARD5:
                card.updateCard(cardType, 3, 1);
            break;

            case Model.GOOD_LEADER2:
                card.updateCard(cardType, 2, 0);
                if(tape!=null)
                tape.SetActive(false);
                break;

            case Model.GOOD_LEADER3:
                card.updateCard(cardType, 3, 0);
                tape.SetActive(false);
                break;
        }
        Ability ability = cardType.abilities[goodID == Model.GOOD_LEADER2 ? 0 : 1];
        premiumAbility.SetText(ability.text[0]);

        if (ability.values.Length > 0) premiumAbility.SetValue(ability.values[0]);
        if (ability.values.Length > 1) premiumAbility.SetValue(ability.values[1]);

        premiumAbilityImage.sprite = abilities[ability.icon - 1];
        premiumBuyButton.SetActive(canBuy);

        if (canBuy)
        {
            Good good = rules.goods[goodID];
            premiumCost.SetValue(good.cost);
        }
        premiumDescription.SetValue(rules.commonParams.premiumCardBonusFarm);
        premiumDescription.SetValue2(rules.commonParams.premiumCardBonusExp);
        premiumDescription.UpdateText();
        premiumName.text = cardType.title[GameManager.Instance.model.user.lang];
        this.goodID = goodID;
        this.cardID = cardID;
    }

    public void BuyPremiumCardClick()
    {
        Rules rules = GameManager.Instance.model.rules;
        User user = GameManager.Instance.model.user;
        Good good = rules.goods[goodID];

        if (user.gold < good.cost)
        {
            noMoney.SetActive(true);
            return;
        }

        StartCoroutine(BuyPremiumCardProcess());
    }

    private IEnumerator BuyPremiumCardProcess()
    {
        Model model = GameManager.Instance.model;

        if (goodID < 3) yield break;
        yield return model.buy(goodID);

        if (model.buyResult.result != -1) buyComplete.SetActive(true);
    }

    private string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        { return text; }

        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    private void UpgradeLevel()
    { StartCoroutine(UpgradeLevelCoroutine()); }

    private IEnumerator UpgradeLevelCoroutine()
    {
        //yield return GameManager.Instance.model.saveDeck(Model.toJSON(GameManager.Instance.model.user.decks[0]));
        yield return GameManager.Instance.model.upgradeCard(deckPart, id, audioSource, upgradeSound);
        updateCard();
        if (selectCardWindow != null)
        {
            if (deckPart == DeckPart.cards) selectCardWindow.updateCardImage(GameManager.Instance.model.user.decks[0].cards[id]);
            if (deckPart == DeckPart.reserve) selectCardWindow.updateCardImage(GameManager.Instance.model.user.decks[0].reserve[id]);
        }

        homeScene.updateGold();

        if (tutorial!=null)
        {
            switch (GameManager.Instance.model.getTutorialScene())
            {
                case 12:
                    tutorial.SwitchToNextTutorialScene();
                break;
            }
        }
    }

    private void updateCard()
    {
        CardV3 card;
        if (deckPart == DeckPart.cards)
            card = GameManager.Instance.model.user.decks[0].cards[id];
        else
            card = GameManager.Instance.model.user.decks[0].reserve[id];
        CardType cardType = GameManager.Instance.model.rules.cardTypes[card.id];
        this.card.updateCard(cardType, card.level, card.p);
        if (card.p == 1)
        {
            ShowPremium(-1, card.id, false);
            ActionTypes.SetActive(true);
            upgradeButton.SetActive(false);
            sellCost.SetValue(cardType.cardDefs[card.level - 1].sell);
            return;
        }
        else
        { upgradeButton.SetActive(true); }

        showLevel(card.level, card.id);
        int lang = GameManager.Instance.model.user.lang;
        updateCosts(cardType.upgradeCost[0].gold, cardType.upgradeCost[1].gold);
        updateLevel1(cardType.cardDefs[0].attack, cardType.cardDefs[0].hp, "Способности отсутствуют!", cardType.title[0]);
        updateLevel2(cardType.cardDefs[1].attack, cardType.cardDefs[1].hp, cardType.abilities[0]);
        updateLevel3(cardType.cardDefs[2].attack, cardType.cardDefs[2].hp, cardType.abilities[1]);
        sellCost.SetValue(cardType.cardDefs[card.level-1].sell);
    }

    private void updateCosts(int l2, int l3)
    {
        level2Cost.text = l2.ToString();
        level3Cost.text = l3.ToString();
    }

    private void updateLevel1(int damage, int hp, string ability, string name)
    {
        int lang = GameManager.Instance.model.user.lang;
        level1Level.SetValue(1);
        level1Damage.SetValue(damage);
        level1HP.SetValue(hp);
        level1Ability.SetText(ability);
        Name.SetText(name);
    }

    private void updateLevel2(int damage, int hp, Ability ability)
    {
        int lang = GameManager.Instance.model.user.lang;
        level2Damage.SetValue(damage);
        level2HP.SetValue(hp);
        level2Ability.SetText(ability.text[0]);
        if (ability.values.Length > 0) level2Ability.SetValue(ability.values[0]);
        if (ability.values.Length > 1) level2Ability.SetValue2(ability.values[1]);
        level2AbilityImage.sprite = abilities[ability.icon - 1];
    }

    private void updateLevel3(int damage, int hp, Ability ability)
    {
        int lang = GameManager.Instance.model.user.lang;
        level3Damage.SetValue(damage);
        level3HP.SetValue(hp);
        level3Ability.SetText(ability.text[0]);
        if (ability.values.Length > 0) level3Ability.SetValue(ability.values[0]);
        if (ability.values.Length > 1) level3Ability.SetValue2(ability.values[1]);
        level3AbilityImage.sprite = abilities[ability.icon - 1];
    }

    private void showLevel(int level, int id)
    {
        level2Text.SetValue(2);
        level3Text.SetValue(3);
        switch (level)
        {
            case 1:
            level2.SetActive(false);
            level3.SetActive(false);
            level2Button.SetActive(canUpgrade && GameManager.Instance.model.canUpgradeCard(id));
            level3Button.SetActive(false);
            break;

            case 2:
            level2.SetActive(true);
            level3.SetActive(false);
            level2Button.SetActive(false);
            level3Button.SetActive(canUpgrade && GameManager.Instance.model.canUpgradeCard(id));
            break;

            case 3:
            level2.SetActive(true);
            level3.SetActive(true);
            level2Button.SetActive(false);
            level3Button.SetActive(false);
            break;
        }
    }
}