using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;
#if !STEAM
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

public class DisableAdsScript : MonoBehaviour
{
    public TranslatorScript costLittle;
    public TranslatorScript costBig;

    public void OpenWindow()
    {
        Model model = GameManager.Instance.model;
        costLittle.SetValue(model.rules.goods[Model.GOOD_DISABLE_ADS_LITTLE].cost);
        costBig.SetValue(model.rules.goods[Model.GOOD_DISABLE_ADS_BIG].cost);
        gameObject.SetActive(true);
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }
}