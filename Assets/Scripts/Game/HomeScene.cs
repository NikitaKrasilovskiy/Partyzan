using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Collections;
using CCGKit;

#if STEAM
using Steamworks.Data;
#endif

public class HomeScene : BaseScene
{
	public TextMeshProUGUI goldText;
	public TextMeshProUGUI backpacksText;
	public AudioSource music;
	public NoBattlesTodayScript noBattlesToday;
	public GameObject attention;
	public PreloaderAmination preloader;
	public TranslatorScript continueCost;
	public GameObject noMoney;
	public TasksWindow tasksWindow;
	public SettingsWindowScript settingsWindow;
	public ProfileWindowScript profileWindow;
	public RatingWindowScript ratingWindow;
	public BackPuckController openBackpackWindow;
	public WayWindowScript wayWindow;
	public ShopWindowScript shopWindow;
	public ShopWindowScript shopWindowPrefab;
	public FreeBackpackWindow freeBackpackWindow;
	public GameObject buyComplete;
	public PostWindowScript postWindow;
	public BuyStarterPackWindow buyStarterPackWindow;
	public DeckSelectorWindow deckSelectorWindow;
	public TasksGeneralWindow tasksGeneralWindow;
	public CardsUpgrade cardsUpgrade;
	public TutorialScript tutorialWindow;
	public Texture2D cursor;

	public static bool goldFight = false;

	public BuyStarterPackWindow nstBag;

	public string url;

	private static bool attentionShowed = false;

	private Model model;

	private void Awake()
	{
		Assert.IsNotNull(goldText);
		Assert.IsNotNull(backpacksText);
		//Model.API_URL = url;
		if (GameManager.Instance.music != null)
		{ if (!GameManager.Instance.music.isPlaying) GameManager.Instance.music.Play(); }
	}

	private IEnumerator Start()
	{
		if (tutorialWindow.getScene() == 28 && GameManager.Instance.model.user.level == 1)
		{ Debug.Log("Next tutorial 29"); tutorialWindow.SwitchToNextTutorialScene(); }

        if (GameManager.Instance.model.user.level > 1)
		{ Debug.Log("Load Server"); GameManager.Instance.loadModel(); GameManager.Instance.model.tutorialScene = 37; }

        Application.targetFrameRate = 60;

		GameManager.Instance.saveModel();

		while (GameManager.Instance.model == null)
			yield return true;

		updateText();

		StartCoroutine(preloadAndShow());

		if (GameManager.Instance.music == null)
		{
			GameManager.Instance.music = music;
			DontDestroyOnLoad(music);
		}
		else
		{ Destroy(music); }

		updateAttention();
		updatePostTimer = 10;

		//if (tutorialWindow.getScene() > 1) tutorialWindow.showState();

		if (cursor != null)
		{
			const CursorMode cursorMode = CursorMode.Auto;
			Vector2 hotSpot = Vector2.zero;
			hotSpot.Set(7, 20);
			Cursor.SetCursor(cursor, hotSpot, cursorMode);
		}

#if STEAM
			StartCoroutine(UpdateAchievements());
#endif
	}


	IEnumerator UpdateAchievements()
	{

#if STEAM
		model = GameManager.Instance.model;

		preloader.Activate();
		yield return model.getTasksGeneral();

		foreach(TaskGeneralProcess taskGeneralProcess in model.tasksGeneral)
		{
			if(taskGeneralProcess.state == TaskGeneralProcess.TASK_GENERAL_STATE_HAVE_REWARD)
			{
				var ach = new Achievement("ACHIEVEMENT_" + (taskGeneralProcess.id + 1));

				ach.Trigger();
			}

			yield return 1;
		}

#endif

		yield return null;
	}

	float updatePostTimer;

	private void Update()
	{
		updatePostTimer -= Time.deltaTime;
		if (updatePostTimer < 0)
		{
			if (GameManager.Instance.model.rules == null) return;
			if (GameManager.Instance.model.rules.commonParams == null) return;
			float postUpdateTime = GameManager.Instance.model.rules.commonParams.postUpdateTime;
			if (postUpdateTime < 5) postUpdateTime = 5;
			updatePostTimer = postUpdateTime;
			UpdatePost();
		}
	}

