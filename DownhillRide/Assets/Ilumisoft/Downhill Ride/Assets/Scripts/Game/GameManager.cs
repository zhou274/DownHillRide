using Ilumisoft.SceneManagement;
using System;
using UnityEngine;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

namespace Ilumisoft.Game
{
    public class GameManager : MonoBehaviour
    {
        private IScoreSystem _scoreSystem;

        private IHighscoreSystem _highscoreSystem;

        private SceneManager _sceneManager;

        private EventManager _eventManager;

        private GameEvent _gameOverEvent;

        public GameObject GameOverPanel;

        public static Action GameOverEvent;

        public string clickid;
        private StarkAdManager starkAdManager;
        private void Awake()
        {
            _eventManager = FindObjectOfType<EventManager>();
            _sceneManager = FindObjectOfType<SceneManager>();

            _scoreSystem = InterfaceUtilities.FindObjectOfType<IScoreSystem>();
            _highscoreSystem = InterfaceUtilities.FindObjectOfType<IHighscoreSystem>();

            GameOverEvent += OnGameOver;
        }
        private void OnDestroy()
        {
            GameOverEvent -= OnGameOver;
        }
        private void Start()
        {
            _gameOverEvent = _eventManager.GetEvent<GameOverEvent>();
            _gameOverEvent.AddListener(OnGameOver);

            _scoreSystem.ResetScore();

            Time.timeScale = 1.0f;
        }

        private void Update()
        {
            _scoreSystem.ModifyScore(5 * Time.deltaTime);
        }

        private void OnGameOver()
        {
            //GameOver should only be triggered once
            //_gameOverEvent.RemoveListener(OnGameOver);

            //Stop game time

            GameOverPanel.SetActive(true);
            //Update highscore
            if (_scoreSystem.Score > _highscoreSystem.Highscore)
            {
                _highscoreSystem.Highscore = _scoreSystem.Score;
            }
            Time.timeScale = 0.0f;
            //Load game over scene after 1 second
            //_sceneManager.LoadSceneDelayed("Game Over", delay: 1.0f);
        }
        public void OnBackMenu()
        {
            _gameOverEvent.RemoveListener(OnGameOver);
            if (_scoreSystem.Score > _highscoreSystem.Highscore)
            {
                _highscoreSystem.Highscore = _scoreSystem.Score;
            }
            ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
            _sceneManager.LoadSceneDelayed("Game Over", delay: 0.3f);

        }
        public void OnContinueGame()
        {
            ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    GameOverPanel.SetActive(false);
                    Player.PlayerResapwn();


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
            
        }


        public void getClickid()
        {
            var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
            if (launchOpt.Query != null)
            {
                foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                    if (kv.Value != null)
                    {
                        Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                        if (kv.Key.ToString() == "clickid")
                        {
                            clickid = kv.Value.ToString();
                        }
                    }
                    else
                    {
                        Debug.Log(kv.Key + "<-参数-> " + "null ");
                    }
            }
        }

        public void apiSend(string eventname, string clickid)
        {
            TTRequest.InnerOptions options = new TTRequest.InnerOptions();
            options.Header["content-type"] = "application/json";
            options.Method = "POST";

            JsonData data1 = new JsonData();

            data1["event_type"] = eventname;
            data1["context"] = new JsonData();
            data1["context"]["ad"] = new JsonData();
            data1["context"]["ad"]["callback"] = clickid;

            Debug.Log("<-data1-> " + data1.ToJson());

            options.Data = data1.ToJson();

            TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
               response => { Debug.Log(response); },
               response => { Debug.Log(response); });
        }


        /// <summary>
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="closeCallBack"></param>
        /// <param name="errorCallBack"></param>
        public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
            }
        }

        /// <summary>
        /// 播放插屏广告
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="errorCallBack"></param>
        /// <param name="closeCallBack"></param>
        public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
                mInterstitialAd.Load();
                mInterstitialAd.Show();
            }
        }
    }
}