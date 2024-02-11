using FullSerializer;
using ServerSide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if STEAM
using Steamworks;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Hub;
//using Firebase.Analytics;

// ReSharper disable All

namespace CCGKit
{
	[Serializable]
	public class Model
	{
		//[NonSerialized]
		public static string API_URL
		{
			get {return "";}
		}
		//public static string API_URL = "server.partisanwars.net";
		public string name = "Player";
		public int silver = 300;
		public int gold = 50;
		public int exp = 0;
		public int hp;
		public int defaultHP = 20;

		public List<DeckWW> decks = new List<DeckWW>();

		public string deviceID;


		private string did = null;

		[NonSerialized] public Rules rules = new Rules();

		internal int currentLevel;

		public bool cardClickHelpShowed = false;

		public ServerState state;

		static public int TUTORIAL_FINISHED = 1;
		//static private int TOTAL_TUTORIAL_SCENES = 24;

		public int tutorialScene = 0;
		
		public int getTutorialScene()
		{
			 if(tutorialScene < 0) return TUTORIAL_FINISHED;
			 if(tutorialScene == 0) return 1;
			 return tutorialScene;
		}

		public void nextTutorialScene()
		{
			//if(getTutorialScene() == TUTORIAL_FINISHED) return;
			Debug.Log("Номер сцены" + tutorialScene);
			int n = getTutorialScene() + 1;
			tutorialScene = n;
			//if(n > TOTAL_TUTORIAL_SCENES) tutorialScene = TUTORIAL_FINISHED;
		}

		public void restartTutorial()
		{
			tutorialScene = 1;
		}

		//public bool tutorialComplete()
		//{
		//	return getTutorialScene() == TUTORIAL_FINISHED;
		//}

		internal int remainBackpackTime()
		{
			return Math.Max(remain(user.backpackTime), 0);
		}

		internal int remainTaskTime(int n)
		{
			return Math.Max(remain(user.tasks[n].timeout), 0);
		}

		internal int remainArenaReward()
		{
			return Math.Max(remain(state.arenaAwardMoment), 0);
		}

		internal int remainProTime()
		{
			return Math.Max(remain(user.proTimeout), 0);
		}

		internal int remain(int t)
		{
			int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			if(state == null) return 0;
			return t - unixTimestamp + unixTimestampLoad - state.servertime;
		}

		public bool freeArena()
		{
			if(remain(state.arenaDropMoment) < 0) return true;
			if(state.arenaDropMoment - user.lastBattleMoment > 12 * 60 * 60) return true;
			return user.todayBattles < 3;
		}

		internal bool arenaAwardTimeout()
		{
			return ((state.arenaAwardMoment - user.arenaResult.timeout) > 24 * 60 * 60);
		}

		internal int remainProDays()
		{
			int sec = remainProTime();
			if(sec == 0) return 0;
			int days = sec / 24 / 60 / 60;
			days++;
			if(days < 0) days = 0;
			return days;
		}

		[NonSerialized] private int noAdsCounter = 0;

		internal bool adsEnabled()
		{
			noAdsCounter++;
			Debug.Log("adsDisableTimeout: " + remain(user.adsDisableTimeout).ToString());
			if(state.noAds) return false;
			if(noAdsCounter <= 2) return false;
			return remain(user.adsDisableTimeout) < 0;
		}

		public bool canGetFreeBackpack()
		{
			return remainBackpackTime() <= 0;
		}

		CardType currentEditorCard;

		public User user;

		public int userId;

		[NonSerialized] public bool syncronized;

		[NonSerialized] public Battle battleScript;

		public static string connectionState = "";

		public IEnumerator loadDataFromServer(string url, Func<string, int> worker, string body = "",
			bool handleError = false)
		{
			yield return GameServer.LoadData(url, worker, deviceID, body);
			
			/*
			//HACK: пофиксить подмену на лету
			//API_URL = ServerData.Instance.API_URL;
			if (deviceID == null)
                fixDeviceID();
			Debug.Log(url);
            Debug.Log("Request: " + url + " Authorization: " + deviceID);
            if (body != "") Debug.Log("Body: " + body);
            connectionState = "";
            int attempts = 0;
            bool complete = false;
            do {
                UnityWebRequest www = new UnityWebRequest(url);
                if (body != "") www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Authorization",deviceID);
                www.method = UnityWebRequest.kHttpVerbPOST;

                yield return www.SendWebRequest();

                if (www.isNetworkError) {
                    // Debug.Log(www.error);
                    attempts++;
                    connectionState = "Запрос к серверу. Попытка №" + attempts.ToString();
                } else {
                    Debug.Log(www.url+" "+www.downloadHandler.text);
                    if (www.downloadHandler.text.Contains("{\"error\":\"")) {
                        Error error = parseJSON<Error>(www.downloadHandler.text);
                        connectionState = "Запрос к серверу. Попытка №" + attempts.ToString()+" Ошибка: "+error.error;
                        complete = true;
                        if (handleError) worker(www.downloadHandler.text);
                    } else {
                        worker(www.downloadHandler.text);
                        complete = true;
                    }
                }
                // Debug.Log(connectionState);
            }
            while (!complete);
            */
		}

		public IEnumerator loadRules()
		{
			if(rules.campaigns != null) yield break;
			yield return loadDataFromServer(API_URL + "rules", (string s) =>
			{
				//Debug.Log("Load Rules Complete!");
				rules = parseJSON<Rules>(s);
				return 0;
			});
		}

		public IEnumerator loadUser()
		{
			yield return loadDataFromServer(API_URL + "hello", (string s) =>
			{
				//Debug.Log("Load User complete!");

				user = parseJSON<User>(s);

				return 0;
			});
		}

		public IEnumerator getFreeBackpack()
		{
			yield return loadDataFromServer(API_URL + "getFreeBackpack", (string s) =>
			{
				FreeBackpack freeBackpack = parseJSON<FreeBackpack>(s);
				user.backpackTime = freeBackpack.backpackTime;
				user.backpacks = freeBackpack.backpacks;
				return 0;
			});
		}

		public IEnumerator updateName(string name)
		{
			return loadDataFromServer("updateName", (string s) =>
			{
				if(s == "OK") user.name = name;
				return 0;
			}, body: name);
		}

		public IEnumerator updateLang(int id)
		{
			return loadDataFromServer("updateLang", (string s) => {return 0;}, body: id.ToString());
		}

		public IEnumerator updateLangAndLoadData(int id)
		{
			yield return updateLang(id);
			yield return getLangBase();
			yield return getLang(id);
			user.lang = id;
		}