	private void updateAttention()
	{
		return;

		if (attentionShowed)
		{ attention.SetActive(false); }
		else
		{
			if (tutorialWindow.getScene() == -1) attention.SetActive(true);
			attentionShowed = true;
		}
	}

	public static bool needTasksShow = true;
	private static bool ratingWindowShow = false;

	public static bool needStarterPack = true;

	public static void needRatingWindowShow()
	{ ratingWindowShow = true; }


	public void StartTutorial()
	{
		tutorialWindow.showState();
	}

	private void StartLang()
	{
		tutorialWindow.lang.SetActive(true);
	}
	 
	private IEnumerator preloadAndShow()
	{
		//PlayerPrefs.DeleteAll();
		preloader.Activate();
		yield return GameManager.Instance.model.preload();
		yield return GameManager.Instance.model.getTasksGeneral();
		updateText();
		preloader.Deactivate();

		if (GameManager.Instance.model.user.level <= 1)
		{
			if (!PlayerPrefs.HasKey("lang"))
			{
				StartLang();
			}
			else if (PlayerPrefs.HasKey("lang"))
			{
				tutorialWindow.showState();
			}
		}
		else
		{
			if (tutorialWindow.getScene() != 29)
			{ GameManager.Instance.model.tutorialScene = -1; }

			GameManager.Instance.saveModel();
		}

		if (!PlayerPrefs.HasKey("FirstStart"))
		{
			PlayerPrefs.SetInt("FirstStart", -1);
		}
		/*
		else if(needStarterPack)
		{
            if (!tutorialWindow.gameObject.activeSelf)
            {
                needStarterPack = false;
                nstBag.OpenWindow();
            }
        }

        if (needTasksShow)
        {
            needTasksShow = false;
            if (GameManager.Instance.model.tutorialComplete()) tasksWindow.OpenWindow();
        }
        if (ratingWindowShow)
		{
			ratingWindowShow = false;
			ratingWindow.OpenWindow();
		}
        */
	}

	public void updateText()
	{
		if (GameManager.Instance.model.user != null)
		{
			updateGold();
			updateBackpacks();
		}
	}

	public void OnFightPressed()
	{
		if (GameManager.Instance.model.freeArena())
		{ BattleOnArena(); }
		else
		{
			noBattlesToday.Open();
			continueCost.SetValue(GameManager.Instance.model.rules.commonParams.continueArenaCost);
		}
	}

	public void OnContinueFightPressed()
	{
		if (GameManager.Instance.model.user.gold < GameManager.Instance.model.rules.commonParams.continueArenaCost)
		{ noMoney.SetActive(true); }
		else
		{
			GameManager.Instance.model.user.gold -= GameManager.Instance.model.rules.commonParams.continueArenaCost;
			goldFight = true;
			GameManager.Instance.saveModel();
			//BattleOnArena();
		}
	}

	public void OnContinueFightAdsPressed()
	{
		BattleScene.freeBattle = false;
		GameManager.Instance.unityAdsExample.ShowRewardedAd(ContinueBattleAds);
		if (GameManager.Instance.model.user.gold < GameManager.Instance.model.rules.commonParams.continueArenaCost)
		{ noMoney.SetActive(true); }
		else
		{ BattleOnArena(); }
	}

	int ContinueBattleAds()
	{
		BattleScene.freeBattle = true;
		BattleOnArena();
		return 0;
	}

	public void OnQuitPressed()
	{ Application.Quit(); }

	public void updateGold()
	{ goldText.text = GameManager.Instance.model.user.gold.ToString(); }

	public void updateBackpacks()
	{ backpacksText.text = GameManager.Instance.model.user.backpacks.ToString(); }

	public void OpenSite()
	{ Application.OpenURL("http://www.herwam.net/"); }

	public void BattleOnArena()
	{
		Invoke("BattleOnArenaInvoke", 0.5f);
	}

