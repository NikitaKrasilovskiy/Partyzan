using CCGKit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.Advertisements;

#if !STEAM
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing;
#endif
using GoogleMobileAds.Api;
using static Hub;

// ReSharper disable All


public class BattleScene: BaseScene, ICardMover
{
	[SerializeField] public float cardScaleFactor = 1.0f;

	private Vector3 cardScale;

	[SerializeField] public GameObject creatureCardViewPrefab;
	public GameObject creatureCardViewMovePrefab;


	[SerializeField] public List<Transform> cardPositions;


	[SerializeField] public List<Transform> enemyPositions;

	private List<CreatureCardViewUI> my;
	private List<CreatureCardViewUI> enemy;

	private List<MoveCard> mixer;
	private List<CreatureCardViewUI> shirts;

	public GameObject WinWindow;
	public GameObject FailWindow;

	protected AudioSource audioSource;
	public AudioSource audioSource1;
	public AudioSource audioSource2;
	public AudioSource butlleMusic;

	public AudioClip winSound;
	public AudioClip failSound;
	public AudioClip attackSound;

	public GameObject newLevelWindow;

	public List<BlockerCard> blockers;

	public Image Dark;

	public PreloaderAmination preloader;

	public GameObject DEMessage;
	public GameObject PLMessage;

	public DisableAdsScript DisableAds;

	private Model model;

	public RewardWindow rewardWindow;

	public GameObject SoundDisabled;

	public GameObject window;

	public TutorialScript tutorial;

	public GameObject surrenderButton;

	public GameObject textCloudDe;
	public GameObject textCloudPl;
	public GameObject textPlstart;
	public GameObject textDestart;
	public GameObject textPlLose;
	public GameObject textDelose;


	public GameObject clip_back;

	public GameObject clip;

	public GameObject leader_start;
	public GameObject enemy_leader_start;

	public GameObject leader;
	public GameObject enemy_leader;

	bool buttleStart = false;

	private bool leaders_created = false;

	private bool dribble_start = false;
	private bool dribble_enemy_start = false;

	public RewardedAd rewardedAd;
	InterstitialAd interstitialAd;
		[SerializeField]
    public string rewardId;
	[SerializeField]
    private string intAdId;

    public GameObject tutorialExit;
    public GameObject _tutorial;
	void Start()
	{

		if (tutorial.getScene() > 36)
		{
			Destroy(_tutorial);
		}
		model = GameManager.Instance.model;
		AdRequest request = new AdRequest.Builder().Build();
		rewardedAd = new RewardedAd(rewardId);
		// rewardedAd.OnAdLoaded += ShowReward;
		//rewardedAd.OnUserEarnedReward += openAdditionalReward;
		rewardedAd.LoadAd(request);
		DEMessage.SetActive(false);
		PLMessage.SetActive(false);
		cardScale = new Vector3(cardScaleFactor * BattleUISizeFixer.factor(), cardScaleFactor * BattleUISizeFixer.factor(),
			cardScaleFactor * BattleUISizeFixer.factor());
		hidePlaceholders();
		StartCoroutine(GameManager.Instance.model.preload());
		scriptActivate = false;
		mixer = null;
		my = null;
		enemy = null;
		GameManager.Instance.model.battleScript = null;
		preloader.Deactivate();
		ProcessPurchase();
		if(!HomeScene.goldFight)
		{
			if(model.adsEnabled())
			{
//#if !STEAM
				Debug.Log("Show Ads");
				// DateTime dt = DateTime.Now - new TimeSpan(0, 0, 1);
				// if (PlayerPrefs.HasKey("ads"))
				//     dt = DateTime.Parse(PlayerPrefs.GetString("ads"));
				// if (dt < DateTime.Now)
				// {
				// if(Advertisement.IsReady())
				// {
				// 	GameManager.Instance.unityAdsExample.ShowDefaultAd();
				// 	DisableAds.OpenWindow();
				// }
				// else
				// {
				// 	StartButtle();
				// }
				//interstitialAd = new InterstitialAd(intAdId);
				//interstitialAd.OnAdLoaded += OnIntertLoaded;
				//interstitialAd.OnAdFailedToLoad += OnAdShowed;
				//interstitialAd.OnAdClosed += OnAdShowed;
				//AdRequest adRequest = new AdRequest.Builder().Build();
				//interstitialAd.LoadAd(adRequest);
				// }
				// else
				// {
				//     StartButtle();
				// }
//else
                StartButtle();
//#endif
			}
			else
			{ StartButtle(); }
		}
		else
		{
			HomeScene.goldFight = false;
			StartButtle();
		}
		accelerate = false;
	}

	private void OnAdShowed(object sender, AdFailedToLoadEventArgs e)
    { StartButtle(); }