		public IEnumerator getTasks()
		{
			return loadDataFromServer(API_URL + "getTasks", (string s) =>
			{
				user.tasks = parseJSON<List<Task>>(s);
				if(!user.allTasksComplete())
				{
					user.tasksRewardGetted = false;
				}

				return 0;
			});
		}

		[NonSerialized] public TasksReward tasksReward = new TasksReward();

		public IEnumerator getTasksReward()
		{
			return loadDataFromServer(API_URL + "getTasksReward", (string s) =>
			{
				tasksReward.gold = 0;
				tasksReward.goldDelta = 0;
				tasksReward = parseJSON<TasksReward>(s);
				if(tasksReward.gold != 0) user.gold = tasksReward.gold;
				user.tasksRewardGetted = true;
				return 0;
			});
		}

		public IEnumerator getTasksReward2(int n)
		{
			return loadDataFromServer("getTasksReward2", (string s) =>
			{
				tasksReward.gold = 0;
				tasksReward.goldDelta = 0;
				tasksReward = parseJSON<TasksReward>(s);
				if(tasksReward.gold != 0) user.gold = tasksReward.gold;
				if((tasksReward.taskNumber > -1) && (tasksReward.taskNumber < 5))
				{
					user.tasks[tasksReward.taskNumber] = tasksReward.task;
				}

				return 0;
			}, body: n.ToString());
		}

		public ErrorMessage errorMessage = null;
		public bool admin = false;


		public IEnumerator login(string email, string pass)
		{
			errorMessage = null;
			admin = false;
			//return loadDataFromServer(API_URL + "login", (string s) =>
			return loadDataFromServer(API_URL + "login", (string s) =>
			{
				if(s.Contains("\"error\":"))
				{
					Debug.Log("Login error");
					errorMessage = parseJSON<ErrorMessage>(s);
					user.login = "";
					user.pass = "";
				}
				else
				{
					Debug.Log("Login OK");
					user = parseJSON<User>(s);
					deviceID = user.auth;
					if(s.Contains("\"role\":1")) admin = true;
				}

				GameManager.Instance.saveModel();
				return 0;
			}, toJSON(new AuthCommand(email, pass)), true);
		}

		public IEnumerator singup(string email, string pass)
		{
			errorMessage = null;

			return loadDataFromServer(API_URL + "singup", (string s) =>
			{
				if(s.Contains("\"error:\""))
				{
					errorMessage = parseJSON<ErrorMessage>(s);
				}
				else
				{
					user.login = email;
					user.pass = pass;
				}

				return 0;
			}, toJSON(new AuthCommand(email, pass)), true);
		}

		public Ranks ranks;

		public IEnumerator getRanks()
		{
			return loadDataFromServer(API_URL + "getRanks", (string s) =>
			{
				ranks = parseJSON<Ranks>(s);
				user.rank = ranks.rank;
				user.arenaRewardPresent = ranks.arenaRewardPresent;
				user.arenaResult = ranks.arenaResult;
				state.arenaAwardMoment = ranks.arenaAwardMoment;
				return 0;
			});
		}


		internal void selectLevel()
		{
			selectedLevel = Math.Min(user.campaigns[selectedCampaign][selectedCity][selectedArea], 2);
		}

		public void editCard(int v)
		{
			if(v == -1)
			{
				currentEditorCard = new CardType();
			}
			else
			{
				currentEditorCard = rules.cardTypes[v];
			}
		}

		public CardType getCurrentEditorCard()
		{
			return currentEditorCard;
		}

		public IEnumerator saveData(string URL, string json)
		{
			yield return loadDataFromServer(URL, (string s) => { return 0; }, json);
		}

		public IEnumerator saveCards()
		{
			return saveData(API_URL + "updateCardJson/" + currentEditorCard.id.ToString(), toJSON(currentEditorCard));
		}

		public IEnumerator saveBackpackContent()
		{
			return saveData("updateBackpackContent", toJSON(rules.backpackContent));
		}

		public IEnumerator saveLevels()
		{
			return saveData(API_URL + "updateLevels", toJSON(rules.levels));
		}

		public IEnumerator saveRulesPart()
		{
			return saveData(API_URL + "updateRulesPart",
				"{\"boosters\":" + toJSON(rules.boosters) +
				",\"premium\":" + toJSON(rules.premium) +
				",\"real\":" + toJSON(rules.real) +
				",\"battleGold\":" + toJSON(rules.battleGold) +
				",\"battleExp\":" + toJSON(rules.battleExp) +
				",\"battleRewardCard\":" + toJSON(rules.battleRewardCard) +
				",\"commonParams\":" + toJSON(rules.commonParams) +
				",\"arenaRewards\":" + toJSON(rules.arenaRewards) +
				"}"
			);
		}

        [Obsolete]
        public IEnumerator uploadFile(string path)
		{
			string URL = "http://datarotator.ru/ccg-admin/api.php";
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("action", "uploadFile");
			var bytes = File.ReadAllBytes(path);

			var form = new WWWForm();
			form.AddField("action", "uploadFile");
			form.AddBinaryData("image", bytes, "hehe.png", "image/png");

			UnityWebRequest www = UnityWebRequest.Post(URL, form);
			yield return www.SendWebRequest();

			if(www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				Debug.Log("Form upload complete!");
				Debug.Log(www.downloadHandler.text);
				uploadedResult = www.downloadHandler.text;
			}
		}

		public string uploadedResult;


		private string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public Model()
		{
			fixDeviceID();
			decks.Add(new DeckWW());
			decks.Add(new DeckWW());
			decks.Add(new DeckWW());
		}

		public void fixDeviceID()
		{
#if UNITY_IOS
				//deviceID = PreloaderScene.ID;
				deviceID = SystemInfo.deviceUniqueIdentifier;
                return;
#endif
			//#if UNITY_STANALONE || UNITY_EDITOR
			//            if(PreloaderScene.tutor)
			//                deviceID = (SteamCl.Instance.client.SteamId+(ulong)UnityEngine.Random.Range(0,1000000)).ToString();
			//            else
			//            deviceID = SteamCl.Instance.client.SteamId.ToString();
			//            return;
			//#endif
			/*
			#if STEAM	

			try 
			{
				SteamClient.Init(1252730);
			}
			catch (System.Exception e)
			{
				log(e.ToString());
			}			
			
			var ticket = SteamUser.GetAuthSessionTicket();
			log(ticket);
			deviceID = "STEAM:" + SteamClient.SteamId + ':' + Convert.ToBase64String(ticket.Data);
				
			// foreach ( var a in SteamUserStats.Achievements )
			// { 
			// 	log(a.Name);
			// }
			#else
			*/
			// android
			#if !UNITY_EDITOR
			deviceID = SystemInfo.deviceUniqueIdentifier;
			#else
			deviceID = SystemInfo.deviceUniqueIdentifier;
			
#endif

		}

