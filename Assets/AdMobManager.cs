using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager instance;
    RewardBasedVideoAd rewardBasedVideoAd;
    BannerView bannerView;
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
    void Start()
    {
        string appId = "ca-app-pub-6915619357801525~2134504325";

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        this.rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdRewarded += PlayerRevival;
        rewardBasedVideoAd.OnAdClosed += RewardBasedVideoAd_OnAdClosed;
        RequestRewardBasedVideoAd();
    }

    private void RewardBasedVideoAd_OnAdClosed(object sender, System.EventArgs e)
    {
        RequestRewardBasedVideoAd();
    }

    private void PlayerRevival(object sender, Reward e)
    {
        InGameManager.instance.Revival();
    }

    public void RequestBannerAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        AdRequest adRequest = new AdRequest.Builder().AddTestDevice("33BE2250B43518CCDA7DE426D04EE232").Build();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        bannerView.LoadAd(adRequest);
    }
    public void DestroyBannerAd()
    {
        bannerView.Destroy();
        bannerView = null;
    }

    public void RequestRewardBasedVideoAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";

        AdRequest adRequest = new AdRequest.Builder().AddTestDevice("33BE2250B43518CCDA7DE426D04EE232").Build();
        // Create a 320x50 banner at the top of the screen.
        rewardBasedVideoAd.LoadAd(adRequest, adUnitId);
    }
    public void ShowRewardBasedVideo()
    {
        StartCoroutine(ShowRewardBasedVideoAd());
    }
    public bool IsBannerOnScreen()
    {
        return bannerView == null ? false : true;
    }

    IEnumerator ShowRewardBasedVideoAd()
    {
        while (!rewardBasedVideoAd.IsLoaded())
        {
            yield return null;
        }
        rewardBasedVideoAd.Show();
    }
}
