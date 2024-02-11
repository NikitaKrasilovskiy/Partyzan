using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.IO;
using CCGKit;
//using Firebase.Analytics;

public class AdManager : MonoBehaviour
{
    private RewardedAd _rewardAd;
    [SerializeField] private HomeScene _homeScene;
    private string _rewardAdUnitId;
    
    
    void Awake()
    {
        MobileAds.Initialize(status => {});
    }
    
    void Start()
    {
        RewardAdRequest();
    }

    public void RewardAdRequest()
    {
        _rewardAdUnitId = "ca-app-pub-5853277310445367/7487611810";
        
        _rewardAd = new RewardedAd(_rewardAdUnitId);
        _rewardAd.OnUserEarnedReward += HandleUserEarnedReward;
        _rewardAd.OnAdClosed += HandleRewardedAdClosed;
        _rewardAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        _rewardAd.OnAdFailedToLoad += HandleFailedToLoad;
        AdRequest request = new AdRequest.Builder().Build();
        _rewardAd.LoadAd(request);
    }

    public void ShowRewardVideo()
    {
        if (_rewardAd.IsLoaded())
        {
            _rewardAd.Show();
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression);
        }
    }
    
    private void HandleFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RewardAdRequest();
    }
    
    private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        RewardAdRequest();
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        _homeScene.GetComponent<HomeScene>().BattleOnArena();
    }
}