		private static fsSerializer serializer = new fsSerializer();

		public static string FileContent(string path)
		{
			try
			{
				StreamReader sr = File.OpenText(path);
				string data = sr.ReadToEnd();
				sr.Close();
				return data;
			}
			catch
			{
				return "";
			}
		}

		public static void SaveJSONFile<T>(string path, T data) where T: class
		{
			fsData serializedData;
			serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
			var file = new StreamWriter(path);
			var json = fsJsonPrinter.PrettyJson(serializedData);
			file.WriteLine(json);
			file.Close();
		}

		public static string toJSON<T>(T data) where T: class
		{
			fsData serializedData;
			serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
			return fsJsonPrinter.CompressedJson(serializedData);
		}

		public static T parseJSON<T>(string fileContents) where T: class
		{
			var data = fsJsonParser.Parse(fileContents);
			object deserialized = null;
			serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
			return deserialized as T;
		}

		[NonSerialized] public List<CardWW> dMe = new List<CardWW>();

		[NonSerialized] public List<CardWW> dEnemy = new List<CardWW>();

		[NonSerialized] public List<ScriptAction> script;

		public bool battle_test = false;

		public IEnumerator battle(string idsS, string data)
		{
			battleScript = null;
			yield return preload();

			yield return loadDataFromServer("battle", (string s) =>
			{
				Debug.Log("Battle Complete");
				battleScript = parseJSON<Battle>(s);
				log(s);
				
				user.applyBattleResult(battleScript);
				upLevelReward = battleScript.profileResult.reward;
				state.arenaDropMoment = battleScript.arenaDropMoment;
				return 0;
			}, idsS + '/' + data);
		}

		public IEnumerator battle_campaign(string idsS)
		{
			battleScript = null;
			yield return preload();

			yield return loadDataFromServer(
				// deck always = 0
				"battle_campaign", (string s) =>
				{
					Debug.Log("Battle Complete");
					battleScript = parseJSON<Battle>(s);
					user.applyBattleResult(battleScript);
					upLevelReward = battleScript.profileResult.reward;
					if(!battle_test)
						user.campaigns[selectedCampaign][selectedCity][selectedArea] = Mathf.Max(battleScript.stars,
							user.campaigns[selectedCampaign][selectedCity][selectedArea]);
					return 0;
				}, selectedCampaign + "/" + selectedCity + "/" + selectedArea + "/" + idsS); // body
		}

		public IEnumerator preload()
		{
			if(!syncronized)
			{
				yield return loadServerState();
				yield return loadRules();
				yield return loadUser();
				// loadLang(user.lang);
				// if(user.langV > currentLangVersion)
				{
					yield return getLangBase();
					yield return getLang(user.lang);
				}

				syncronized = true;
			}
		}

		int unixTimestampLoad;
		public int time;

		public IEnumerator loadServerState()
		{
			yield return loadDataFromServer(API_URL + "state", (string s) =>
			{
				state = parseJSON<ServerState>(s);
				unixTimestampLoad = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				time = state.servertime;
				//Debug.Log("deltaTime: " + (unixTimestampLoad - state.servertime).ToString());
				return 0;
			});
		}


		[NonSerialized] public BackpackResult backpackResult;

		public int selectedCampaign = 0;
		public int selectedCity = 0;
		public int selectedArea = 0;
		public int selectedLevel = 0;

		[NonSerialized] public List<Level> upLevelReward;

		public IEnumerator openBackpack()
		{
			backpackResult = null;

			if(user.backpacks <= 0) yield break;

			yield return loadDataFromServer(API_URL + "openBackpack", (string s) =>
			{
				backpackResult = parseJSON<BackpackResult>(s);
				if(backpackResult.result == -1) return 0;
				user.applyResult(backpackResult);
				upLevelReward = backpackResult.reward;
				return 0;
			});
		}


		public IEnumerator saveDeck(string data)
		{
			Debug.Log("savedeck id = " + userId.ToString());
			yield return loadDataFromServer("saveDeck/0", (string s) =>
			{
				Debug.Log("save complete!");
				return 0;
			}, body: data);

			/*
			UnityWebRequest www = new UnityWebRequest(URL);
            www.SetRequestHeader("Authorization", deviceID);
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.method = UnityWebRequest.kHttpVerbPOST;
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log(www.error);
                user.decks[0] = parseJSON<DeckV2>(www.downloadHandler.text);
            } else {
                Debug.Log("save complete!");
                Debug.Log(www.downloadHandler.text);
            }
			*/
		}

		public IEnumerator upgradeCard(DeckPart deckPart, int id, AudioSource audioSource, AudioClip clip)
		{
			List<CardV3> list = null;
			if(deckPart == DeckPart.cards) list = user.decks[0].cards;
			if(deckPart == DeckPart.reserve) list = user.decks[0].reserve;
			CardV3 card = list[id];
			int cost = rules.cardTypes[card.id].upgradeCost[card.level - 1].gold;
//#if !UNITY_EDITOR
			if(cost > user.gold) yield break;
//#endif
			string body = ((deckPart == DeckPart.cards)? "cards": "reserve") + "/" + id.ToString();
			yield return loadDataFromServer("upgrade", (string s) =>
			{
//#if !UNITY_EDITOR
				if(s == "OK")
				{
					user.gold -= rules.cardTypes[card.id].upgradeCost[card.level - 1].gold;
					list[id].level++;
					GameManager.Instance.PlaySound(audioSource, clip);
				}
//#else
				//user.gold -= 555;
				//    list[id].level++;
				//GameManager.Instance.PlaySound(audioSource, clip);
//#endif

				return 0;
			}, body);
		}

		public CommonResult couponEditorResult;

		public IEnumerator createCoupon(Coupon coupon)
		{
			yield return loadDataFromServer("createCoupon", (string s) =>
			{
				couponEditorResult = parseJSON<CommonResult>(s);
				if(couponEditorResult.result == "OK")
				{
					coupons[coupon.id] = parseJSON<Coupon>(toJSON(coupon));
				}

				return 0;
			}, toJSON(coupon));
		}

