using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaRewardWindow : MonoBehaviour
{
    public GameObject back01;
    public GameObject back02;
    public GameObject back03;

    public TextMeshProUGUI gold;
    public TextMeshProUGUI exp;
    public TextMeshProUGUI time;

    [SerializeField]
    GameObject rewardTimerBack;
    [SerializeField]
    GameObject rewardTimer;
    [SerializeField]
    GameObject helpButt;

    public GameObject GrabButton;

    public void Show()
    {
        Model model = GameManager.Instance.model;

        if ((!model.user.arenaRewardPresent) || model.user.arenaResult.empty() || model.arenaAwardTimeout())
        {
            ShowDisabled();
            Debug.Log("AAAAAAAAAAAA");
            return;
        };

        gold.text = model.user.arenaResult.gold.ToString();
        exp.text = model.user.arenaResult.exp.ToString();

        int from = 2;
        for ( int i = 0; i < model.rules.arenaRewards.Count; i++)
        { if (model.user.arenaResult.exp == model.rules.arenaRewards[i].exp) from = model.rules.arenaRewards[i].from; }

        int id = 1;
        int rank = model.ranks.rank;
        rewardTimer.SetActive(rank <= 100);
        rewardTimerBack.SetActive(rank <= 100);
        helpButt.SetActive(rank > 100);
        Debug.Log("rating !!!!!!!!!!!!!!! " + rank);

        if (rank > 3)
            id = 1;
        if (rank < 3)
            id = 2;
        if (rank < 2)
            id = 3;
        
        //if (from < 2) id = 1;
       // if (from > 3) id = 3;
        back01.SetActive(id == 1);
        back02.SetActive(id == 2);
        back03.SetActive(id == 3);

        if(rank <= 100)
            GrabButton.SetActive(model.user.arenaRewardPresent);
        else
            GrabButton.SetActive(false);
    }

    public void ShowDisabled()
    {
        Model model = GameManager.Instance.model;
        gameObject.SetActive(true);

        gold.text = "0";
        exp.text = "0";

        foreach (ArenaReward ar in model.rules.arenaRewards)
        {
            if (ar.contain(model.ranks.rank)) {
                gold.text = ar.gold.ToString();
                exp.text = ar.exp.ToString();
                Debug.Log("Gold: " + gold.text);
                Debug.Log("Exp: " + exp.text);
            }
        }
        //int id = 2;
        //if (model.ranks.rank < 2) id = 1;
        //if (model.ranks.rank > 3) id = 3;
        int id = 1;
        int rank = model.ranks.rank;
        rewardTimer.SetActive(rank < 101);
        rewardTimerBack.SetActive(rank < 101);
        helpButt.SetActive(rank > 100);
        Debug.Log("rating !!!!!!!!!!!!!!! " + rank);
        if (rank > 3)
            id = 1;

        if (rank < 3)
            id = 2;

        if (rank < 2)
            id = 3;

        back01.SetActive(id == 1);
        back02.SetActive(id == 2);
        back03.SetActive(id == 3);
        GrabButton.SetActive(false);
    }
}