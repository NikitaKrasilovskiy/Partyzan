using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeCloud : MonoBehaviour
{
    [SerializeField]
    CloudBG cloudBG;

    [SerializeField]
    PrizeCounter goldPrizeCounter;

    [SerializeField]
    PrizeCounter vipPrizeCounter;

    [SerializeField]
    PrizeCounter expPrizeCounter;

    BackPuckPrize _backPuckPrize;

    public void SetPrizeCloud(BackPuckPrize backPuckPrize)
    {
        gameObject.SetActive(true);
        goldPrizeCounter.gameObject.SetActive(false);
        vipPrizeCounter.gameObject.SetActive(false);
        expPrizeCounter.gameObject.SetActive(false);
        _backPuckPrize = backPuckPrize;
        PrizeCounter prizeCounter = null;

        switch (_backPuckPrize.prizeType)
        {
            case BackPuckPrize.PrizeType.Exp:
                {
                    prizeCounter = expPrizeCounter;
                    break;
                }

            case BackPuckPrize.PrizeType.VIP:
                {
                    prizeCounter = vipPrizeCounter;
                    break;
                }

            case BackPuckPrize.PrizeType.Gold:
                {
                    prizeCounter = goldPrizeCounter;
                    break;
                }

            default:
                {break;}
        }
        prizeCounter.gameObject.SetActive(true);
        prizeCounter.SetCount(_backPuckPrize.count);
    }
}