		public IEnumerator removeCoupon(string couponeID)
		{
			yield return loadDataFromServer("removeCoupon", (string s) =>
			{
				couponEditorResult = parseJSON<CommonResult>(s);
				if(couponEditorResult.result == "OK")
				{
					coupons.Remove(couponeID);
				}

				return 0;
			}, body: couponeID);
		}

		public IEnumerator updateCoupon(Coupon coupon)
		{
			yield return loadDataFromServer("updateCoupon", (string s) =>
			{
				couponEditorResult = parseJSON<CommonResult>(s);
				if(couponEditorResult.result == "OK")
				{
					coupons[coupon.id] = parseJSON<Coupon>(toJSON(coupon));
				}

				return 0;
			}, toJSON(coupon));
		}

		public Dictionary<string, Coupon> coupons;

		public IEnumerator showCoupons()
		{
			yield return loadDataFromServer("showCoupons", (string s) =>
			{
				coupons = parseJSON<Dictionary<string, Coupon>>(s);
				return 0;
			});
		}

		public CouponResult couponResult;

		public IEnumerator useCoupon(string coupone)
		{
			yield return loadDataFromServer("useCoupon", (string s) =>
			{
				couponResult = parseJSON<CouponResult>(s);
				user.gold = couponResult.gold;
				return 0;
			}, body: coupone);
		}

		public ArenaRewardResult arenaRewardResult;

		public IEnumerator getArenaReward()
		{
			string url = API_URL + "getArenaReward";
			yield return loadDataFromServer(url, (string s) =>
			{
				arenaRewardResult = parseJSON<ArenaRewardResult>(s);
				user.gold = arenaRewardResult.gold;
				user.exp = arenaRewardResult.exp;
				user.arenaResult.gold = 0;
				user.arenaResult.exp = 0;
				user.arenaRewardPresent = false;
				return 0;
			});
		}

		public IEnumerator getRole()
		{
			string url = API_URL + "getRole";
			yield return loadDataFromServer(url, (string s) =>
			{
				if(s == "1") admin = true;
				if(s == "0") admin = false;
				return 0;
			});
		}

		public bool canClickCityButton(int n)
		{
			if(n <= 0) return true;
			if(user.campaigns[selectedCampaign][n][0] > 0) return true;
			int prevAreas = user.campaigns[selectedCampaign][n - 1].Count - 1;
			if(user.campaigns[selectedCampaign][n - 1][prevAreas] > 1) return true;
			return false;
		}

		public bool canClickAreaButton(int n)
		{
			if(getTutorialScene() != -1) return true;
			if(user.campaigns[selectedCampaign][selectedCity][n] == 3) return false;
			if((n == 0) && (selectedCity == 0)) return true;
			if(user.campaigns[selectedCampaign][selectedCity][n] > 0) return true;
			if(n > 0)
			{
				if(user.campaigns[selectedCampaign][selectedCity][n - 1] > 1) return true;
				else return false;
			}
			else
			{
				int prevAreas = user.campaigns[selectedCampaign][selectedCity - 1].Count - 1;
				if(user.campaigns[selectedCampaign][selectedCity - 1][prevAreas] > 1) return true;
			}

			return false;
		}

		public int deckSize()
		{
			if(user.level >= 8) return 5;
			if(user.level >= 5) return 4;
			if(user.level >= 3) return 3;
			return 2;
		}

		public int openCellLevel()
		{
			if(user.level < 3) return 3;
			if(user.level < 5) return 5;
			if(user.level < 8) return 8;
			return 0;
		}

		public bool canUpgradeCard(int n)
		{
			return ((n != 0) && (n != 6) && (n != 12));
		}

		public const int OUT_MESSAGE = 0;
		public const int IN_MESSAGE = 1;


		public User userEdit;
		public int userEditID;

		public IEnumerator getUser(int userID)
		{
			string url = API_URL + "getUser/" + userID.ToString();
			yield return loadDataFromServer(url, (string s) =>
			{
				userEditID = userID;
				userEdit = parseJSON<User>(s);
				return 0;
			});
		}

		public IEnumerator deleteUser(int userID)
		{
			string url = API_URL + "deleteUser/" + userID.ToString();
			yield return loadDataFromServer(url, (string s) => {return 0;});
		}

		public IEnumerator updateUser(int userID, string data)
		{
			string url = API_URL + "updateUser/" + userID.ToString();
			yield return loadDataFromServer(url, (string s) => {return 0;}, toJSON(userEdit));
		}

		public List<UserPreview> searchResult;

		public IEnumerator findUsersByText(string text)
		{
			string url = API_URL + "findUsersByText/" + text;
			yield return loadDataFromServer(url, (string s) =>
			{
				searchResult = parseJSON<List<UserPreview>>(s);
				return 0;
			});
		}

		public IEnumerator topEloUsers()
		{
			string url = API_URL + "topEloUsers";
			yield return loadDataFromServer(url, (string s) =>
			{
				searchResult = parseJSON<List<UserPreview>>(s);
				return 0;
			});
		}

		public IEnumerator updateEloRating(List<EloSet> eloSets)
		{
			string url = API_URL + "updateEloRating";
			yield return loadDataFromServer(url, (string s) => {return 0;}, toJSON(eloSets));
		}

		public string setEloForAllResult;

		public IEnumerator setEloForAll(float elo)
		{
			string url = API_URL + "setEloForAll/" + elo.ToString();
			yield return loadDataFromServer(url, (string s) =>
			{
				setEloForAllResult = parseJSON<string>(s);
				return 0;
			});
		}

		public List<LogRecord> logRecords;

		public IEnumerator getUserStory(int userID)
		{
			string url = API_URL + "getUserStory";
			yield return loadDataFromServer(url, (string s) =>
			{
				logRecords = parseJSON<List<LogRecord>>(s);
				return 0;
			});
		}

		public bool sendAdminMessageComplete = false;

		public IEnumerator sendAdminMessage(AdminMessage adminMessage)
		{
			sendAdminMessageComplete = false;
			yield return loadDataFromServer("sendAdminMessage", (string s) =>
			{
				if(s == "OK") sendAdminMessageComplete = true;
				return 0;
			}, toJSON(adminMessage));
		}

		public IEnumerator sendAdminPersonelMessage(int id, string message, bool important)
		{
			sendAdminMessageComplete = false;
			AdminPersonelMessage adminPersonelMessage = new AdminPersonelMessage();
			adminPersonelMessage.id = id;
			adminPersonelMessage.message = message;
			adminPersonelMessage.important = important;
			string url = API_URL + "sendAdminPersonelMessage";
			yield return loadDataFromServer(url, (string s) =>
			{
				if(s == "\"OK\"") sendAdminMessageComplete = true;
				return 0;
			}, toJSON(adminPersonelMessage));
		}

