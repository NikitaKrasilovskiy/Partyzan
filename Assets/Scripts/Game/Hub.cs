using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;

public class Hub : MonoBehaviour
{
	public static Hub hub;
	public Sprite[] card_sprites;
	public AudioClip[] card_sounds1;
	public AudioClip[] card_sounds2;
	
	public delegate void Log(object val);
	
	public static Log log = Debug.Log;

	public List <string> steam_prices;

	// public TextAsset lang_base;
	// public List<TextAsset> langs;
	// public Dictionary<int, Sprite> card_sprites;

	void Awake()
	{
		hub = this;

		// GameManager.Instance.model.loadLangBase();
		// GameManager.Instance.model.loadLang(GameManager.Instance.model.user.lang);
	}
}