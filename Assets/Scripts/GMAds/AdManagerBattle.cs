using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.IO;
using CCGKit;
//using Firebase.Analytics;

public class AdManagerBattle : MonoBehaviour
{
    private RewardedAd _rewardAdAd;
    [SerializeField] private BattleScene _battleScene;
    private string _rewardAdUnitIdAd;
    
    void Awake()
    {
        MobileAds.Initialize(status => {});
    }
    
    // Start is called before the first frame update
    void Start()
    {
        RewardAdRequest();
    }
    
    public void RewardAdRequest()
    {
        _rewardAdUnitIdAd = "ca-app-pub-5853277310445367/8528369932";
        
        _rewardAdAd = new RewardedAd(_rewardAdUnitIdAd);
        _rewardAdAd.OnUserEarnedReward += HandleUserEarnedReward2;
        _rewardAdAd.OnAdClosed += HandleRewardedAdClosed2;
        _rewardAdAd.OnAdFailedToShow += HandleRewardedAdFailedToShow2;
        _rewardAdAd.OnAdFailedToLoad += HandleFailedToLoad;
        
        AdRequest request = new AdRequest.Builder().Build();
        _rewardAdAd.LoadAd(request);
    }
    
    public void ShowRewardVideoAd()
    {
        if (_rewardAdAd.IsLoaded())
        {
            _rewardAdAd.Show();
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression);
        }
    }
    
    private void HandleFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RewardAdRequest();
    }
    
    private void HandleRewardedAdFailedToShow2(object sender, AdErrorEventArgs e)
    {
        //RewardAdRequest();
    }

    private void HandleRewardedAdClosed2(object sender, EventArgs e)
    {
        
    }

    private void HandleUserEarnedReward2(object sender, Reward e)
    {
        _battleScene.GetComponent<BattleScene>().openAdditionalReward();
    }
}