		public List<Chat> chats;

		public IEnumerator getChats()
		{
			string url = API_URL + "getChats";
			yield return loadDataFromServer(url, (string s) =>
			{
				chats = parseJSON<List<Chat>>(s);
				return 0;
			});
		}

		bool haveNewMessages = false;

		public IEnumerator haveMessages()
		{
			string url = API_URL + "haveMessages";
			yield return loadDataFromServer(url, (string s) =>
			{
				haveNewMessages = (s == "1");
				return 0;
			});
		}

		public IEnumerator sendMessage(int chatID, string message)
		{
			MessagePack messagePack = new MessagePack();
			messagePack.chatID = chatID;
			messagePack.id = 0;
			messagePack.message = message;
			yield return loadDataFromServer("sendMessage", (string s) => {return 0;}, toJSON(messagePack));
		}

		public IEnumerator removeMessage(int chatID, ChatMessage chatMessage)
		{
			string url = API_URL + "removeMessage/" + chatID.ToString() + "/" + chatMessage.userID.ToString() + "/" +
			             chatMessage.dt.ToString();
			yield return loadDataFromServer(url, (string s) => {return 0;});
		}

		public IEnumerator banInChats(int userID, int time)
		{
			string url = API_URL + "banInChats/" + userID.ToString() + "/" + time.ToString();
			yield return loadDataFromServer(url, (string s) => {return 0;});
		}

		public string adminMessage = "";
		public bool adminMessageImportant = false;

		public IEnumerator getAdminMessage()
		{
			adminMessage = "";
			string url = API_URL + "getAdminMessage";
			yield return loadDataFromServer(url, (string s) =>
			{
				adminMessage = s;
				return 0;
			});
		}

		public IEnumerator getAdminMessageV2()
		{
			adminMessage = "";
			yield return loadDataFromServer("getAdminMessageV2", (string s) =>
			{
				if(s == " ")
				{
					adminMessage = "";
					return 0;
				}

				adminMessageImportant = (s[s.Length - 1] == '1');
				if((s[s.Length - 1] == '0') || (s[s.Length - 1] == '1')) adminMessage = s.Remove(s.Length - 1);
				else adminMessage = s;
				return 0;
			});
		}

		public const int GOOD_BACKPACK = 0;
		public const int GOOD_PREMIUM = 1;
		public const int GOOD_CARD_HERO2 = 2;
		public const int GOOD_CARD_HERO3 = 3;

		[NonSerialized] public BuyResult buyResult;

		public const int GOOD_GOLD1000 = 0;
		public const int GOOD_GOLD350 = 1;
		public const int GOOD_GOLD50 = 2;
		public const int GOOD_BACKPACK3 = 3;
		public const int GOOD_BACKPACK2 = 4;
		public const int GOOD_BACKPACK1 = 5;
		public const int GOOD_VIP30 = 6;
		public const int GOOD_VIP15 = 7;
		public const int GOOD_VIP3 = 8;
		public const int GOOD_LEADER2 = 9;
		public const int GOOD_LEADER3 = 10;
		public const int GOOD_CARD1 = 11;
		public const int GOOD_CARD2 = 12;
		public const int GOOD_CARD3 = 13;
		public const int GOOD_CARD4 = 14;
		public const int GOOD_CARD5 = 15;
		public const int GOOD_DISABLE_ADS_LITTLE = 16;
		public const int GOOD_DISABLE_ADS_BIG = 17;
		public const int GOOD_STARTER_PACK = 18;
		public const int GOOD_GOLD_PACK4 = 19;
		public const int GOOD_GOLD_PACK5 = 20;
		public const int GOOD_GOLD_PACK6 = 21;
		public const int GOOD_VIP_V4 = 22;
		public const int GOOD_VIP_V5 = 23;
		public const int GOOD_VIP_V6 = 24;
		public const int GOOD_BACKPACKS_X3 = 25;
		public const int GOOD_BACKPACKS_X6 = 26;
		public const int GOOD_BACKPACKS_X9 = 27;


		public IEnumerator buy(int goodID, string json = "", string sig = "")
		{
			if((goodID == GOOD_LEADER2) && (user.decks[0].cards[0].level >= 2)) yield break;
			if((goodID == GOOD_LEADER3) && (user.decks[0].cards[0].level >= 3)) yield break;
			backpackResult = null;
			yield return loadDataFromServer("buy", (string s) =>
			{
				Debug.LogWarning(s);
				buyResult = parseJSON<BuyResult>(s);
				user.gold = buyResult.gold;
				user.proTimeout = buyResult.proTimeout;
				switch(buyResult.result)
				{
					case GOOD_GOLD1000:
						user.gold = buyResult.gold;
						break;
					case GOOD_GOLD350:
						user.gold = buyResult.gold;
						break;
					case GOOD_GOLD50:
						user.gold = buyResult.gold;
						break;
					case GOOD_GOLD_PACK4:
						user.gold = buyResult.gold;
						break;
					case GOOD_GOLD_PACK5:
						user.gold = buyResult.gold;
						break;
					case GOOD_GOLD_PACK6:
						user.gold = buyResult.gold;
						break;
					case GOOD_BACKPACK3:
						backpackResult = buyResult.backpackResult;
						user.applyResult(backpackResult);
						upLevelReward = backpackResult.reward;
						break;
					case GOOD_BACKPACK2:
						backpackResult = buyResult.backpackResult;
						user.applyResult(backpackResult);
						upLevelReward = backpackResult.reward;
						break;
					case GOOD_BACKPACK1:
						backpackResult = buyResult.backpackResult;
						user.applyResult(backpackResult);
						upLevelReward = backpackResult.reward;
						break;
					case GOOD_BACKPACKS_X3:
					case GOOD_BACKPACKS_X6:
					case GOOD_BACKPACKS_X9:
						user.backpacks = buyResult.backpackResult.backpacks;
						break;
					case GOOD_VIP30:
						break;
					case GOOD_VIP15:
						break;
					case GOOD_VIP3:
						break;
					case GOOD_LEADER2:
						user.decks[0].cards[0].level = 2;
						break;
					case GOOD_LEADER3:
						user.decks[0].cards[0].level = 3;
						break;
					case GOOD_CARD1:
						user.decks[0].reserve.Add(new CardV3(1, 3, 1));
						break;
					case GOOD_CARD2:
						user.decks[0].reserve.Add(new CardV3(2, 3, 1));
						break;
					case GOOD_CARD3:
						user.decks[0].reserve.Add(new CardV3(3, 3, 1));
						break;
					case GOOD_CARD4:
						user.decks[0].reserve.Add(new CardV3(4, 3, 1));
						break;
					case GOOD_CARD5:
						user.decks[0].reserve.Add(new CardV3(5, 3, 1));
						break;
					case GOOD_DISABLE_ADS_LITTLE:
						PlayerPrefs.SetString("ads", (DateTime.Now + new TimeSpan(1, 0, 0, 0)).ToString());
						break;
					case GOOD_DISABLE_ADS_BIG:
						PlayerPrefs.SetString("ads", (DateTime.Now + new TimeSpan(30, 0, 0, 0)).ToString());
						break;
				}


				return 0;
			}, BillingData.text(json, sig, goodID));
			
			//FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase);
		}