	private void OnAdShowed(object sender, EventArgs e)
    { StartButtle(); }

    private void OnIntertLoaded(object sender, EventArgs e)
    { interstitialAd.Show(); }

	public void StartButtle()
	{ buttleStart = true; }

	float time = 0;
	int actionID = -1;
	public static float actionLength = 1.0f;
	bool scriptActivate = false;
	ActionDef scriptAction;

	private readonly int ATTACK = 1;
	private readonly int DEAD = 2;
	private readonly int WIN = 3;
	private readonly int FAIL = 4;
	private readonly int BUFF = 5;


	bool paused = false;

	class Moving
	{
		Vector3 from;
		Vector3 to;
		Transform transform;

		public Moving(Transform transform, Vector3 to)
		{
			this.from = transform.position;
			this.to = to;
			this.transform = transform;
			transform.SetAsLastSibling();
		}

		public void update(float p)
		{
			if(p > 1) p = 1;
			if(p < 0) p = 0;
			transform.position = Vector3.Lerp(from, to, p);
		}

		public void finish()
		{ transform.position = to; }
	}

	private List<Moving> moving;

	void finishMoving()
	{
		if(moving == null)
		{ moving = new List<Moving>(); }
		else
		{
			foreach(Moving m in moving)
			{ m.finish(); }

			moving.Clear();
		}
	}

	void EnemyDieHard()
	{
		enemy[0].dead();
		enemy[0] = null;
		DEMessage.SetActive(false);
		textCloudDe.SetActive(false);
	}

	void MeDieHard()
	{
		Debug.Log("MeDieHard");
		my[0].dead();
		my[0] = null;
		PLMessage.SetActive(false);
		textCloudPl.SetActive(false);
	}

	bool isCampaign()
	{ return !GameManager.Instance.model.battle_test; }

	bool isArena()
	{ return GameManager.Instance.model.battle_test; }

	private bool accelerate = false;
	private static float accelerateFactor = 4;


	void start_scene()
	{
		if(!leaders_created)
		{
			BattleCard battleCard = GameManager.Instance.model.battleScript.disposition[0][0];
			leader = genLeaderCard(battleCard, leader_start.transform, "leader");

			battleCard = GameManager.Instance.model.battleScript.disposition[1][0];
			enemy_leader = genLeaderCard(battleCard, enemy_leader_start.transform, "enemy_leader");

			// clip_back.GetComponent<Image>().DOFade(0, 5);

			// clip.transform.DOScale(new Vector3(0, 0, 0), 5);

			Sequence mySequence = DOTween.Sequence();
			mySequence.Append(leader.transform.DOMove(new Vector3(-2.1f, 2.5f, 0), 0.2f));
			mySequence.Join(enemy_leader.transform.DOMove(new Vector3(1.5f, -0.78f, 0), 0.2f));
			mySequence.Append(leader.transform.DOMove(new Vector3(-3.81f, 3.62f, 0), 0.05f));
			mySequence.Join(enemy_leader.transform.DOMove(new Vector3(4.2f, -3.14f, 0), 0.05f));
			mySequence.Append(clip.transform.DOScale(new Vector3(1, 1, 1), 0.2f));
			mySequence.AppendInterval(1.5f);
			mySequence.AppendCallback(hide_leaders);
			mySequence.Append(clip_back.GetComponent<Image>().DOFade(0, 0.2f));
			mySequence.Join(clip.GetComponent<Image>().DOFade(0, 0.2f));
			mySequence.AppendCallback(clip_cmpl);
			leaders_created = true;
		}
	}


	void hide_leaders()
	{
		leader.SetActive(false);
		enemy_leader.SetActive(false);
	}


	void clip_cmpl()
	{
		showCards();
		scriptActivate = true;
		if(isCampaign()) time = -13.9f;
		else time = 0;
	}

