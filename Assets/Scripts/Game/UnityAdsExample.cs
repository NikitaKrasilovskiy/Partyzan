using CCGKit;
using System;
using System.Collections;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements; // only compile Ads code on supported platforms
#endif

public class UnityAdsExample : MonoBehaviour
{
    public void ShowDefaultAd()
    {
#if UNITY_ADS
        if (!Advertisement.IsReady())
        {
            Debug.Log("Ads not ready for default placement");
            return;
        }

        Advertisement.Show();
#endif

    }

    const string RewardedPlacementId = "rewardedVideo";

    public void ShowRewardedAd(Func<int> worker)
    {
#if UNITY_ADS
        this.worker = worker;
        if (!Advertisement.IsReady(RewardedPlacementId))
        {
            Debug.Log(string.Format("Ads not ready for placement '{0}'", RewardedPlacementId));
            return;
        }

        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(RewardedPlacementId, options);
#endif
    }

    public bool canShowRewardedAd()
    {
#if UNITY_ANDROID
        //return Advertisement.IsReady(RewardedPlacementId);
#elif UNITY_STANDALONE
        return false;
#else
        return false;
#endif
        return true;
    }

#if UNITY_ADS
    Func<int> worker;

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
            Debug.Log("The ad was successfully shown.");
            AdditionalRewardProcess();
            break;

            case ShowResult.Skipped:
            Debug.Log("The ad was skipped before reaching the end.");
            break;

            case ShowResult.Failed:
            Debug.LogError("The ad failed to be shown.");
            break;
        }
    }

    void AdditionalRewardProcess()
    {тworker();т}

#endif
}