		[NonSerialized] public SellResult sellResult;

		public IEnumerator sell(int deck, DeckPart deckPart, int id)
		{
			string body = deck.ToString() + "/" + ((int)deckPart).ToString() + "/" + id.ToString();
			yield return loadDataFromServer("sell", (string s) =>
			{
				sellResult = parseJSON<SellResult>(s);
				user.gold = sellResult.gold;
				if(sellResult.part == (int)DeckPart.cards) user.decks[sellResult.deck].cards.RemoveAt(sellResult.id);
				if(sellResult.part == (int)DeckPart.reserve) user.decks[sellResult.deck].reserve.RemoveAt(sellResult.id);
				return 0;
			}, body);
		}

		[NonSerialized] public StarterPackResult starterPackResult;

		public IEnumerator buyStarterPack(string json, string sig)
		{
			string url = API_URL + "starterPack";
			yield return loadDataFromServer(url, (string s) =>
			{
				starterPackResult = parseJSON<StarterPackResult>(s);
				if(starterPackResult.result == -1) return 0;
				user.applyResult(starterPackResult.mainContent);
				user.applyResult(starterPackResult.additionContent);
				user.adsDisableTimeout = starterPackResult.adsDisableTimeout;
				upLevelReward = starterPackResult.mainContent.reward;
				foreach(Level l in starterPackResult.additionContent.reward)
				{
					upLevelReward.Add(l);
				}

				return 0;
			}, BillingData.text(json, sig));
		}

		[NonSerialized] public Chat chat;

		public IEnumerator getChat(int chatID)
		{
			string url = "getChat";
			yield return loadDataFromServer(url, (string s) =>
			{
				chat = parseJSON<Chat>(s);
				return 0;
			}, body: chatID.ToString());
		}

		[NonSerialized] BackpackResult doubleResult;

		public IEnumerator DoubleReward()
		{
			Debug.Log("111111111111111");
			yield return loadDataFromServer("doubleReward", (string s) =>
			{
				Debug.Log("222222222222222");
				doubleResult = parseJSON<BackpackResult>(s);
				if(doubleResult.result == -1) return 0;
				user.applyResult(doubleResult);
				upLevelReward = doubleResult.reward;
				return 0;
			});
		}

		public IEnumerator activeDeck(int i)
		{
			yield return loadDataFromServer("doubleReward", (string s) =>
			{
				user.activeDeck = int.Parse(s);
				return 0;
			}, body: i.ToString());
		}

		[NonSerialized] public List<TaskGeneralProcess> tasksGeneral;
		[NonSerialized] public bool haveGeneralReward = false;

		void updateHaveGeneralReward()
		{
			haveGeneralReward = false;
			foreach(TaskGeneralProcess taskGeneralProcess in tasksGeneral)
			{
				if(taskGeneralProcess.state == TaskGeneralProcess.TASK_GENERAL_STATE_HAVE_REWARD)
				{
					haveGeneralReward = true;
					break;
				}
			}
		}

		public IEnumerator getTasksGeneral()
		{
			string url = API_URL + "getTasksGeneral";
			yield return loadDataFromServer(url, (string s) =>
			{
				tasksGeneral = parseJSON<List<TaskGeneralProcess>>(s);
				updateHaveGeneralReward();
				return 0;
			});
		}

		[NonSerialized] public TasksReward taskGeneralReward;

		public IEnumerator getTaskGeneralReward(int id)
		{
			yield return loadDataFromServer("getTaskGeneralReward", (string s) =>
			{
				taskGeneralReward = parseJSON<TasksReward>(s);
				user.gold = taskGeneralReward.gold;
				if(taskGeneralReward.goldDelta > 0) tasksGeneral[id].state = TaskGeneralProcess.TASK_GENERAL_STATE_COMPLETE;
				updateHaveGeneralReward();
				return 0;
			}, body: id.ToString());
		}


		[NonSerialized] public Dictionary<string, int> langBase;

		[NonSerialized] public Dictionary<int, string> langData;

		public static void SaveFile(string path, string data)
		{
			var file = new StreamWriter(path);
			file.WriteLine(data);
			file.Close();
		}

		public IEnumerator getLangBase()
		{
			string url = API_URL + "lang/base";
			yield return loadDataFromServer(url, (string s) =>
			{
				Dictionary<string, string> langBaseString = parseJSON<Dictionary<string, string>>(s);
				if(langBaseString.ContainsKey("error")) return 0;
				// SaveFile(Application.persistentDataPath + "/langBase.json", s);
				loadLangBase(s);
				return 0;
			});
		}

		public void loadLangBase(string data)
		{
			langBase = new Dictionary<string, int>();
			// string data = FileContent(Application.persistentDataPath + "/langBase.json");
			if(data == "") return;
			Dictionary<string, string> langBaseString = parseJSON<Dictionary<string, string>>(data);
			foreach(KeyValuePair<string, string> kv in langBaseString)
			{
				if(kv.Key != "version")
				{
					if(!langBase.ContainsKey(kv.Value))
					{
						langBase.Add(kv.Value, int.Parse(kv.Key));
					}
				}
			}
		}

		public IEnumerator getLang(int id)
		{
			yield return loadDataFromServer("lang/get", (string s) =>
			{
				Dictionary<string, string> langString = parseJSON<Dictionary<string, string>>(s);
				if(langString.ContainsKey("error")) return 0;
				// SaveFile(Application.persistentDataPath + "/lang" + id.ToString() + ".json", s);
				loadLang(id, s);
				user.langV = currentLangVersion;
				return 0;
			}, body: id.ToString());
		}

