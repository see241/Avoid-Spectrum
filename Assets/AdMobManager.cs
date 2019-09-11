using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public enum RewardType
{
    Revival, Coin
}

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager instance;
    private RewardBasedVideoAd rewardBasedVideoAd;
    private RewardType requestRewardType;
    private BannerView bannerView;

    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //string appId = "ca-app-pub-6915619357801525/5075173068";
        string appId = "ca-app-pub-6915619357801525~2134504325";

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdRewarded += RewardBasedVideoAd_OnAdRewarded;
        rewardBasedVideoAd.OnAdClosed += RewardBasedVideoAd_OnAdClosed;
        RequestRewardBasedVideoAd();
    }

    private void RewardBasedVideoAd_OnAdClosed(object sender, EventArgs e)
    {
        RequestRewardBasedVideoAd();
    }

    private void RewardBasedVideoAd_OnAdRewarded(object sender, Reward e)
    {
        switch (requestRewardType)
        {
            case RewardType.Revival:
                InGameManager.instance.RevivalReady();
                break;

            case RewardType.Coin:
                InGameManager.instance.AdGold();
                break;
        }
    }

    public void RequestBannerAd()
    {
        Debug.Log("BannerAdLoad Start");
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        string adUnitId = "ca-app-pub-6915619357801525/3355376218";
        AdRequest adRequest = new AdRequest.Builder().Build();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        bannerView.LoadAd(adRequest);
        Debug.Log("BannerAdLoad End");
    }

    public void DestroyBannerAd()
    {
        bannerView.Destroy();
        bannerView = null;
    }

    public void RequestRewardBasedVideoAd()
    {
        Debug.Log("VideoAdLoad Start");
        //string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        string adUnitId = "ca-app-pub-6915619357801525/5075173068";

        AdRequest adRequest = new AdRequest.Builder().Build();
        // Create a 320x50 banner at the top of the screen.
        rewardBasedVideoAd.LoadAd(adRequest, adUnitId);
        Debug.Log("VideoAdLoad End");
    }

    public void ShowRewardBasedVideo(RewardType ad)
    {
        requestRewardType = ad;
        StartCoroutine(ShowRewardBasedVideoAd());
    }

    public bool IsBannerOnScreen()
    {
        return bannerView == null ? false : true;
    }

    private IEnumerator ShowRewardBasedVideoAd()
    {
        while (!rewardBasedVideoAd.IsLoaded())
        {
            yield return null;
        }
        rewardBasedVideoAd.Show();
    }
}