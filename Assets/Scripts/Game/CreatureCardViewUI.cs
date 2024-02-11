using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using CCGKit;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using static Hub;

public class CreatureCardViewUI: MonoBehaviour
{
	[SerializeField] protected Image Background;
	[SerializeField] protected Image Image;
	[SerializeField] protected TextMeshProUGUI bodyText;
	[SerializeField] protected Image level;
	[SerializeField] public Image DeadMask;
	[SerializeField] protected DigiFontUI attackText;
	[SerializeField] protected DigiFontUI defenseText;
	[SerializeField] protected DigiFontUI attackModifier;
	[SerializeField] public DigiFontUI hpModifier;
	[SerializeField] protected List<Sprite> levelSprites;
	[SerializeField] protected List<Sprite> levelLabelSprites;
	[SerializeField] protected Sprite premiumBack;
	[NonSerialized] private int cardID = -1;
	[SerializeField] protected List<Sprite> generals;
	[SerializeField] protected List<Sprite> flags;
	[SerializeField] public Image shadow;
	[SerializeField] public Image Flag;
	[SerializeField] protected Sprite shirt;
	[SerializeField] public List<GameObject> NormalCardGraph;
	[SerializeField] public Image boom;
	[SerializeField] public AudioClip sound1;
	[SerializeField] public AudioClip sound2;
	[SerializeField] public GameObject border;
	[SerializeField] public GameObject borderPremium;
	[SerializeField] public GameObject borderLeader;
	[SerializeField] public Image Action;
	[SerializeField] public List<Sprite> abilities;

	Vector3 defStartPos;

	void Awake()
	{
		Assert.IsNotNull(Image);
		Assert.IsNotNull(bodyText);
		Assert.IsNotNull(attackText);
		Assert.IsNotNull(defenseText);
		Assert.IsNotNull(levelSprites);
		Assert.AreEqual(levelSprites.Count, 3);
		Assert.IsNotNull(level);
		Assert.IsNotNull(attackModifier);
		Assert.IsNotNull(hpModifier);
		Assert.IsNotNull(generals);
		Assert.AreEqual(generals.Count, 3);
		Assert.IsNotNull(DeadMask);
		Assert.IsNotNull(shirt);
		attackModifier.hideFide();
		hpModifier.hideFide();
		Action.gameObject.SetActive(false);
		defStartPos = defenseText.transform.localPosition;
	}

	void OnDestroy()
	{ Destroy(shadow); }

	internal int getCardID()
	{ return cardID; }

	[NonSerialized] private float deadCounter = -1;

	internal void dead()
	{
		showShirt();
		Image.gameObject.SetActive(false);
		border.gameObject.SetActive(false);
		borderPremium.gameObject.SetActive(false);
		borderLeader.gameObject.SetActive(false);
		level.gameObject.SetActive(false);

		foreach(GameObject go in NormalCardGraph)
		{ if(go != null) go.SetActive(false); }

		shadow.gameObject.SetActive(false);
		DeadMask.gameObject.SetActive(true);
		Background.gameObject.SetActive(false);
		deadCounter = 1.0f;
	}

	protected void Update()
	{
		if(deadCounter > 0)
		{
			deadCounter -= Time.deltaTime / BattleScene.actionLength * 2;
			if(deadCounter <= 0) Destroy(this.gameObject);
			DeadMask.color = new Color(1, 1, 1, deadCounter);
		}

		if(Action.gameObject.activeInHierarchy)
		{
			actionTime += Time.deltaTime;
			Action.color = Color.Lerp(Color.white, destColor, actionTime);
			Action.transform.position = Vector3.Lerp(baseActionPosition, destination, actionTime);
		}
	}

	internal void updateCard(BattleCard battleCard)
	{
		updateCard(GameManager.Instance.model.rules.cardTypes[battleCard.id], battleCard.level, battleCard.p);
		attackModifier.hideFide();
		hpModifier.hideFide();
		attackText.set(battleCard.attack);
		defenseText.set(battleCard.hp);
		setLevel(battleCard.level);
		Flag.sprite = flags[battleCard.country()];
		if(battleCard.isGeneral())
		{
			switch(battleCard.id)
			{
				case 0:
					setGeneral(0);
					break;

				case 6:
					setGeneral(1);
					break;

				case 12:
					setGeneral(2);
					break;
			}
		}
		else
		{
			defenseText.transform.localPosition = defStartPos;
			border.SetActive(true);
			borderPremium.SetActive(false);
			borderLeader.SetActive(false);
		}

		if(battleCard.p == 1)
		{ setPremium(); }
	}

