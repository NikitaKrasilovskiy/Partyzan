using CCGKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using FullSerializer;
using TMPro;

//{ "cards":[{ "id":6,"level":1},{ "id":-1},{ "id":7,"level":1},{ "id":7,"level":1},{ "id":7,"level":1},{ "id":-1}],"reward_gold":100}

[Serializable]
public class CampaignCard
{
    [SerializeField]
    public int id;

    [SerializeField]
    public int level;
}

[Serializable]
public class CampaignArea
{
    [SerializeField]
    public List<CampaignCard> cards;

    [SerializeField]
    public int reward_gold;
}


[Serializable]
public class City
{
    [SerializeField]
    public List<string> title;

    [SerializeField]
    public List<List<CampaignArea>> areas;
}

[Serializable]
public class Campaigns : List<List<City>>
{

}

[Serializable]
public class LevelUI
{
    [SerializeField]
    public List<CardUI> cards;
}

[Serializable]
public class CardUI
{
    [SerializeField]
    public Dropdown type;

    [SerializeField]
    public Dropdown level;
}

public class CampaignEditorScene : MonoBehaviour
{
    public InputField titleRU;
    public InputField titleEN;
    public InputField titlePL;
    public InputField goldLevel1;
    public InputField goldLevel2;
    public InputField goldLevel3;

    public Dropdown cities;
    public Dropdown areas;

    public List<LevelUI> levelsUI;

    private int campaign = 0;
    private int city = 0;
    private int area = 0;

    public PreloaderAmination preloader;
    public TextMeshProUGUI message;

    Model model;

    void Start()
    {
        model = GameManager.Instance.model;
        StartCoroutine(loadData());
    }

    public void Save()
    { StartCoroutine(saveRequest()); }

    public void SelectCampaign(int id)
    {
        campaign = id;
        cities.value = 0;
        city = 0;
        areas.value = 0;
        area = 0;
        UpdateCampaign();
    }

    public void SelectCity(int id)
    {
        city = id;
        areas.value = 0;
        area = 0;
        UpdateCity();
    }

    public void SelectArea(int id)
    {
        area = id;
        UpdateArea();
    }

    private void UpdateCampaign()
    {
        cities.ClearOptions();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        for (int i=0;i<campaigns[campaign].Count;i++)
            list.Add(new Dropdown.OptionData(campaigns[campaign][i].title[0]));
        cities.AddOptions(list);
        UpdateCity();
    }

    private void UpdateCity()
    {
        titleRU.text = campaigns[campaign][city].title[0];
        titleEN.text = campaigns[campaign][city].title[1];
        titlePL.text = campaigns[campaign][city].title[2];

        areas.ClearOptions();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        for (int i = 0; i < campaigns[campaign][city].areas.Count; i++)
            list.Add(new Dropdown.OptionData("Шаг "+(i+1).ToString()));
        areas.AddOptions(list);

        UpdateArea();
    }

    private void UpdateArea()
    {
        goldLevel1.text = campaigns[campaign][city].areas[area][0].reward_gold.ToString();
        goldLevel2.text = campaigns[campaign][city].areas[area][1].reward_gold.ToString();
        goldLevel3.text = campaigns[campaign][city].areas[area][2].reward_gold.ToString();
        for (int l = 0; l<3;l++)
        {
            for (int c=0;c<6;c++)
            {
                levelsUI[l].cards[c].type.value = campaigns[campaign][city].areas[area][l].cards[c].id + 1;
                levelsUI[l].cards[c].level.value = campaigns[campaign][city].areas[area][l].cards[c].level - 1;
            }
        }
    }

    private void UpdateModel()
    {
        campaigns[campaign][city].areas[area][0].reward_gold = int.Parse(goldLevel1.text);
        campaigns[campaign][city].areas[area][1].reward_gold = int.Parse(goldLevel2.text);
        campaigns[campaign][city].areas[area][2].reward_gold = int.Parse(goldLevel3.text);
        for (int l = 0; l < 3; l++)
        {
            for (int c = 0; c < 6; c++)
            {
                campaigns[campaign][city].areas[area][l].cards[c].id = levelsUI[l].cards[c].type.value - 1;
                campaigns[campaign][city].areas[area][l].cards[c].level = levelsUI[l].cards[c].level.value + 1;
            }
        }
    }

	class MsgUpdateAreaData
	{
		public int campaign_id;
		public int city_id;
		public int area_id;
		public City city;
	}

    public IEnumerator saveRequest()
    {
        campaigns[campaign][city].title[0] = titleRU.text;
        campaigns[campaign][city].title[1] = titleEN.text;
        campaigns[campaign][city].title[2] = titlePL.text;

        UpdateModel();

        preloader.Activate();

        var data = new MsgUpdateAreaData { campaign_id = campaign, city_id = city, area_id = area, city = campaigns[campaign][city] };

        yield return model.loadDataFromServer("updateArea", (string s) => { return 0; }, Model.toJSON(data));

        message.text = "Сохранено\n";

        preloader.Deactivate();
    }

    private Campaigns campaigns;

    public IEnumerator loadData()
    {
        yield return GameManager.Instance.model.loadRules();
        campaigns = GameManager.Instance.model.rules.campaigns;
        UpdateCardTypes();
        SelectCampaign(0);
    }

    private void UpdateCardTypes()
    {
        cities.ClearOptions();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        list.Add(new Dropdown.OptionData("Нет карты"));
        for (int i = 0; i < GameManager.Instance.model.rules.cardTypes.Count; i++)
            list.Add(new Dropdown.OptionData(GameManager.Instance.model.rules.cardTypes[i].title[0]));

        cities.AddOptions(list);

        for (int l = 0; l < 3; l++)
        {
            for (int c = 0; c < 6; c++)
            {
                levelsUI[l].cards[c].type.ClearOptions();
                levelsUI[l].cards[c].type.AddOptions(list);
            }
        }
    }

    public void UpdateCityTitle(string title)
    {
        cities.options[city].text = titleRU.text;
        cities.captionText.text = titleRU.text;
    }
}