	void Update()
	{
		if(buttleStart)
		{
			if(SoundDisabled != null) SoundDisabled.SetActive(!soundOn);
			if(paused) return;
			if(!scriptActivate)
			{
				if((mixer == null) && (GameManager.Instance.model.syncronized))
                {
                    showMixer();

                    if (GameManager.Instance.model.user.level > 1)
                    {
                        tutorialExit.SetActive(false);
                    }
                    else
                    {
                        tutorial.SwitchToNextTutorialScene();
                        if (tutorial.getScene() == 27) surrenderButton.SetActive(false);
                    }

                    window.SetActive(false);
                    battle();
                }
				

				if((mixer != null) && (GameManager.Instance.model.syncronized))
				{
					//updateMixer();
				}

				if((my == null) && (enemy == null) && (GameManager.Instance.model.syncronized) &&
				   (GameManager.Instance.model.battleScript != null))
				{ start_scene(); }
				return;
			}
			//time += Time.deltaTime * (accelerate ? 1.0f : accelerateFactor);
			if (tutorial)
			{
				if(!tutorial.isActiveAndEnabled)
				{
					time += Time.deltaTime;
					time += Time.deltaTime;
					if(accelerate)
					{
						time += Time.deltaTime;
						time += Time.deltaTime;
						time += Time.deltaTime;
						time += Time.deltaTime;
						time += Time.deltaTime;
					}
				}
			}
			else
			{
				time += Time.deltaTime;
				time += Time.deltaTime;
				if(accelerate)
				{
					time += Time.deltaTime;
					time += Time.deltaTime;
					time += Time.deltaTime;
					time += Time.deltaTime;
					time += Time.deltaTime;
				}
			}


			int currentAction = (int)Mathf.Floor(time / actionLength);
			if(currentAction != actionID)
			{
				actionID = currentAction;
				Debug.Log("Action: " + actionID);
				if((currentAction > 0) && (DEAD == GameManager.Instance.model.battleScript.actions[currentAction - 1].action))
				{
					finishMoving();
				}

				log("====current action======");
				log(currentAction);
				if(currentAction >= 0) scriptAction = GameManager.Instance.model.battleScript.actions[currentAction];
				else scriptAction = null;
				if(currentAction == -13)
				{
					if (tutorial)
					{
						if(!tutorial.isActiveAndEnabled)
						{
							audioSource1.clip = my[0].sound1;
							audioSource1.Play();
						}
					}
					else
					{
						audioSource1.clip = my[0].sound1;
						audioSource1.Play();
					}
					
					DEMessage.SetActive(false);
					textCloudDe.SetActive(false);
					PLMessage.SetActive(true);
					textCloudPl.SetActive(true);
					textPlstart.SetActive(true);
				}

				if(currentAction == -6)
				{
					if (tutorial)
					{
						if (!tutorial.isActiveAndEnabled)
						{
							audioSource2.clip = enemy[0].sound1;
							if (!accelerate)
								audioSource2.Play();
						}
					}
					else
					{
						audioSource2.clip = enemy[0].sound1;
						if (!accelerate)
							audioSource2.Play();
					}

					DEMessage.SetActive(true);
					textCloudDe.SetActive(true);
					textDestart.SetActive(true);
					PLMessage.SetActive(false);
				}

				if(currentAction == 0)
				{
					DEMessage.SetActive(false);
					PLMessage.SetActive(false);
					textCloudDe.SetActive(false);
					textDestart.SetActive(false);
					textCloudPl.SetActive(false);
					textPlstart.SetActive(false);
				}

				if(scriptAction == null) return;
				if((scriptAction.action == WIN) || (scriptAction.action == FAIL))
				{
					//hideArmy();
					scriptActivate = false;
					if(isCampaign())
					{
						if(scriptAction.action == WIN) Invoke("Message", 4.0f);
						else if(scriptAction.action == FAIL) Invoke("Message", 5.5f);
					}

					else
					{ Invoke("Message", 0.2f); }
				}

				if(scriptAction.action == DEAD)
				{
					finishMoving();
					if((scriptAction.actorDeck == 2) && (enemy[scriptAction.actorId] != null))
					{
						if((scriptAction.actorId == 0) && (isCampaign()))
						{
							audioSource2.clip = enemy[0].sound2;
							audioSource2.Play();
						}

						if((scriptAction.actorId > 0) || (isArena()))
						{
							enemy[scriptAction.actorId].dead();
							enemy[scriptAction.actorId] = null;
						}
						else
						{
							DEMessage.SetActive(true);
							textCloudDe.SetActive(true);
							textDelose.SetActive(true);
							Invoke("EnemyDieHard", 4.0f);
						}

						int j = 1;
						for(int i = 1; i < enemy.Count; i++)
						{
							if(enemy[i] != null)
							{
								enemy[j] = enemy[i];
								if(i != j) enemy[i] = null;
								float x = cardPositions[j].position.x;
								float y = enemy[j].transform.position.y;
								float z = enemy[j].transform.position.z;
								Vector3 v = new Vector3(x, y, z);
								moving.Add(new Moving(enemy[j].shadow.transform, v));
								moving.Add(new Moving(enemy[j].transform, v));
								moving.Add(new Moving(enemy[j].boom.transform, v + dV));
								j++;
							}
						}
					}

					if((scriptAction.actorDeck == 1) && (my[scriptAction.actorId] != null))
					{
						if((scriptAction.actorId == 0) && (isCampaign()))
						{
							audioSource2.clip = my[0].sound2;
							audioSource2.Play();
						}

						if((scriptAction.actorId > 0) || (isArena()))
						{
							my[scriptAction.actorId].dead();
							my[scriptAction.actorId] = null;
						}
						else
						{
							PLMessage.SetActive(true);
							textCloudPl.SetActive(true);
							textPlLose.SetActive(true);
							Invoke("MeDieHard", 5.5f);
						}

						int j = 1;
						for(int i = 1; i < my.Count; i++)
						{
							if(my[i] != null)
							{
								my[j] = my[i];
								if(i != j) my[i] = null;
								float x = cardPositions[j].position.x;
								float y = my[j].transform.position.y;
								float z = my[j].transform.position.z;
								Vector3 v = new Vector3(x, y, z);
								moving.Add(new Moving(my[j].shadow.transform, v));
								moving.Add(new Moving(my[j].transform, v));
								moving.Add(new Moving(my[j].boom.transform, v + dV));
								j++;
							}
						}
					}
				}

				if((scriptAction.action == ATTACK) || (scriptAction.action == BUFF) || (scriptAction.action == DEAD))
				{
					bool enemyLive = true;
					for(int i = scriptAction.effects.Count - 1; i >= 0; i--)
					{
						CreatureCardViewUI ccE = (scriptAction.effects[i].deck == 2)
							? enemy[scriptAction.effects[i].id]
							: my[scriptAction.effects[i].id];
						if(ccE != null)
						{
							ccE.setAttack(scriptAction.effects[i].attack);
							ccE.setHP(scriptAction.effects[i].hp);
							ccE.boom.color = Color.white;
							ccE.boom.DOFade(0.0f, 0.5f);
							if(scriptAction.effects[i].hp <= 0) enemyLive = false;
							if(scriptAction.action == BUFF) ccE.showAction(scriptAction.effects[i].icon);
							if(i < scriptAction.effects.Count - 1)
								if((scriptAction.action == ATTACK) && (scriptAction.effects[i].icon != -1))
									ccE.showAction(scriptAction.effects[i].icon);
							//ccE.gameObject.transform.SetAsLastSibling();
						}
					}

					if(scriptAction.action == ATTACK)
					{
						CreatureCardViewUI cc = (scriptAction.actorDeck == 2)
							? enemy[scriptAction.actorId]
							: my[scriptAction.actorId];
						AudioClip defaultHit = null; //attackSound
						if(cc != null)
						{
							if(enemyLive)
							{ GameManager.Instance.PlaySound(audioSource, (cc.sound1 == null)? defaultHit: cc.sound1); }
							else
							{ GameManager.Instance.PlaySound(audioSource, (cc.sound2 == null)? defaultHit: cc.sound2); }
						}
						else
						{ GameManager.Instance.PlaySound(audioSource, defaultHit); }
					}
				}
			}

			if((scriptAction != null) && (scriptAction.action == BUFF))
			{
				float t = time - (actionID * actionLength);
				Transform t1;
				Transform t2;

				if(scriptAction.actorDeck == 1)
				{
					t1 = cardPositions[scriptAction.actorId];
					t2 = enemyPositions[0];
				}
				else
				{
					t1 = enemyPositions[scriptAction.actorId];
					t2 = cardPositions[0];
				}

				float p = (t / actionLength);

				p = Mathf.Sin(p * 30) * 0.3f * (Mathf.Max(0.6f - p, 0));
				CreatureCardViewUI ccw = (scriptAction.actorDeck == 1)? my[scriptAction.actorId]: enemy[scriptAction.actorId];
				if(ccw != null)
				{
					ccw.transform.position = new Vector3(t1.position.x, (t2.position.y - t1.position.y) * p + t1.position.y,
						t1.position.z);
					ccw.shadow.transform.position = ccw.transform.position;
					ccw.shadow.transform.SetAsFirstSibling();
				}

				//ccw.gameObject.transform.SetAsLastSibling();
			}

			if((scriptAction != null) && (scriptAction.action == ATTACK))
			{
				float t = time - (actionID * actionLength);
				Transform t1;
				Transform t2;
				int index = 0;
				int index_e = 0;

				if(scriptAction.actorDeck == 1)
				{
					t1 = cardPositions[scriptAction.actorId];
					t2 = enemyPositions[scriptAction.effects[0].id];
					index = scriptAction.actorId;
					index_e = scriptAction.effects[0].id;
				}
				else
				{
					t1 = enemyPositions[scriptAction.actorId];
					t2 = cardPositions[scriptAction.effects[0].id];
					index = scriptAction.actorId;
					index_e = scriptAction.effects[0].id;
				}

				float p = (t / actionLength);
				float s = p - Mathf.Floor(p);
				s *= 2;
				if(s > 1) s = 1;
				s -= 0.5f;
				s = Mathf.Abs(s);
				s = 0.5f - s;
				s *= 2;
				if(scriptAction.actorId == scriptAction.effects[0].id)
					s *= 1f;

				p = Mathf.Sin(p * 30) * 0.3f * (Mathf.Max(0.6f - p, 0));
				CreatureCardViewUI ccw;
				CreatureCardViewUI ccwe;
				if(scriptAction.actorDeck == 1)
				{
					ccw = my[scriptAction.actorId];
					ccwe = enemy[scriptAction.actorId];
					if(ccwe == null)
					{ ccwe = enemy[0]; }
				}
				else
				{
					ccwe = my[scriptAction.actorId];
					ccw = enemy[scriptAction.actorId];
					
					if(ccwe == null)
					{ ccwe = my[0]; }					
				}

				if(ccw != null)
				{

					float dist = 0.7f;

					if(index_e == 0)
					{
						dist = 0.9f;
						
						if(index > 2)
						{ dist = 0.95f; }
						
						if(index > 4)
						{ dist = 0.97f; }
						
					}

					float time = 0.150f;

					if(accelerate)
					{ time = time / 4; }
					
					if(s > dist)
					{
						// ccw.transform.position = Vector3.Lerp(t1.position, t2.position, 0.5f);
						if(!dribble_start)
						{

							if(scriptAction.actorDeck == 1)
							{ ccw.transform.DOPunchPosition(new Vector3(0, -1.7f, 0), time, 2, 0.5f).OnComplete(() => { dribble_start = false; }); }
							else
							{ ccw.transform.DOPunchPosition(new Vector3(0, 1.7f, 0), time, 2, 0.5f).OnComplete(() => {dribble_start = false; }); }							

							dribble_start = true;
						}

					}
					else
					{ ccw.transform.position = Vector3.Lerp(t1.position, t2.position, s); }

					if(s > dist)
					{
						if(ccwe != null)
						{
							if(!dribble_enemy_start)
							{
								ccwe.transform.DOPunchRotation(new Vector3(0, 0, 10f), time, 2, 0.5f).OnComplete(() => { dribble_enemy_start = false; });
								dribble_enemy_start = true;
							}
						}
					}
					
					ccw.shadow.transform.position = ccw.transform.position;
					ccw.shadow.transform.SetAsFirstSibling();
					ccw.transform.SetAsLastSibling();
					ccw.transform.parent.SetAsLastSibling();
				}

				//ccw.gameObject.transform.SetAsLastSibling();
			}

			if((scriptAction != null) && (scriptAction.action == DEAD))
			{
				float t = time - (actionID * actionLength);
				float p = (t / actionLength) * 2 - 1;
				foreach(Moving m in moving)
				{ m.update(p); }
			}
		}
	}