		[NonSerialized] int currentLangVersion = -1;

		public void loadLang(int id, string data)
		{
			langData = new Dictionary<int, string>();
			// string data = FileContent(Application.persistentDataPath + "/lang" + id.ToString() + ".json");
			if(data == "") return;
			Dictionary<string, string> langBaseString = parseJSON<Dictionary<string, string>>(data);
			foreach(KeyValuePair<string, string> kv in langBaseString)
			{
				if(kv.Key != "version")
				{
					langData.Add(int.Parse(kv.Key), kv.Value);
				}
				else
				{
					currentLangVersion = int.Parse(kv.Value);
				}
			}

			foreach(TranslatorScript ts in GameObject.FindObjectsOfType<TranslatorScript>())
			{
				ts.UpdateText();
				ts.UpdateFont();
			}
		}

		public IEnumerator getLangUpdate()
		{
			string url = API_URL + "/lang/update";
			yield return loadDataFromServer(url, (string s) => {return 0;});
		}

		public int getTextId(string text)
		{
			return langBase.ContainsKey(text)? langBase[text]: -1;
		}

		public string getText(int id)
		{
			return langData[id];
		}

		public bool haveText(int id)
		{
			return langData.ContainsKey(id);
		}

		[NonSerialized] public BackpackResult starterPackContent;

		public IEnumerator getStarterPackContent()
		{
			string url = API_URL + "/getStarterPackContent";
			yield return loadDataFromServer(url, (string s) =>
			{
				starterPackContent = parseJSON<BackpackResult>(s);
				if(starterPackContent.result == -1) return 0;
				user.applyResult(starterPackContent);
				upLevelReward = starterPackContent.reward;
				return 0;
			});
		}
	}

	public class ScriptAction
	{
		public int id;
		public int actor1;
		public int id1;
		public int actor2;
		public int id2;
		public int hp2;
	}

	[Serializable]
	public class ScriptActionV2
	{
		[SerializeField] int id;
	}

	[Serializable]
	public class DeckWW
	{
		public string title = "";
		public List<CardWW> cards = new List<CardWW>();
	}

	[Serializable]
	public class CardWW
	{
		public int id = 0;
		public int attack = 2;
		public int hp = 5;
	}

	[Serializable]
	public class UserR
	{
		[SerializeField] public int gold = 0;
		[SerializeField] public int backpacks = 0;
		[SerializeField] public int exp = 0;
		[SerializeField] public int backpackTime = 0;
		[SerializeField] public int remainBattles = 5;
		[SerializeField] public int remainBackpacks = 1;
		[SerializeField] public string login;
		[SerializeField] public string pass;
		[SerializeField] public string auth;
		[SerializeField] public string name;
		[SerializeField] public List<DeckV2> decks;
		[SerializeField] public List<List<List<int>>> campaigns;
	}

	[Serializable]
	public class PValue
	{
		[SerializeField] public int p;
		[SerializeField] public int value;

		public PValue()
		{
		}

		public PValue(int value, int p)
		{
			this.p = p;
			this.value = value;
		}
	};

	[Serializable]
	public class BackpackContent
	{
		[SerializeField] public int cost;
		[SerializeField] public List<PValue> premium;
		[SerializeField] public List<PValue> exp;
		[SerializeField] public List<PValue> cards;
		[SerializeField] public List<PValue> gold;
	};

	[Serializable]
	public class Booster
	{
		[SerializeField] public int cost;
		[SerializeField] public int size;
	};

	[Serializable]
	public class Premium
	{
		[SerializeField] public int cost;
		[SerializeField] public int time;
		[SerializeField] public int extraExp;
		[SerializeField] public int extraGold;
	};

	[Serializable]
	public class Real
	{
		[SerializeField] public int gold;
		[SerializeField] public int rubles;
		[SerializeField] public float euros;
	};

	[Serializable]
	public class BattleGold
	{
		[SerializeField] public int battles;
		[SerializeField] public int gold;
	};

	[Serializable]
	public class BattleExp
	{
		[SerializeField] public int win;
		[SerializeField] public int fail;
	};

	[Serializable]
	public class BattleRewardCard
	{
		[SerializeField] public int battleWins;
	};

	[Serializable]
	public class Level
	{
		[SerializeField] public int number;
		[SerializeField] public int exp;
		[SerializeField] public string rewardType;
		[SerializeField] public int rewardValue;
	};

	[Serializable]
	public class CommonParams
	{
		[SerializeField] public int freeBackpackTimer;
		[SerializeField] public int continueArenaCost;
		[SerializeField] public int maxCouponsPerDay;
		[SerializeField] public int arenaRewardWait;
		[SerializeField] public int premiumCardBonusFarm = 5;
		[SerializeField] public int premiumCardBonusExp = 10;
		[SerializeField] public int chatMinimumDelta = 10;
		[SerializeField] public int chatSize = 50;
		[SerializeField] public int chatMinLevel = 0;
		[SerializeField] public bool chatBlocked = false;
		[SerializeField] public int maxMessageLength = 500;
		[SerializeField] public int maxWordLength = 25;
		[SerializeField] public int chatUpdateTime = 5;
		[SerializeField] public int postUpdateTime = 10;
	}

	[Serializable]
	public class ArenaReward
	{
		[SerializeField] public int from;
		[SerializeField] public int to;
		[SerializeField] public int gold;
		[SerializeField] public int exp;

		public bool onePlace()
		{
			return from == to;
		}

		public bool contain(int id)
		{
			return (id >= from) && (id <= to);
		}
	};

	[Serializable]
	public class StarterPack
	{
		[SerializeField] public int gold;
		[SerializeField] public int exp;
		[SerializeField] public int premiumCards;
		[SerializeField] public int card2level;
		[SerializeField] public int card3level;
		[SerializeField] public int disableAds;
		[SerializeField] public int premiumDays;
		[SerializeField] public int heavyBackbacks;
	}

	[Serializable]
	public class Rules
	{
		[SerializeField] public List<CardType> cardTypes;
		[SerializeField] CampaignArea default_mission;
		[SerializeField] UserR default_user;
		[SerializeField] public Campaigns campaigns;
		[SerializeField] public List<BackpackContent> backpackContent;
		[SerializeField] public List<Booster> boosters;
		[SerializeField] public List<Premium> premium;
		[SerializeField] public List<Real> real;
		[SerializeField] public BattleGold battleGold;
		[SerializeField] public BattleExp battleExp;
		[SerializeField] public BattleRewardCard battleRewardCard;
		[SerializeField] public List<Level> levels;
		[SerializeField] public CommonParams commonParams;
		[SerializeField] public List<ArenaReward> arenaRewards;
		[SerializeField] public List<int> admins;
		[SerializeField] public List<Good> goods;
		[SerializeField] public StarterPack starterPack;
	}