	internal void updateCard(CardType value, int l, int premium)
	{
		Debug.Log("update_card");
		Debug.Log(value.id);
		if(value.cardDefs[l - 1] != null) attackText.set(value.cardDefs[l - 1].attack);
		if(value.cardDefs[l - 1] != null) defenseText.set(value.cardDefs[l - 1].hp);
		// updateTexture(value.image);
		
		
		//Image.sprite = hub.card_sprites[value.id];
		Image.sprite = hub.card_sprites[value.id];

		bool changeCard = false;
		if (changeCard == false)
		{
			if (Image.sprite == hub.card_sprites[2])
			{
				Image.sprite = hub.card_sprites[3];
				changeCard = true;
			}
		}
		if (changeCard == false)
		{
			if (Image.sprite == hub.card_sprites[3])
			{
				Image.sprite = hub.card_sprites[2];
				changeCard = true;
			}
		}
		
		
		Image.color = Color.white;
		
		
		
		// updateSound(value.attack_sound1, SOUND1);
		// updateSound(value.attack_sound2, SOUND2);
		
		sound1 = hub.card_sounds1[value.id];
		sound2 = hub.card_sounds2[value.id];
		
		bodyText.gameObject.SetActive(false);
		setLevel(l);
		cardID = value.id;
		attackModifier.delta(0);
		attackModifier.hideFide();
		hpModifier.delta(0);
		hpModifier.hideFide();
		if(value.isGeneral())
		{
			Debug.Log("GENERAL " + value.id);
			switch(value.id)
			{
				case 0:
					setGeneral(0);
					break;

				case 6:
					setGeneral(1);
					break;

				case 12:
					setGeneral(2);
					break;
			}
		}
		else
		{
			defenseText.transform.localPosition = defStartPos;

			foreach(GameObject go in NormalCardGraph)
			{ if(go != null) go.SetActive(true); }

			border.SetActive(true);
			borderPremium.SetActive(true);
			borderLeader.SetActive(false);
		}

		Flag.sprite = flags[value.country];
		if(premium == 1)
		{ setPremium(); }
	}

	private void setGeneral(int n)
	{
		//Background.sprite = generals[n];
		foreach(GameObject go in NormalCardGraph)
		{ if(go != null) go.SetActive(false); }

		Vector3
			v = attackText.gameObject.transform.position; // new Vector3(defenseText.gameObject.transform.position.x - 10, attackText.gameObject.transform.position.y + 15, attackText.gameObject.transform.position.z);
		defenseText.gameObject.transform.position = v;
		border.SetActive(false);
		borderPremium.SetActive(false);
		borderLeader.SetActive(true);
	}

	public void showShirt()
	{
		attackText.gameObject.SetActive(false);
		defenseText.gameObject.SetActive(false);
		bodyText.gameObject.SetActive(false);
		attackModifier.delta(0);
		attackModifier.hideFide();
		hpModifier.delta(0);
		hpModifier.hideFide();
		Background.sprite = shirt;
		Flag.gameObject.SetActive(false);
	}

	internal void setTitle(string title)
	{ bodyText.text = title; }

	internal void setAttack(int attack)
	{
		Debug.Log("setAttack(" + attack.ToString() + ")");
		attackModifier.delta(attack - attackText.getV());
		attackText.set(attack);
	}

	internal void setHP(int hp2)
	{
		hpModifier.delta(hp2 - defenseText.getV());
		defenseText.set(hp2);
	}

	internal void setLevel(int l)
	{
		Background.sprite = levelSprites[l - 1];
		level.sprite = levelLabelSprites[l - 1];
	}

