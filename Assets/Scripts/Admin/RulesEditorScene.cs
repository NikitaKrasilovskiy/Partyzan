using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesEditorScene : MonoBehaviour
{
    public BoosterEdit[] booster;

    public PremiumEdit[] premium;

    public RealEdit[] real;

    public InputField battleGoldBattles;
    public InputField battleGoldGold;

    public InputField battleExpWin;
    public InputField battleExpFail;
    public InputField batlesRewardCard;

    public InputField freeBackpackTimer;
    public InputField continueArenaCost;
    public InputField maxCouponsPerDay;

    public ArenaRewardEdit[] arenaRewards;

    Model model;

    void Start ()
    {
        model = GameManager.Instance.model;
        StartCoroutine(setData());
    }

    public void Save()
    {
        getData();
        StartCoroutine(model.saveRulesPart());
    }

    IEnumerator setData()
    {
        yield return model.loadRules();

        for (int i = 0; i < model.rules.boosters.Count; i++) booster[i].setData(i);
        for (int i = 0; i < model.rules.premium.Count; i++) premium[i].setData(i);
        for (int i = 0; i < model.rules.real.Count; i++) real[i].setData(i);
        battleGoldBattles.text = model.rules.battleGold.battles.ToString();
        battleGoldGold.text = model.rules.battleGold.gold.ToString();
        battleExpWin.text = model.rules.battleExp.win.ToString();
        battleExpFail.text = model.rules.battleExp.fail.ToString();
        batlesRewardCard.text = model.rules.battleRewardCard.battleWins.ToString();
        freeBackpackTimer.text = model.rules.commonParams.freeBackpackTimer.ToString();
        continueArenaCost.text = model.rules.commonParams.continueArenaCost.ToString();
        maxCouponsPerDay.text = model.rules.commonParams.maxCouponsPerDay.ToString();
        for (int i = 0; i < model.rules.arenaRewards.Count; i++) arenaRewards[i].setData(i);
    }

    void getData()
    {
        for (int i = 0; i < model.rules.boosters.Count; i++) booster[i].getData();
        for (int i = 0; i < model.rules.premium.Count; i++) premium[i].getData();
        for (int i = 0; i < model.rules.real.Count; i++) real[i].getData();
        model.rules.battleGold.battles = int.Parse(battleGoldBattles.text);
        model.rules.battleGold.gold = int.Parse(battleGoldGold.text);
        model.rules.battleExp.win = int.Parse(battleExpWin.text);
        model.rules.battleExp.fail = int.Parse(battleExpFail.text);
        model.rules.battleRewardCard.battleWins = int.Parse(batlesRewardCard.text);
        model.rules.commonParams.freeBackpackTimer = int.Parse(freeBackpackTimer.text);
        model.rules.commonParams.continueArenaCost = int.Parse(continueArenaCost.text);
        model.rules.commonParams.maxCouponsPerDay = int.Parse(maxCouponsPerDay.text);
        for (int i = 0; i < model.rules.arenaRewards.Count; i++) arenaRewards[i].getData();
    }
}