	int[] ids = new int[] {0, 1, 2, 3, 4, 5};

	string idsS()
	{
		return ids[0].ToString() + "/" + ids[1].ToString() + "/" + ids[2].ToString() + "/" + ids[3].ToString() + "/" +
		       ids[4].ToString() + "/" + ids[5].ToString();
	}

	public void Message()
	{
		panelCanvasGroup.blocksRaycasts = true;
		panelCanvasGroup.GetComponent<Image>().DOKill();
		panelCanvasGroup.GetComponent<Image>().DOFade(0.5f, 0.5f);

		if(scriptAction.action == WIN)
		{
			WinWindow.SetActive(true);
			//if (GameManager.Instance.unityAdsExample.canShowRewardedAd())
			//rewardWindow.ShowCurrentReward();
			GameManager.Instance.PlaySound(audioSource, winSound);

			if (GameManager.Instance.model.user.level <= 1)
			{
				tutorial.SwitchToNextTutorialScene();
			}
		}
		else
		{
			FailWindow.SetActive(true);
			// if (GameManager.Instance.unityAdsExample.canShowRewardedAd())
			//rewardWindow.ShowCurrentReward();
			GameManager.Instance.PlaySound(audioSource, failSound);
		}

		showNewLevels();
	}

	public void DoubleReward()
	{
		//GameManager.Instance.unityAdsExample.ShowRewardedAd(openAdditionalReward);
		if(rewardedAd.IsLoaded())
			ShowReward();
	}

