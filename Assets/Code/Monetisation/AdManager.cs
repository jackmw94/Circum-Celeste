using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.Level;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour, IUnityAdsShowListener
{
    [SerializeField] private LevelManager _levelManager;
    [Space(15)]
    [SerializeField] private int _noAdsForNLevels = 3;
    [SerializeField] private int _runAdEveryNLevels = 3;
    [SerializeField] private float _minimumTime = 45f;
    [Space(15)]
    [SerializeField] private float _showBannerStartDelay = 1.5f;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private int _levelsCount;
    private float _lastAdTime = -1000f;
    
    private bool ForceAds
    {
        get
        {
#if CIRCUM_DEBUG
            return true;
#endif
            return false;
        }
    }

    private string InterstitialAdUnitId
    {
        get
        {
#if UNITY_IOS
            return "Interstitial_iOS";
#else
            return "Interstitial_Android";
#endif
        }
    }
    
    private string BannerAdUnitId
    {
        get
        {
#if UNITY_IOS
            return "Banner_iOS";
#elif UNITY_ANDROID
            return "Banner_Android";
#endif
        }
    }

    private string GameId
    {
        get
        {
#if UNITY_IOS
            return "4708126";
#elif UNITY_ANDROID
            return "4708127";
#endif
        }
    }
    
    private IEnumerator Start()
    {
        _levelManager.LevelAboutToStart += LevelAboutToStartListener;
        
        Advertisement.Initialize(GameId);
        
        Advertisement.Banner.SetPosition(_bannerPosition);
        
        LoadBanner();
        Advertisement.Load(InterstitialAdUnitId);
        
        yield return new WaitUntil(() => SceneManager.sceneCount == 1);
        yield return new WaitForSeconds(_showBannerStartDelay);
        
        if (!RemoteDataManager.Instance.IsEarlyBird || ForceAds)
        {
            ShowBannerAd();
        }
    }

    private void LevelAboutToStartListener(bool isTutorial)
    {
        if (isTutorial)
        {
            return;
        }
        
        if (RemoteDataManager.Instance.IsEarlyBird && !ForceAds)
        {
            return;
        }
        
        _levelsCount++;
        
        if (_levelsCount <= _noAdsForNLevels)
        {
            CircumDebug.Log($"No ad because we haven't started {_noAdsForNLevels} levels since either start or last ad (current={_levelsCount})");
            return;
        }

        if (Random.Range(0, _runAdEveryNLevels) != 0)
        {
            CircumDebug.Log("No ad because random chance said so");
            return;
        }

        float timeSinceLastAd = Time.time - _lastAdTime;
        if (timeSinceLastAd < _minimumTime)
        {
            CircumDebug.Log($"No ad because minimum time hasn't elapsed ({timeSinceLastAd} < {_minimumTime})");
            return;
        }

        _lastAdTime = Time.time;
        _levelsCount = 0;
        CircumDebug.Log("Showing interstitial ad");
        Advertisement.Show(InterstitialAdUnitId, this);
    }

    private void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = () => {},
            errorCallback = ErrorCallback
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(BannerAdUnitId, options);
    }

    private void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = () => {},
            hideCallback = () => {},
            showCallback = () => {}
        };
 
        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(BannerAdUnitId, options);
    }

    private void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    private void ErrorCallback(string message)
    {
        CircumDebug.Log($"Error loading ad : {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        CircumDebug.LogError($"Could not show ad {placementId} due to error {error}: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        if (placementId.Equals(InterstitialAdUnitId))
        {
            HideBannerAd();
        }
    }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(InterstitialAdUnitId))
        {
            ShowBannerAd();
        }
    }
}
