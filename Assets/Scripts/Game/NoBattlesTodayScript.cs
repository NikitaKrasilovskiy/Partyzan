using CCGKit;
using UnityEngine;

public class NoBattlesTodayScript : MonoBehaviour
{
    public TranslatorScript continueText;
    public GameObject ForAdsButton;
    public AdManager adManager;

    public void Open()
    {
        continueText.SetValue(GameManager.Instance.model.rules.commonParams.continueArenaCost);
        gameObject.SetActive(true);
        adManager.GetComponent<AdManager>().RewardAdRequest();

        //#if !STEAM
       // ForAdsButton.SetActive(GameManager.Instance.unityAdsExample.canShowRewardedAd());
        //#endif
    }
}