	void ShowReward()
	{ rewardedAd.Show(); }
/*
		public void openAdditionalReward(object sender, Reward args)
	{
		Model model = GameManager.Instance.model;
		rewardWindow.ShowDoubleResult();
		StartCoroutine(model.DoubleReward());
		showNewLevels();
		//return 0;
	}
	*/
		public void openAdditionalReward()
		{
			Model model = GameManager.Instance.model;
			rewardWindow.ShowDoubleResult();
			StartCoroutine(model.DoubleReward());
			showNewLevels();
			//return 0;
		}
		public void dobleRevardAdManager()
		{
			
		}
		
	public void showNewLevels()
	{
		if((GameManager.Instance.model.upLevelReward != null) && (GameManager.Instance.model.upLevelReward.Count > 0))
		{ NewLevelWindow.mastShow = true; }
	}

	public void autoSet()
	{ }

	public void battle()
	{
		ids[0] = 0;
		for(int i = 1; i < 6; i++)
		{
			if(mixer[i] == null)
				ids[i] = -1;
			else
				ids[i] = mixer[i].id;
		}

		if(isArena())
			StartCoroutine(ArenaProcess());
		else
			StartCoroutine(CampaignProcess());
	}

	private IEnumerator ArenaProcess()
	{
		preloader.Activate();
		yield return GameManager.Instance.model.battle(idsS(), freeBattle? "freeBattle": "");
		HomeScene.needRatingWindowShow();
		preloader.Deactivate();
	}

