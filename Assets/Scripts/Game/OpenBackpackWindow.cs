using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenBackpackWindow : MonoBehaviour
{
    public List<TextMeshProUGUI> Count;
    public List<TextMeshProUGUI> Gold;
    public List<TextMeshProUGUI> Exp;

    public GameObject VipGO;
    public GameObject ExpGO;
    public GameObject GoldGO;

    public TextMeshProUGUI VipDelta;
    public TextMeshProUGUI ExpDelta;
    public TextMeshProUGUI GoldDelta;

    public GameObject OpenedBackpack;
    public GameObject OpenedBackpack2;
    public GameObject OpenedBackpack3;
    public GameObject ClosedBackpack;
    public GameObject ClosedBackpack2;
    public GameObject ClosedBackpack3;

    public CreatureCardViewUI cardView;

    public GameObject newLevelWindow;

    public PreloaderAmination preloader;

    public GameObject CloseButton;

    public TutorialScript tutorial;

    [SerializeField]
    Image bg;

    [SerializeField]
    Sprite[] sprites;

    void Start ()
    { preloader.Deactivate(); }

    private void OpenWindowWithDefaultState()
    {
        gameObject.SetActive(true);
        ShowDefaultState();
        gameObject.SetActive(true);
        ShowDefaultState();
    }

    public void OpenWindow()
    {
        if (GameManager.Instance.model.user.backpacks>0)
        {
            situation = SITUATION_OPENBACKPACK;
            OpenWindowWithDefaultState();
        }
    }

    public const int BACKBACK_LEVEL_1 = 1;
    public const int BACKBACK_LEVEL_2 = 2;
    public const int BACKBACK_LEVEL_3 = 3;

    public void BuyBackpack(int level)
    {
        switch (level)
        {
            case BACKBACK_LEVEL_1:
            situation = SITUATION_BUY_BACKPACK_1;
            OpenWindowWithDefaultState();
            ClosedBackpack.SetActive(true);
            ClosedBackpack2.SetActive(false);
            ClosedBackpack3.SetActive(false);
            break;

            case BACKBACK_LEVEL_2:
            situation = SITUATION_BUY_BACKPACK_2;
            OpenWindowWithDefaultState();
            ClosedBackpack.SetActive(false);
            ClosedBackpack2.SetActive(true);
            ClosedBackpack3.SetActive(false);
            break;

            case BACKBACK_LEVEL_3:
            situation = SITUATION_BUY_BACKPACK_3;
            OpenWindowWithDefaultState();
            ClosedBackpack.SetActive(false);
            ClosedBackpack2.SetActive(false);
            ClosedBackpack3.SetActive(true);
            break;
        }
        CloseButton.SetActive(false);
        bg.sprite = sprites[0];
    }

    public void StarterPack()
    {
        if (GameManager.Instance.model.starterPackResult.result != -1) return;
        situation = SITUATION_STARTER_PACK;
        OpenWindowWithDefaultState();
        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(true);
        CloseButton.SetActive(false);
        bg.sprite = sprites[0];
        GameManager.Instance.model.backpackResult = GameManager.Instance.model.starterPackResult.mainContent;
    }

    private void OpenStarterPackAdditional()
    {
        situation = SITUATION_STARTER_PACK_ADD;
        OpenWindowWithDefaultState();
        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(true);
        CloseButton.SetActive(false);
        GameManager.Instance.model.backpackResult = GameManager.Instance.model.starterPackResult.additionContent;
    }

    public void OpenTutorialBackpack()
    {
        situation = SITUATION_TUTORIAL_PACK;
        OpenWindowWithDefaultState();
        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(true);
        CloseButton.SetActive(false);
        bg.sprite = sprites[0];
        GameManager.Instance.model.backpackResult = GameManager.Instance.model.starterPackContent;
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    public void OpenBackback()
    {
        if (GameManager.Instance.model.user.backpacks == 0) return;
        StartCoroutine(OpenBackbackProcess());
    }

    private bool blocked = false;

    private void ShowBackpackContent(BackpackResult result)
    {
        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(false);
        OpenedBackpack.SetActive(true);
        OpenedBackpack2.SetActive(false);
        OpenedBackpack3.SetActive(false);

        UpdateText(Count, GameManager.Instance.model.user.backpacks);
        UpdateText(Gold, GameManager.Instance.model.user.gold);
        UpdateText(Exp, GameManager.Instance.model.user.exp);

        ExpGO.SetActive(result.deltaExp > 0);
        ExpDelta.text = result.deltaExp.ToString();

        GoldGO.SetActive(result.deltaGold > 0);
        GoldDelta.text = result.deltaGold.ToString();

        VipGO.SetActive(result.deltaPremium > 0);
        VipDelta.text = result.deltaPremium.ToString();

        cardView.gameObject.SetActive(false);

        currentCard = -1;

        if ((GameManager.Instance.model.upLevelReward != null) && (GameManager.Instance.model.upLevelReward.Count > 0))
        {
            Debug.Log(GameManager.Instance.model.upLevelReward);
            GameManager.Instance.model.upLevelReward.Reverse();
            foreach (Level l in GameManager.Instance.model.upLevelReward)
            {
                Debug.Log(l);
                GameObject go = Instantiate(newLevelWindow);
                go.SetActive(true);
                go.GetComponent<NewLevelWindow>().Show(l);
                go.transform.SetParent(gameObject.transform);
                go.transform.localScale = newLevelWindow.transform.localScale;
                go.transform.position = Vector3.zero;
            }
            GameManager.Instance.model.upLevelReward.Clear();
        }
    }

    private IEnumerator OpenBackbackProcess()
    {
        blocked = true;
        preloader.Activate();
        yield return GameManager.Instance.model.openBackpack();
        BackpackResult result = GameManager.Instance.model.backpackResult;

        if ((result == null) || (result.result == -1))
        { ShowDefaultState(); }
        else
        { ShowBackpackContent(result); }

        blocked = false;
        preloader.Deactivate();
    }

    private bool ShowCard()
    {
        currentCard++;
        BackpackResult result = GameManager.Instance.model.backpackResult;

        if (result.cards.Count > currentCard)
        {
            ExpGO.SetActive(false);
            GoldGO.SetActive(false);
            VipGO.SetActive(false);
            cardView.gameObject.SetActive(true);
            cardView.updateCard(GameManager.Instance.model.rules.cardTypes[result.cards[currentCard].id], result.cards[currentCard].level, result.cards[currentCard].p);
            return true;
        }
        else
        { return false; }
    }

    public void GetReward()
    { ShowDefaultState(); }

    private void ShowDefaultState()
    {
        ClosedBackpack.SetActive(true);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(false);
        OpenedBackpack.SetActive(false);
        OpenedBackpack2.SetActive(false);
        OpenedBackpack3.SetActive(false);
        VipGO.SetActive(false);
        GoldGO.SetActive(false);
        ExpGO.SetActive(false);
        cardView.gameObject.SetActive(false);
        UpdateText(Count, GameManager.Instance.model.user.backpacks);
        CloseButton.SetActive(true);
        bg.sprite = sprites[1];
    }

    private void UpdateText(List<TextMeshProUGUI>gui, int value)
    {
        if (gui != null)
        {
            foreach (TextMeshProUGUI t in gui)
                t.text = value.ToString();
        }
    }

    int currentCard = -1;

    public void Click()
    {
        if (blocked) return;
        if (OpenedBackpack.activeInHierarchy || OpenedBackpack2.activeInHierarchy || OpenedBackpack3.activeInHierarchy)
        {
            switch (situation)
            {
                case SITUATION_OPENBACKPACK:
                    if (ShowCard()) return;

                    if (GameManager.Instance.model.user.backpacks > 0)
                    { ShowDefaultState(); }
                    else
                    { CloseWindow(); }
                break;

                case SITUATION_COUPON:
                    CloseWindow();
                break;

                case SITUATION_BUY_BACKPACK_1:
                    if (ShowCard()) return;
                    CloseWindow();
                break;

                case SITUATION_BUY_BACKPACK_2:
                    if (ShowCard()) return;
                    CloseWindow();
                break;

                case SITUATION_BUY_BACKPACK_3:
                    if (ShowCard()) return;
                    CloseWindow();
                break;

                case SITUATION_STARTER_PACK:
                    if (ShowCard()) return;
                    OpenStarterPackAdditional();
                break;

                case SITUATION_STARTER_PACK_ADD:
                    if (ShowCard()) return;
                    CloseWindow();
                break;

                case SITUATION_TUTORIAL_PACK:
                    if (ShowCard()) return;
                    if (tutorial!=null) tutorial.SwitchToNextTutorialScene();
                    CloseWindow();
                break;
            }
        }
        else
        {
            switch (situation)
            {
                case SITUATION_OPENBACKPACK:
                    OpenBackback();
                break;

                case SITUATION_COUPON:
                    showCouponResult(couponResult);
                break;

                case SITUATION_BUY_BACKPACK_1:
                    ShowBackpackContent(GameManager.Instance.model.backpackResult);
                break;

                case SITUATION_BUY_BACKPACK_2:
                    ShowBackpackContent(GameManager.Instance.model.backpackResult);
                    OpenedBackpack.SetActive(false);
                    OpenedBackpack2.SetActive(true);
                break;

                case SITUATION_BUY_BACKPACK_3:
                    ShowBackpackContent(GameManager.Instance.model.backpackResult);
                    OpenedBackpack.SetActive(false);
                    OpenedBackpack3.SetActive(true);
                break;

                case SITUATION_STARTER_PACK:
                    ShowBackpackContent(GameManager.Instance.model.starterPackResult.mainContent);
                    OpenedBackpack.SetActive(false);
                    OpenedBackpack3.SetActive(true);
                break;

                case SITUATION_STARTER_PACK_ADD:
                    ShowBackpackContent(GameManager.Instance.model.starterPackResult.additionContent);
                    OpenedBackpack.SetActive(false);
                    OpenedBackpack3.SetActive(true);
                break;

                case SITUATION_TUTORIAL_PACK:
                    ShowBackpackContent(GameManager.Instance.model.starterPackContent);
                    OpenedBackpack.SetActive(false);
                    OpenedBackpack3.SetActive(true);
                break;
            }
        }
    }

    const int SITUATION_OPENBACKPACK = 0;
    const int SITUATION_COUPON = 1;
    const int SITUATION_BUY_BACKPACK_1 = 2;
    const int SITUATION_BUY_BACKPACK_2 = 3;
    const int SITUATION_BUY_BACKPACK_3 = 4;
    const int SITUATION_STARTER_PACK = 5;
    const int SITUATION_STARTER_PACK_ADD = 6;
    const int SITUATION_TUTORIAL_PACK = 7;


    int situation = SITUATION_OPENBACKPACK;

    public void showCouponResult(CouponResult result)
    {
        situation = SITUATION_COUPON;

        gameObject.SetActive(true);
        blocked = false;

        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(false);
        OpenedBackpack.SetActive(false);
        OpenedBackpack2.SetActive(false);
        OpenedBackpack3.SetActive(true);

        UpdateText(Count, GameManager.Instance.model.user.backpacks);
        UpdateText(Gold, GameManager.Instance.model.user.gold);
        UpdateText(Exp, GameManager.Instance.model.user.exp);

        ExpGO.SetActive(false);

        GoldGO.SetActive(result.goldDelta > 0);
        GoldDelta.text = result.goldDelta.ToString();

        VipGO.SetActive(false);

        cardView.gameObject.SetActive(false);
    }

    CouponResult couponResult;

    public void showCouponClosed(CouponResult result)
    {
        situation = SITUATION_COUPON;
        couponResult = result;

        gameObject.SetActive(true);
        blocked = false;

        ClosedBackpack.SetActive(false);
        ClosedBackpack2.SetActive(false);
        ClosedBackpack3.SetActive(true);
        OpenedBackpack.SetActive(false);
        OpenedBackpack2.SetActive(false);
        OpenedBackpack3.SetActive(false);

        UpdateText(Count, GameManager.Instance.model.user.backpacks);
        UpdateText(Gold, GameManager.Instance.model.user.gold);
        UpdateText(Exp, GameManager.Instance.model.user.exp);

        ExpGO.SetActive(false);
        GoldGO.SetActive(false);
        VipGO.SetActive(false);

        cardView.gameObject.SetActive(false);
    }
}