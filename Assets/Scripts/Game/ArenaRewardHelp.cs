using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaRewardHelp : MonoBehaviour
{
    public TranslatorScript onePlace;
    public TranslatorScript somePlaces;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI exp;

    public void UpdateData(ArenaReward arenaReward)
    {
        onePlace.gameObject.SetActive(arenaReward.onePlace());
        somePlaces.gameObject.SetActive(!arenaReward.onePlace());
        if (arenaReward.onePlace())
        {
            onePlace.SetValue(arenaReward.from);
        }
        else
        {
            somePlaces.SetValue(arenaReward.from);
            somePlaces.SetValue2(arenaReward.to);
        }
        gold.text = arenaReward.gold.ToString();
        exp.text = arenaReward.exp.ToString();
    }
}