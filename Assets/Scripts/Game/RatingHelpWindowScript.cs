using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingHelpWindowScript : MonoBehaviour
{
    public ArenaRewardHelp[] ArenaRewardHelps;

    public void OpenWindow()
    {
        for (int i = 0; i < GameManager.Instance.model.rules.arenaRewards.Count; i++ )
        { ArenaRewardHelps[i].UpdateData(GameManager.Instance.model.rules.arenaRewards[i]); }
        gameObject.SetActive(true);
    }
}