	private IEnumerator CampaignProcess()
	{
		preloader.Activate();
		yield return GameManager.Instance.model.battle_campaign(idsS());
		preloader.Deactivate();
	}

	void showCards()
	{
		//hideMixer();
		my = new List<CreatureCardViewUI>();
		enemy = new List<CreatureCardViewUI>();
		for(int i = 0; i < cardPositions.Count; i++)
		{
			BattleCard battleCard = GameManager.Instance.model.battleScript.disposition[0][i];
			Debug.Log("showcards=========================");
			Debug.Log(battleCard.id);
			if(!battleCard.present())
			{ my.Add(null); }
			else
			{ my.Add(genCard(battleCard, cardPositions[i])); }
		}

		for(int i = 0; i < enemyPositions.Count; i++)
		{
			BattleCard battleCard = GameManager.Instance.model.battleScript.disposition[1][i];
			if(!battleCard.present())
			{ enemy.Add(null); }
			else
			{ enemy.Add(genCard(battleCard, enemyPositions[i])); }
		}
	}

	void showMixer()
	{
		int deckSize = GameManager.Instance.model.deckSize();
		if(deckSize >= 5) blockers[2].Hide();
		if(deckSize >= 4) blockers[1].Hide();
		if(deckSize >= 3) blockers[0].Hide();

		mixer = new List<MoveCard>();
		mixer.Add(null);
		for(int i = 1; i < cardPositions.Count; i++)
		{
			if(i >= GameManager.Instance.model.user.decks[0].cards.Count)
			{
				mixer.Add(null);
				continue;
			}

			CardV3 cardV3 = GameManager.Instance.model.user.decks[0].cards[i];
			if(!cardV3.present())
			{
				mixer.Add(null);
				continue;
			}

			mixer.Add(genCardMovement(cardV3, cardPositions[i], i));
		}

		shirts = new List<CreatureCardViewUI>();
		for(int i = 1; i < enemyPositions.Count; i++)
		{ shirts.Add(genCardShirt(enemyPositions[i])); }
		
		hideMixer();
	}

	CreatureCardViewUI genCardShirt(Transform t)
	{
		GameObject go = Instantiate(creatureCardViewPrefab as GameObject);
		go.transform.SetParent(t.parent);
		go.transform.SetAsFirstSibling();
		CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();
		cardView.showShirt();
		cardView.transform.localScale = cardScale;
		cardView.gameObject.GetComponent<RectTransform>().position = t.position;
		/*
		cardView.hpModifier.background.transform.SetParent(t.parent);
		cardView.hpModifier.transform.SetParent(t.parent);
		cardView.shadow.transform.SetParent(t.parent);
		cardView.shadow.transform.SetAsFirstSibling();
		*/
		return cardView;
	}

	CreatureCardViewUI genCard(BattleCard battleCard, Transform t)
	{
		GameObject go = Instantiate(creatureCardViewPrefab as GameObject);
		go.transform.SetParent(t.parent);
		go.transform.SetAsFirstSibling();
		CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();
		cardView.updateCard(battleCard);
		cardView.transform.localScale = cardScale;
		cardView.gameObject.GetComponent<RectTransform>().position = t.position;
		cardView.hpModifier.background.transform.SetParent(t.parent);
		cardView.hpModifier.transform.SetParent(t.parent);
		cardView.shadow.transform.SetParent(t.parent);
		cardView.shadow.transform.SetAsFirstSibling();
		dV = cardView.boom.transform.position - cardView.transform.position;
		return cardView;
	}