	public void BattleOnArenaInvoke()
	{
		GameManager.Instance.model.battle_test = true;
		StopInvokes();
		GameManager.LoadScene("Battle");
	}
	
	private const int TASK_REGISTER = 1;
	private const int TASK_SET_NAME = 2;
	private const int TASK_ARENA_3_WINS = 3;
	private const int TASK_OPEN_BP = 4;
	private const int TASK_CAMPAIGN_STAR = 5;

	[System.Obsolete]
	public void NavigateToTask(TaskBlock taskBlock)
	{
		tasksWindow.gameObject.SetActive(false);
		switch (taskBlock.getTaskId())
		{
			case TASK_REGISTER:
				settingsWindow.OpenWindow();
				break;

			case TASK_SET_NAME:
				profileWindow.OpenWindow();
				break;

			case TASK_ARENA_3_WINS:
				ratingWindow.OpenWindow();
				break;

			case TASK_OPEN_BP:
				openBackpackWindow.OpenWindow();
				break;

			case TASK_CAMPAIGN_STAR:
				wayWindow.OpenWindow();
				break;
		}
	}

	public void BackpacksClick()
	{
		if (GameManager.Instance.model.user.backpacks > 0)
		{ openBackpackWindow.OpenWindow(); }
		else
		{
			OpenShopWindow();
			shopWindow.OpenBackpacksBuy();
		}
	}

	public void StopInvokes()
	{ CancelInvoke(); }

	public void UpdatePost()
	{ StartCoroutine(UpdatePostProcess()); }

	IEnumerator UpdatePostProcess()
	{
		Model model = GameManager.Instance.model;
		yield return model.getAdminMessageV2();
		if (model.adminMessage != "")
		{ postWindow.ShowMessage(model.adminMessage, model.adminMessageImportant); }
	}

	public void OpenShopWindow()
	{
		if (shopWindow == null)
		{
			shopWindow = Instantiate(shopWindowPrefab);
			shopWindow.openBackpackWindow = openBackpackWindow;
			shopWindow.buyComplete = buyComplete;
			shopWindow.noMoney = noMoney;
			shopWindow.buyStarterPackWindow = buyStarterPackWindow;
			shopWindow.deckSelectorWindow = deckSelectorWindow;
			shopWindow.tasksGeneralWindow = tasksGeneralWindow;
			shopWindow.cardsUpgrade = cardsUpgrade;
			shopWindow.gameObject.transform.SetParent(noMoney.transform.parent);
			shopWindow.transform.SetSiblingIndex(shopWindow.transform.GetSiblingIndex() - 23);
			shopWindow.transform.position = Vector3.zero;

#if UNITY_ANDROID
			shopWindow.transform.localScale = Vector3.one * 1.96f;
#else
			shopWindow.transform.localScale = Vector3.one * 0.98f;
#endif
		}

		shopWindow.OpenWindow();
	}

	public void OpenShopWindowGold()
	{
		OpenShopWindow();
		shopWindow.OpenGoldBuy();
	}

	public void OpenBuyStarterPack()
	{ openBackpackWindow.gameObject.SetActive(true); }

	public void OpenDeckSelectorWindow()
	{

		deckSelectorWindow.OpenWindow();
		if (tutorialWindow.getScene() == 5) tutorialWindow.SwitchToNextTutorialScene();
    }

	public void OpenRatingWindow()
	{
		//if (tutorialWindow.getScene() == -1)
			ratingWindow.OpenWindow();
		//else tutorialWindow.SwitchToNextTutorialScene();
	}

	public void OpenTasksWindow()
	{
		tasksWindow.OpenWindow();
		
		//if (tutorialWindow.getScene() == -1) tasksWindow.OpenWindow();
		//else tutorialWindow.SwitchToNextTutorialScene();
	}

	public void OpenClanWindow()
	{ }

	public void OpenFreeBackpackWindow()
	{
		//if (tutorialWindow.getScene() == -1)
			freeBackpackWindow.OpenWindow();
		//else tutorialWindow.SwitchToNextTutorialScene();
	}
}