	public bool WriteAllBytes(string fileName, byte[] byteArray)
	{
		try
		{
			using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				fs.Write(byteArray, 0, byteArray.Length);
				return true;
			}
		}
		catch(Exception ex)
		{
			Console.WriteLine("Exception caught in process: {0}", ex);
			return false;
		}
	}

    [Obsolete]
    IEnumerator loadWWWTexture(string url, string filename)
	{
		Debug.Log("loadWWWTexture");
		//TODO: заменить на веб скачивание когда появится возможность
		string localUrl = url.Remove(0, "http://datarotator.ru/ccg-admin".Length);
		Debug.Log(localUrl);
		localUrl = Application.streamingAssetsPath + localUrl;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            localUrl = "file:///"+localUrl;
#endif
		WWW www = new WWW(localUrl);
		// Wait for download to complete
		yield return www;

		// assign texture
		Rect rect = new Rect(0, 0, www.texture.width, www.texture.height);
		Image.sprite = Sprite.Create(www.texture, rect, new Vector2(0.5f, 0.5f));
		Image.color = Color.white;

		WriteAllBytes(filename, www.bytes);
		if(GameManager.Instance.textures.ContainsKey(filename))
			GameManager.Instance.textures[filename] = www.texture;
		else
			GameManager.Instance.textures.Add(filename, www.texture);
	}

    [Obsolete]
    public void updateFile(string url)
	{
		string filename = Application.persistentDataPath + "/" + url.GetHashCode().ToString("X8") +
		                  url.Substring(url.Length - 4);
		if(File.Exists(filename))
		{
			Debug.Log(filename);
			Debug.Log(url);
			Texture2D texture;
			if(GameManager.Instance.textures.ContainsKey(filename))
			{ texture = GameManager.Instance.textures[filename]; }
			else
			{
				byte[] fileData = File.ReadAllBytes(filename);
				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
				GameManager.Instance.textures.Add(filename, texture);
			}

			Rect rect = new Rect(0, 0, texture.width, texture.height);
			Image.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
			Image.color = Color.white;
		}
		else
		{ StartCoroutine(loadWWWTexture(url, filename)); }
	}

    [Obsolete]
    public void updateFileOGG(string url, int n)
	{
		string filename = Application.persistentDataPath + "/" + url.GetHashCode().ToString("X8") +
		                  url.Substring(url.Length - 4);
		if(File.Exists(filename))
		{
			if(GameManager.Instance.textures.ContainsKey(filename))
			{
				if(n == SOUND1) sound1 = GameManager.Instance.sounds[filename];
				if(n == SOUND2) sound2 = GameManager.Instance.sounds[filename];
			}
			else
			{
				// StartCoroutine(loadSound("file:///" + filename, n));
			}
		}
		else
		{ StartCoroutine(loadOGGSound(url, filename, n)); }
	}

    [Obsolete]
    IEnumerator loadSound(string path, int n)
	{
		//TODO: заменить на веб скачивание когда появится возможность
		string localUrl = path;
		if(path.Contains("http://"))
		{
			localUrl = path.Remove(0, "http://datarotator.ru/ccg-admin".Length);
			localUrl = Application.streamingAssetsPath + localUrl;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            localUrl = "file:///"+localUrl;
#endif

		}

		WWW www = new WWW(localUrl);
		yield return www;
		Debug.Log(path);
		if(n == SOUND1) sound1 = www.GetAudioClip();
		if(n == SOUND2) sound2 = www.GetAudioClip();
	}

	private const int SOUND1 = 1;
	private const int SOUND2 = 2;

    [Obsolete]
    IEnumerator loadOGGSound(string url, string filename, int n)
	{
		//TODO: заменить на веб скачивание когда появится возможность
		string localUrl = url.Remove(0, "http://datarotator.ru/ccg-admin".Length);

		localUrl = Application.streamingAssetsPath + localUrl;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            localUrl = "file:///"+localUrl;
#endif

		WWW www = new WWW(localUrl);
		yield return www;

		if(n == SOUND1) sound1 = www.GetAudioClip();
		if(n == SOUND2) sound2 = www.GetAudioClip();

		WriteAllBytes(filename, www.bytes);
		if(GameManager.Instance.sounds.ContainsKey(filename))
			GameManager.Instance.sounds[filename] = www.GetAudioClip();
		else
			GameManager.Instance.sounds.Add(filename, www.GetAudioClip());
	}

	public void updateTexture(string url)
	{
		Debug.Log(url);
		if(url == null) return;
		if(url == "") return;
		// updateFile(url);
	}

    [Obsolete]
    public void updateSound(string url, int n)
	{
		if(url == null) return;
		if(url == "") return;
		updateFileOGG(url, n);
	}

	public void setCountry(int country)
	{ }

	public void setPremium()
	{
		Background.sprite = premiumBack;
		border.SetActive(false);
		borderPremium.SetActive(true);
	}

	float actionTime = 0;
	Vector3 baseActionPosition;
	Vector3 destination;
	Color destColor;

	public void showAction(int id)
	{
		if(Action.gameObject.activeInHierarchy)
		{
			CancelInvoke("HideAction");
			HideAction();
		}
		else
		{ baseActionPosition = Action.transform.position; }

		if(id - 1 < 0)
		{
			Action.gameObject.SetActive(false);
			return;
		}
		destination = new Vector3(baseActionPosition.x, baseActionPosition.y + 2.5f, baseActionPosition.z);
		destColor = new Color(1, 1, 1, 0);
		Action.sprite = abilities[id - 1];
		Action.gameObject.SetActive(true);
		Action.color = Color.white;
		actionTime = 0;
		Invoke("HideAction", 1.0f);
	}

	private void HideAction()
	{
		Action.transform.position = baseActionPosition;
		Action.gameObject.SetActive(false);
	}
}