	GameObject genLeaderCard(BattleCard battleCard, Transform t, string name)
	{
		GameObject go = Instantiate(creatureCardViewPrefab as GameObject);
		go.name = name;
		go.transform.SetParent(t.parent);
		go.transform.SetAsFirstSibling();

		CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();
		cardView.updateCard(battleCard);
		cardView.gameObject.GetComponent<RectTransform>().position = t.position;
		cardView.hpModifier.background.transform.SetParent(t.parent);
		cardView.hpModifier.transform.SetParent(t.parent);
		cardView.shadow.transform.SetParent(t.parent);
		cardView.shadow.transform.SetAsFirstSibling();
		dV = cardView.boom.transform.position - cardView.transform.position;
		go.transform.parent = clip_back.transform;
		go.transform.localScale = new Vector3(1, 1, 1);
		return go;
	}

	Vector3 dV;

	MoveCard genCardMovement(CardV3 cardV3, Transform t, int id)
	{
		GameObject go = Instantiate(creatureCardViewMovePrefab as GameObject);
		go.transform.SetParent(t.parent);
		go.transform.SetAsFirstSibling();
		CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();
		cardView.updateCard(GameManager.Instance.model.rules.cardTypes[cardV3.id], cardV3.level, cardV3.p);
		cardView.transform.localScale = cardScale;
		cardView.gameObject.GetComponent<RectTransform>().position = t.position;
		cardView.hpModifier.background.transform.SetParent(t.parent);
		cardView.hpModifier.transform.SetParent(t.parent);
		MoveCard moveCard = go.GetComponent<MoveCard>();
		moveCard.id = id;
		moveCard.ind = id;
		//cardView.shadow.transform.SetParent(t.parent);
		//cardView.shadow.transform.SetAsFirstSibling();
		return moveCard;
	}

	public MenuButton autoComplete;

	public void movementFinish(GameObject card)
	{
		float x = card.gameObject.transform.position.x;
		int newId = 0;
		if(x < cardPositions[1].position.x) newId = 1;
		else if(x > cardPositions[5].position.x) newId = 6;
		else
			for(int i = 1; i < 5; i++)
			{ if((x > cardPositions[i].position.x) && (x < cardPositions[i + 1].position.x)) newId = i + 1; }

		int oldIndex = card.GetComponent<MoveCard>().ind;
		if((newId < 6) && (mixer[newId] == null))
		{
			mixer[newId] = mixer[oldIndex];
			mixer[oldIndex] = null;
		}
		else if((mixer[newId - 1] == null) && (newId > 1))
		{
			mixer[newId - 1] = mixer[oldIndex];
			mixer[oldIndex] = null;
		}
		else
		{
			var item = mixer[oldIndex];
			mixer.RemoveAt(oldIndex);
			if(newId > oldIndex) newId--;
			// the actual index could have shifted due to the removal
			if(newId < 5)
				mixer.Insert(newId, item);
			else
				mixer.Add(item);
		}

		int j = 1;
		for(int i = 1; i < 6; i++)
		{
			if(mixer[i] != null)
			{
				mixer[j] = mixer[i];
				if(i != j) mixer[i] = null;
				j++;
			}
		}

		for(int i = 1; i < 6; i++)
		{
			if(mixer[i] != null)
			{
				mixer[i].animTo(cardPositions[i].transform.position);
				mixer[i].ind = i;
			}
		}

		autoComplete.onClickEvent.Invoke();
	}

	void hideMixer()
	{
		for(int i = 0; i < mixer.Count; i++)
		{ if(mixer[i] != null) Destroy(mixer[i].gameObject); }

		for(int i = 0; i < shirts.Count; i++)
		{ if(shirts[i] != null) Destroy(shirts[i].gameObject); }

		for(int i = 0; i < blockers.Count; i++)
		{ if(blockers[i] != null) Destroy(blockers[i].gameObject); }
	}

	void hideArmy()
	{
		for(int i = 0; i < my.Count; i++)
		{ if(my[i] != null) Destroy(my[i].gameObject); }

		for(int i = 0; i < enemy.Count; i++)
		{ if(enemy[i] != null) Destroy(enemy[i].gameObject); }
	}

	void hidePlaceholders()
	{
		Color invisible = new Color(1, 1, 1, 0);
		foreach(Transform t in cardPositions)
		{ t.gameObject.GetComponent<SpriteRenderer>().color = invisible; }

		foreach(Transform t in enemyPositions)
		{ t.gameObject.GetComponent<SpriteRenderer>().color = invisible; }
	}

	public void ToHome()
	{
		GameManager.Instance.StopSound();
		audioSource1.Stop();
		audioSource2.Stop();
		Dark.gameObject.SetActive(true);
		Dark.DOColor(Color.black, 3);

		if (isArena() || (tutorial.getScene() == 28))
			GameManager.LoadSceneAsync("Home");
		else
			GameManager.LoadSceneAsync("Campaign");
			
		GameManager.Instance.SoundOn();
	}

