using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CCGKit;
using UnityEngine.Networking;
using System.Text;
using static Hub;

namespace ServerSide
{
    public delegate void TimeCallback(DateTime dt);

    public delegate void ServerCallback(string responce);

    public class ServerData : MonoBehaviour
    {
        [SerializeField]
        //public string API_URL = "http://game.gamedevinvest.ru:8000/";
        public string API_URL = "http://server.partisanwars.net:3726/";
        private string connectionState;

        public DateTime lastRequestTime;

        #region 
        private static ServerData _instance;
        public static ServerData Instance
        {
            get
            {
                if (_instance == null)
                { _instance = FindObjectOfType<ServerData>(); }

                if (_instance == null)
                {
                    GameObject g = new GameObject("ServerData");
                    _instance = g.AddComponent<ServerData>();
                }
                
                return _instance;
            }
        }
        #endregion

        private void OnEnable()
        {
            _instance = this;
          //  Debug.Log("SERVER DATA !NULL " + _instance != null);
            DontDestroyOnLoad(this.gameObject);
        }

        private void Awake()
        {
            _instance = this;
          // Debug.Log("SERVER DATA !NULL "+_instance!=null);
            DontDestroyOnLoad(this.gameObject);

#if UNITY_EDITOR
            //API_URL = "https://server.partisanwars.net:3726/";
            //API_URL = "http://185.200.242.77/";
#endif
        }


        public void GetTime(TimeCallback timeCallback)
        { StartCoroutine(TimeRequest(timeCallback)); }

        public IEnumerator TimeRequest(TimeCallback timeCallback)
        {
            yield return loadDataFromServer("state", (string response) =>
            {
                ServerState serverState = Model.parseJSON<ServerState>(response);
                int time = serverState.servertime;
                TimeSpan timeSpan = new TimeSpan(0, 0, time);
                DateTime dateTime = new DateTime(1970, 1, 1) + timeSpan;
                lastRequestTime = dateTime;

#if UNITY_EDITOR 
                timeCallback.Invoke(DateTime.Now);
#else
                timeCallback.Invoke(dateTime);
#endif

                return 0;
            });
        }

        public IEnumerator TimeRequest()
        {
            yield return loadDataFromServer("state", (string response) =>
            {
                ServerState serverState = Model.parseJSON<ServerState>(response);
                int time = serverState.servertime;
                TimeSpan timeSpan = new TimeSpan(0, 0, time);
                DateTime dateTime = new DateTime(1970, 1, 1) + timeSpan;
                lastRequestTime = dateTime;

                return 0;
            });
        }

        public IEnumerator SteamPriceRequest()
        {
			yield return loadDataFromServer("steamPriceList", (string response) =>
			{
			    hub.steam_prices = Model.parseJSON<List<string>>(response); // (1-6) price ("150 руб." | "5 $")

				return 0;
			});
        }

        public IEnumerator loadDataFromServer(string url, Func<string, int> worker, string body = "", bool handleError = false)
        {
			yield return GameServer.LoadData(url, worker, GameManager.Instance.model.deviceID, body);
		}

        public IEnumerator SteamPurchase(int itemId)
        {
            yield return loadDataFromServer("steamPurchase", (string response)=>
			{
				// в строке response сервер возвращает либо номер заказа, либо FAIL в случае ошибки
				// номер заказа (orderID) надо сохранить и потом (после оплаты в стим панели) вызвать CompliteSteamPurchase(orderID)

				return 0;
			}, body: itemId.ToString());
		}

        public IEnumerator CompliteSteamPurchase(ulong orderID)
        {
			yield return loadDataFromServer("completeSteamPurchase", (string response) =>
			{
				BuyResult buyResult;
				buyResult = Model.parseJSON<BuyResult>(response);
				GameManager.Instance.model.user.gold = buyResult.gold;
				GameManager.Instance.model.user.proTimeout = buyResult.proTimeout;

				return 0;
			}, body: orderID.ToString());
        }
    }
}