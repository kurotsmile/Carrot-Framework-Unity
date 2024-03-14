using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using System.Collections;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace Carrot
{
    public class Carrot_ads : MonoBehaviour, IUnityAdsShowListener, IUnityAdsInitializationListener, IUnityAdsLoadListener
    {
        //Obj Carrot Ads
        public GameObject window_ads_prefab;
        private Carrot carrot;
        private string s_data_carrotapp_all_ads = "";
        //Config
        private int count_show_ads = 0;
        private bool is_ads = true;

        //Ads Admob config
        private InterstitialAd interstitialAd;
        private BannerView _bannerView;
        private RewardedAd rewardedAd;

        private string id_ads_Interstitial_admob;
        private string id_ads_Banner_admob;
        private string id_ads_Rewarded_admob;

        //Event Customer
        public UnityAction onRewardedSuccess;

        //Consent Form
        ConsentForm _consentForm;

        public void On_load(Carrot carrot)
        {
            this.carrot = carrot;
            if (PlayerPrefs.GetInt("is_ads", 0) == 0)
                this.is_ads = true;
            else
                this.is_ads = false;

            this.set_enable_all_emp_btn_removeads(this.is_ads);

            if (this.carrot.type_ads == TypeAds.Admod_Unity_Carrot || this.carrot.type_ads == TypeAds.Unity_Admob_Carrot)
            {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
                this.setup_ads_Admob();
#endif
            }

            if (this.carrot.type_ads == TypeAds.Admod_Unity_Carrot || this.carrot.type_ads == TypeAds.Unity_Admob_Carrot)
            {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
                Advertisement.Initialize(carrot.id_ads_unity_App_android, carrot.ads_uniy_test_mode, this);
#endif
            }

            if (this.carrot.is_offline() && this.is_ads)
            {
                this.s_data_carrotapp_all_ads = PlayerPrefs.GetString("s_data_carrotapp_all_ads");
            }
        }

        private void set_enable_all_emp_btn_removeads(bool is_show_ads)
        {
            for (int i = 0; i < this.carrot.emp_btn_remove_ads.Length; i++)
            {
                this.carrot.emp_btn_remove_ads[i].SetActive(is_show_ads);
            }
        }


        public void create_banner_ads()
        {
            if (this.is_ads)
            {
                if (carrot.type_ads == TypeAds.Admod_Unity_Carrot)
                {
                    if (id_ads_Banner_admob != "") this.Admob_CreateBannerView();
                }

                if (carrot.type_ads == TypeAds.Unity_Admob_Carrot)
                {
                    if (carrot.id_ads_unity_Banner_android!="") this.Unity_ShowBannerAd();
                }
            }
        }

        public void show_ads_Interstitial()
        {
            if (this.is_ads)
            {
                this.count_show_ads++;
                if (this.count_show_ads >= this.carrot.click_show_ads)
                {
                    if (this.carrot.type_ads == TypeAds.Admod_Unity_Carrot)
                        this.Admob_Show_InterstitialAd();
                    else if (this.carrot.type_ads == TypeAds.Unity_Admob_Carrot)
                        this.Unity_ShowVideoAd();
                    else
                        this.show_carrot_ads();
                    this.count_show_ads = 0;
                }
            }
        }

        public void set_act_Rewarded_Success(UnityAction act)
        {
            this.onRewardedSuccess = act;
        }

        public void show_ads_Rewarded()
        {
            if(carrot.type_ads==TypeAds.Admod_Unity_Carrot) this.Admob_ShowRewardedAd();
            if (carrot.type_ads == TypeAds.Unity_Admob_Carrot) this.Unity_ShowRewardedAd();
        }

        public void remove_ads()
        {
            this.Destroy_Banner_Ad();
            this.Destroy_Interstitial_Ad();
            PlayerPrefs.SetInt("is_ads", 1);
            this.is_ads = false;
            this.set_enable_all_emp_btn_removeads(this.is_ads);
        }

        public bool get_status_ads()
        {
            return this.is_ads;
        }

        public void set_status_ads(bool is_status)
        {
            this.is_ads = is_status;
        }

        public void Show_box_dev_test()
        {
            Carrot_Box box_test = carrot.Create_Box();
            box_test.set_icon(carrot.icon_carrot_ads);
            box_test.set_title("Ads Test");

            Carrot_Box_Item item_admob_video = box_test.create_item();
            item_admob_video.set_title("Admob Video Ads");
            item_admob_video.set_tip("Interstitia ID:" + this.id_ads_Interstitial_admob);
            item_admob_video.set_icon(carrot.game.icon_play_music_game);
            item_admob_video.set_act(() => Admob_Show_InterstitialAd());

            Carrot_Box_Item item_unity_video = box_test.create_item();
            item_unity_video.set_title("Unity Video Ads");
            item_unity_video.set_tip("Interstitia ID:" + this.carrot.id_ads_unity_Interstitial_android);
            item_unity_video.set_icon(carrot.game.icon_play_music_game);
            item_unity_video.set_act(() => Unity_ShowVideoAd());

            Carrot_Box_Item item_admob_Rewarded = box_test.create_item();
            item_admob_Rewarded.set_title("Admob Rewarded Ads");
            item_admob_Rewarded.set_tip("Ads ID:" + this.id_ads_Rewarded_admob);
            item_admob_Rewarded.set_icon(carrot.game.icon_play_music_game);
            item_admob_Rewarded.set_act(() => Admob_ShowRewardedAd());

            Carrot_Box_Item item_unity_Rewarded = box_test.create_item();
            item_unity_Rewarded.set_title("Unity Rewarded Ads");
            item_unity_Rewarded.set_tip("Ads ID:" + this.carrot.id_ads_unity_Rewarded_android);
            item_unity_Rewarded.set_icon(carrot.game.icon_play_music_game);
            item_unity_Rewarded.set_act(() => Unity_ShowRewardedAd());

            Carrot_Box_Item item_carrot_ads = box_test.create_item();
            item_carrot_ads.set_title("Carrot Ads");
            item_carrot_ads.set_tip("App ID:" + this.carrot.Carrotstore_AppId);
            item_carrot_ads.set_icon(carrot.game.icon_play_music_game);
            item_carrot_ads.set_act(() => show_carrot_ads());
        }

        #region Carrot Ads
        [ContextMenu("Show Carrot Ads")]
        public void show_carrot_ads()
        {
            if (this.s_data_carrotapp_all_ads != "")
            {
                this.Act_load_ads_data(this.s_data_carrotapp_all_ads);
            }
            else
            {
                this.carrot.show_loading();
                StructuredQuery q = new("app");
                q.Add_where("status", Query_OP.EQUAL, "publish");
                this.carrot.server.Get_doc(q.ToJson(), Act_show_carrot_ads_done, Act_show_carrot_ads_fail);
            }
        }

        private void Act_show_carrot_ads_done(string s_data)
        {
            this.carrot.hide_loading();
            this.s_data_carrotapp_all_ads = s_data;
            PlayerPrefs.SetString("s_data_carrotapp_all_ads", s_data);
            this.Act_load_ads_data(s_data);
        }

        private void Act_show_carrot_ads_fail(string s_error)
        {
            this.carrot.hide_loading();
            if (this.s_data_carrotapp_all_ads != "") this.Act_load_ads_data(this.s_data_carrotapp_all_ads);
        }

        private void Act_load_ads_data(string s_data)
        {
            Fire_Collection fc = new(s_data);
            if (!fc.is_null) this.Act_show_ads(fc.Get_doc_random().Get_IDictionary());
        }

        private void Act_show_ads(IDictionary data_ads)
        {
            GameObject window_ads = this.carrot.create_window(this.window_ads_prefab);
            window_ads.name = "window_ads";
            window_ads.gameObject.SetActive(true);
            Carrot_Window_Ads ads = window_ads.GetComponent<Carrot_Window_Ads>();
            ads.On_load(this.carrot);
            ads.Load_data_ads(data_ads);
        }
        #endregion

        #region ConsentForm For Admob
        void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                UnityEngine.Debug.LogError(error);
                return;
            }

            if (ConsentInformation.IsConsentFormAvailable())
            {
                LoadConsentForm();
            }
        }

        void LoadConsentForm()
        {
            ConsentForm.Load(OnLoadConsentForm);
        }

        void OnLoadConsentForm(ConsentForm consentForm, FormError error)
        {
            if (error != null)
            {
                UnityEngine.Debug.LogError(error);
                return;
            }
            _consentForm = consentForm;
            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
            {
                _consentForm.Show(OnShowForm);
            }
        }


        void OnShowForm(FormError error)
        {
            if (error != null)
            {
                UnityEngine.Debug.LogError(error);
                return;
            }
            LoadConsentForm();
        }
        #endregion

        #region Admob Ads
        private void setup_ads_Admob()
        {

#if UNITY_ANDROID
            this.id_ads_Interstitial_admob = this.carrot.id_ads_Interstitial_android;
            this.id_ads_Banner_admob = this.carrot.id_ads_Banner_android;
            this.id_ads_Rewarded_admob = this.carrot.id_ads_Rewarded_android;
#elif UNITY_IOS
            this.id_ads_Interstitial_admob = this.carrot.id_ads_Interstitial_ios;
            this.id_ads_Banner_admob = this.carrot.id_ads_Banner_ios;
            this.id_ads_Rewarded_admob = this.carrot.id_ads_Rewarded_ios;
#endif
            RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetSameAppKeyEnabled(true).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize(HandleInitCompleteAction_admob);

            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.EEA
            };

            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        private void HandleInitCompleteAction_admob(InitializationStatus obj)
        {
            this.carrot.log("HandleInitCompleteAction admob !");
            if (this.id_ads_Interstitial_admob != "") this.Admob_LoadInterstitialAd();
            if (this.id_ads_Rewarded_admob != "") this.Admob_LoadRewardedAd();
            this.create_banner_ads();
        }

        private AdRequest get_AdRequest_Admob()
        {
            var adRequest = new AdRequest.Builder()
                   .AddKeyword("game")
                   .AddKeyword("life")
                   .AddKeyword("love")
                   .AddKeyword("fun")
                   .AddKeyword("video").AddExtra("rdp", "1").Build();
            return adRequest;
        }

        private void Admob_LoadInterstitialAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            this.carrot.log("Loading the interstitial ad. (" + this.id_ads_Interstitial_admob + ")");
            InterstitialAd.Load(this.id_ads_Interstitial_admob, this.get_AdRequest_Admob(),
                (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        this.carrot.log("interstitial ad failed to load an ad with error : " + error);
                        return;
                    }

                    this.carrot.log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                    interstitialAd = ad;
                    this.Admob_interstitialAd_RegisterEventHandlers(ad);
                });
        }

        private void Admob_interstitialAd_RegisterEventHandlers(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                this.carrot.log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                this.carrot.log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                this.carrot.log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                this.carrot.log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                this.carrot.log("Interstitial ad full screen content closed.");
                this.Admob_LoadInterstitialAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                this.carrot.log("Interstitial ad failed to open full screen content " + "with error : " + error);
                this.Admob_LoadInterstitialAd();
            };
        }

        private void Admob_Show_InterstitialAd()
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
                this.carrot.log("Showing interstitial Admob Ad! (id:" + this.id_ads_Interstitial_admob + ")");
            }
            else
            {
                if (carrot.type_ads == TypeAds.Admod_Unity_Carrot)
                {
                    this.Unity_ShowVideoAd();
                }
                else
                {
                    this.show_carrot_ads();
                    this.carrot.log("Interstitial ad is not ready yet! (id:" + this.id_ads_Interstitial_admob + ")");
                }
                this.Admob_LoadInterstitialAd();
            }
        }

        public void Destroy_Interstitial_Ad()
        {
            if (interstitialAd != null)
            {
                this.carrot.log("Destroying interstitialAd Admob ad.");
                interstitialAd.Destroy();
                interstitialAd = null;
            }
        }

        private void Admob_CreateBannerView()
        {
            this.carrot.log("Creating banner view (" + this.id_ads_Banner_admob + ")");
            if (_bannerView != null)
            {
                Destroy_Banner_Ad();
            }

            if (this.id_ads_Banner_admob == "") return;

            _bannerView = new BannerView(this.id_ads_Banner_admob, AdSize.Banner, AdPosition.Top);

            // send the request to load the ad.
            this.carrot.log("Loading banner ad.  (" + this.id_ads_Banner_admob + ")");
            _bannerView.LoadAd(this.get_AdRequest_Admob());

            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                this.carrot.log("Banner view loaded an ad with response : " + _bannerView.GetResponseInfo());
            };

            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                this.carrot.log("Banner view failed to load an ad with error : " + error);
                if (this.carrot.type_ads == TypeAds.Admod_Unity_Carrot) this.Unity_ShowBannerAd();
            };

            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                this.carrot.log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                this.carrot.log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                this.carrot.log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                this.carrot.log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                this.carrot.log("Banner view full screen content closed.");
            };
        }


        public void Destroy_Banner_Ad()
        {
            if (_bannerView != null)
            {
                this.carrot.log("Destroying banner ad.");
                _bannerView.Destroy();
                _bannerView = null;
            }

            if (carrot.id_ads_unity_Banner_android != "") Advertisement.Banner.Hide();
        }

        private void Admob_LoadRewardedAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            this.carrot.log("Loading the rewarded ad. (" + this.id_ads_Rewarded_admob + ")");

            var adRequest = new AdRequest.Builder().Build();
            RewardedAd.Load(this.id_ads_Rewarded_admob, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        this.carrot.log("Rewarded ad failed to load an ad " + "with error : " + error);
                        return;
                    }
                    this.carrot.log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                    rewardedAd = ad;
                });
        }

        private void Admob_ShowRewardedAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Show((Reward reward) =>
                {
                    this.onRewardedSuccess.Invoke();
                    this.carrot.log(String.Format("Rewarded ad rewarded the user. Type: {0}, amount: {1}.", reward.Type, reward.Amount));
                });
            }
            else
            {
                if (this.carrot.type_ads == TypeAds.Admod_Unity_Carrot) this.Unity_ShowRewardedAd();
            }
        }
        #endregion

        #region Unity Ads
        public void Unity_ShowBannerAd()
        {
            if (this.carrot.id_ads_unity_Banner_android != "")
            {
                Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
                Advertisement.Banner.Show(this.carrot.id_ads_unity_Banner_android);
            }
        }

        public void Unity_HideBannerAd()
        {
            Advertisement.Banner.Hide();
        }

        public void Unity_ShowVideoAd()
        {
            if(this.carrot.id_ads_unity_Interstitial_android!="") Advertisement.Show(this.carrot.id_ads_unity_Interstitial_android, this);
        }

        public void Unity_ShowRewardedAd()
        {
            if(this.carrot.id_ads_unity_Rewarded_android!="") Advertisement.Show(carrot.id_ads_unity_Rewarded_android, this);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log("Unity Ads show fail:" + placementId + " ->" + message);
            if (placementId == this.carrot.id_ads_unity_Interstitial_android)
            {
                if (carrot.type_ads == TypeAds.Unity_Admob_Carrot)
                    this.Admob_Show_InterstitialAd();
                else
                    this.show_carrot_ads();
            }

            if (placementId == this.carrot.id_ads_unity_Rewarded_android)
            {
                if (carrot.type_ads == TypeAds.Unity_Admob_Carrot)
                    this.Admob_ShowRewardedAd();
                else
                    this.show_carrot_ads();
            }

            if (placementId == this.carrot.id_ads_unity_Banner_android)
            {
                if (carrot.type_ads == TypeAds.Unity_Admob_Carrot)
                {
                    this.Admob_CreateBannerView();
                }
            }
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log("Unity Ads Show start:" + placementId);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("Unity Ads click:" + placementId);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log("Unity Ads show complete:" + placementId);
            if (placementId == this.carrot.id_ads_unity_Rewarded_android)
            {
                if (showCompletionState == UnityAdsShowCompletionState.COMPLETED) onRewardedSuccess?.Invoke();
            }
        }

        public void OnInitializationComplete()
        {
            Advertisement.Load(this.carrot.id_ads_unity_Banner_android, this);
            Advertisement.Load(this.carrot.id_ads_unity_Interstitial_android, this);
            Advertisement.Load(this.carrot.id_ads_unity_Rewarded_android, this);
            Debug.Log("Unity Ads setup success!");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log("Unity Ads setup fail:" + message);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log("Unity Ads load success:" + placementId);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log("Unity Ads Load fail:" + placementId + " ->" + message);
            if (placementId == this.carrot.id_ads_unity_Banner_android)
            {
                if (this.carrot.type_ads == TypeAds.Unity_Admob_Carrot) this.Admob_CreateBannerView();
            }
        }
        #endregion
    }
}