	public GameObject SurrenderWindows;
	[NonSerialized] internal static bool freeBattle = false;

	public void AskSurrender()
	{
		paused = true;
		SurrenderWindows.SetActive(true);
	}

	private bool surrender = false;

	public void SurrenderYes()
	{
		surrender = true;
		SurrenderWindows.SetActive(false);
		FailWindow.SetActive(true);
		GameManager.Instance.PlaySound(audioSource, failSound);
	}

	public void SurrenderNo()
	{
		SurrenderWindows.SetActive(false);
		paused = false;
		SurrenderWindows.SetActive(false);
	}

	public void click(GameObject card)
	{ }

	public void BigClick()
	{
		Debug.Log("Click");
		if((time < 0) && (scriptActivate))
		{
			audioSource1.Stop();
			audioSource2.Stop();
			DEMessage.SetActive(false);
			PLMessage.SetActive(false);
			textCloudDe.SetActive(false);
			textDestart.SetActive(false);
			textCloudPl.SetActive(false);
			textPlstart.SetActive(false);
			time = 0.01f;
		}

		if(IsInvoking())
		{
			CancelInvoke();
			if(DEMessage.activeInHierarchy)
			{
				EnemyDieHard();
				DEMessage.SetActive(false);
				textCloudDe.SetActive(false);
			}

			if(PLMessage.activeInHierarchy)
			{
				MeDieHard();
				PLMessage.SetActive(false);
				textCloudPl.SetActive(false);
			}

			audioSource1.Stop();
			audioSource2.Stop();
			Message();
		}

		if(surrender)
		{ CloseFailWindow(); }

		if(WinWindow.activeInHierarchy)
		{
			return;
			// CloseWinWindow();
		}

		if(FailWindow.activeInHierarchy)
		{ CloseFailWindow(); }
	}

	public void BuyDisableAdsLittle()
	{
		//GameManager.Instance.myIAPManager.Process("disable_ads_short", this);
	}

	public void BuyDisableAdsBig()
	{
		//  GameManager.Instance.myIAPManager.Process("disable_ads_long", this);
	}


	public void AccelerateHandler()
	{
		accelerate = !accelerate;
		audioSource1.Stop();
		audioSource2.Stop();
	}

	private bool soundOn = true;

	public void DisableSoundHandler()
	{
		if(soundOn)
		{
			if(audioSource != null) audioSource.volume = 0;
			if(audioSource1 != null) audioSource1.volume = 0;
			if(audioSource2 != null) audioSource2.volume = 0;
			if (butlleMusic != null) butlleMusic.volume = 0;
			soundOn = false;
			GameManager.Instance.SoundOff();
		}
		else
		{
			if(audioSource != null) audioSource.volume = 1;
			if(audioSource1 != null) audioSource1.volume = 1;
			if(audioSource2 != null) audioSource2.volume = 1;
			if (butlleMusic != null) butlleMusic.volume = 1;
			soundOn = true;
			GameManager.Instance.SoundOn();
		}
	}

	public void SurrenderHandler()
	{ AskSurrender(); }


	public void CloseWinWindow()
	{
		if(rewardWindow.isActiveAndEnabled)
		{ WinWindow.SetActive(false); }
		else
		{ ToHome(); }
	}

	public void CloseFailWindow()
	{
		if(rewardWindow.isActiveAndEnabled)
		{ FailWindow.SetActive(false); }
		else
		{ ToHome(); }
	}


	public void ProcessPurchase()
	{
#if UNITY_IOS || UNITY_STANDALONE_OSX
		 var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		 // Get a reference to IAppleConfiguration during IAP initialization.
		 var appleConfig = builder.Configure<IAppleConfiguration>();
		 byte[] receiptData = null;
		 try{
		      receiptData = System.Text.Encoding.UTF8.GetBytes(Application.identifier);
		      Debug.LogError(Application.identifier);
		 }
		 catch{
		     Debug.LogError(appleConfig);
		 }
		 AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
		 Debug.Log(receipt.bundleID);
		 Debug.Log(receipt.receiptCreationDate);
		 foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts) {
		     Debug.Log(productReceipt.transactionID);
		     Debug.Log(productReceipt.productID);
		 }
#endif
    }

#if !STEAM
    public bool isSubscriptionActive(AppleInAppPurchaseReceipt appleReceipt)
	{
		bool isActive = false;

		AppleInAppPurchaseReceipt apple = appleReceipt;
		if(null != apple)
		{
			DateTime expirationDate = apple.subscriptionExpirationDate;
			DateTime now = DateTime.Now;
			//DateTime cancellationDate = apple.cancellationDate;

			if(DateTime.Compare(now, expirationDate) < 0)
			{
				isActive = true;
			}
		}

		return isActive;
	}

	#endif
}