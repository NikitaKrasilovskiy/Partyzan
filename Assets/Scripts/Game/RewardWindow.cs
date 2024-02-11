using CCGKit;
using UnityEngine;

public class RewardWindow : MonoBehaviour
{
    public TranslatorScript Exp;
    public TranslatorScript Gold;
    public TranslatorScript DoubleExp;
    public TranslatorScript DoubleGold;
    public GameObject DoubleOffer;
    public GameObject hider;

    public void ShowCurrentReward()
    {
        Model model = GameManager.Instance.model;
        gameObject.SetActive(true);
        try
        {
            Exp.SetValue(model.battleScript.profileResult.deltaExp);
            Gold.SetValue(model.battleScript.profileResult.deltaGold);
            DoubleExp.SetValue(model.battleScript.profileResult.deltaExp * 2);
            DoubleGold.SetValue(model.battleScript.profileResult.deltaGold * 2);

#if !STEAM
            hider.SetActive(FindObjectOfType<BattleScene>().rewardedAd.IsLoaded());
#endif

        }
        catch (System.Exception ex)
        { Debug.LogError("Exception in RewardWindow: " + ex.Message); }
    }

    public void ShowDoubleResult()
    {
        Model model = GameManager.Instance.model;
        Exp.SetValue(model.battleScript.profileResult.deltaExp*2);
        Gold.SetValue(model.battleScript.profileResult.deltaGold*2);
        DoubleOffer.SetActive(false);
    }
}