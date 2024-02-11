using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CCGKit;

public class CardView : MonoBehaviour
{
    [SerializeField]
    Image cardBG;
    [SerializeField]
    Sprite[] cardSprites;
    [SerializeField]
    TextMeshProUGUI exp;
    [SerializeField]
    TextMeshProUGUI gold;

    private void OnEnable()
    { StartCoroutine(Enable()); }

    IEnumerator Enable()
    {
        Model model = GameManager.Instance.model;
        yield return model.getRanks();

        int rank = model.ranks.rank;

        foreach (ArenaReward ar in model.rules.arenaRewards)
        {
            if (ar.contain(model.ranks.rank))
            {
                gold.text = ar.gold.ToString();
                exp.text = ar.exp.ToString();
                Debug.Log("Gold: " + gold.text);
                Debug.Log("Exp: " + exp.text);
            }
        }

        if (rank > 3)
        { cardBG.sprite = cardSprites[0]; }
        else
        {
            if (rank > 1)
            { cardBG.sprite = cardSprites[1]; }
            else
            { cardBG.sprite = cardSprites[2]; }
        }
    }
}