	[Serializable]
	public class Error
	{
		[SerializeField] public string error;
	}

	public enum DeckPart
	{
		cards,
		reserve
	}

	[Serializable]
	public class PlayerRank
	{
		[SerializeField] public string name;
	}

	[Serializable]
	public class ArenaResult
	{
		[SerializeField] public int gold;
		[SerializeField] public int exp;
		[SerializeField] public int timeout;

		public bool empty()
		{
			return (gold == 0) && (exp == 0);
		}
	};


	[Serializable]
	public class Ranks
	{
		[SerializeField] public List<PlayerRank> players;
		[SerializeField] public int rank;
		[SerializeField] public bool arenaRewardPresent;
		[SerializeField] public ArenaResult arenaResult;
		[SerializeField] public int arenaAwardMoment;
	}

	[Serializable]
	public class ErrorMessage
	{
		[SerializeField] public string error;
	}

	[Serializable]
	public class AuthCommand
	{
		[SerializeField] public string name;
		[SerializeField] public string pass;

		public AuthCommand()
		{
		}

		public AuthCommand(string _name, string _pass)
		{
			name = _name;
			pass = _pass;
		}
	}

	[Serializable]
	public class TasksReward
	{
		[SerializeField] public int goldDelta;
		[SerializeField] public int gold;
		[SerializeField] public int taskNumber = -1;
		[SerializeField] public Task task;
	}

	[Serializable]
	public class Coupon
	{
		[SerializeField] public string id;
		[SerializeField] public int type;
		[SerializeField] public bool used;
		[SerializeField] public int start;
		[SerializeField] public int finish;
		[SerializeField] public int gold;
	}

	[Serializable]
	public class CouponResult
	{
		[SerializeField] public string result;
		[SerializeField] public int gold = 0;
		[SerializeField] public int goldDelta = 0;
	}

	[Serializable]
	public class CommonResult
	{
		[SerializeField] public string result;
	}

	[Serializable]
	public class ArenaRewardResult
	{
		[SerializeField] public int gold;
		[SerializeField] public int goldDelta;
		[SerializeField] public int exp;
		[SerializeField] public int expDelta;

		public bool empty()
		{
			return (goldDelta == 0) && (expDelta == 0);
		}
	}

	[Serializable]
	public class ChatMessage
	{
		[SerializeField] public int dt;
		[SerializeField] public int userID;
		[SerializeField] public string author;
		[SerializeField] public string text;
		[SerializeField] public bool readed;
		[SerializeField] public int role;

		public string getText()
		{
			string colorStart = "";
			string colorFinish = "";
			switch(role)
			{
				case 0:
					colorStart = "";
					colorFinish = "";
					break;
				case 1:
					colorStart = "<color=#FF0000>";
					colorFinish = "</color>";
					break;
				case 2:
					colorStart = "<color=#0000FF>";
					colorFinish = "</color>";
					break;
			}

			return colorStart + author + colorFinish + ": " + text;
		}
	}

	[Serializable]
	public class Chat
	{
		[SerializeField] public int id = 0;
		[SerializeField] public bool canReply = true;
		[SerializeField] public string[] name;
		[SerializeField] public bool newMessages = false;
		[SerializeField] public ChatMessage[] messages;
	}

	[Serializable]
	public class UserPreview
	{
		[SerializeField] public int id;
		[SerializeField] public string name;
		[SerializeField] public string email;
		[SerializeField] public float elo;
		[SerializeField] public int gold;
	}

	[Serializable]
	public class EloSet
	{
		[SerializeField] public int id;
		[SerializeField] public float elo;
	}

	[Serializable]
	public class LogRecord
	{
		[SerializeField] public int dt;
		[SerializeField] public int id;
		[SerializeField] public string message;
		[SerializeField] public string var1;
		[SerializeField] public string var2;
		[SerializeField] public string var3;
	}

	[Serializable]
	public class Good
	{
		[SerializeField] public int id = 0;
		[SerializeField] public List<string> title;
		[SerializeField] public int count = 1;
		[SerializeField] public int cost = 100;
		[SerializeField] public int discount = 0;
		[SerializeField] public int color = 0;
		[SerializeField] public bool real = false;
	}

	[Serializable]
	public class BuyResult
	{
		[SerializeField] public int result = -1;
		[SerializeField] public int gold = 0;
		[SerializeField] public int goldDelta = 0;
		[SerializeField] public int proTimeout = 0;
		[SerializeField] public int adsDisableTimeout = 0;
		[SerializeField] public BackpackResult backpackResult;
	};

	[Serializable]
	public class SellResult
	{
		[SerializeField] public int result;
		[SerializeField] public int gold;
		[SerializeField] public int goldDelta;
		[SerializeField] public int deck;
		[SerializeField] public int part;
		[SerializeField] public int id;
	}

	[Serializable]
	public class MessagePack
	{
		[SerializeField] public int chatID;
		[SerializeField] public int id;
		[SerializeField] public string message;
	}

	[Serializable]
	public class AdminMessage
	{
		[SerializeField] public string[] message;
		[SerializeField] public bool important;
	}

	[Serializable]
	public class AdminPersonelMessage
	{
		[SerializeField] public int id;
		[SerializeField] public string message;
		[SerializeField] public bool important;
	}

	[Serializable]
	public class AdditionalReward
	{
		[SerializeField] public int exp = 0;
		[SerializeField] public int gold = 0;
	}

	[Serializable]
	public class RecieveAdminMessage
	{
		[SerializeField] public int id;
		[SerializeField] public string message;
	}

	[Serializable]
	public class StarterPackResult
	{
		[SerializeField] public int result = 0;
		[SerializeField] public BackpackResult mainContent;
		[SerializeField] public BackpackResult additionContent;
		[SerializeField] public int adsDisableTimeout;
	};

	[Serializable]
	public class BillingData
	{
		[SerializeField] public string data;
		[SerializeField] public string signature;
		[SerializeField] public int goodID;

		public static string text(string d, string s, int g = -1)
		{
			BillingData billingData = new BillingData();
			billingData.data = d;
			billingData.signature = s;
			billingData.goodID = g;
			return Model.toJSON(billingData);
		}
	};
}