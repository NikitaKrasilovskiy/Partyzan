using CCGKit;
using System.Collections;
using TMPro;
using UnityEngine;

public class RatingWindowScript : MonoBehaviour
{
    public TextMeshProUGUI[] top;
    public TranslatorScript myRank;

    public GameObject list;
    public GameObject rank;
    public PreloaderAmination preloader;

    bool updated = false;

    void OnEnable()
    { StartCoroutine(updateData()); }

    void Update()
    {
        Model model = GameManager.Instance.model;

        if ((model.remain(model.state.arenaAwardMoment) < -3)&&(!updated))
        {
            updated = true;
            model.state.arenaAwardMoment += 24 * 60 * 60;
            StartCoroutine(updateDataAndState());
        }
    }

    IEnumerator updateDataAndState()
    {
        Model model = GameManager.Instance.model;
        //yield return model.loadServerState();
        yield return updateData();
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        StartCoroutine(updateData());
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    IEnumerator updateData() {
        list.SetActive(false);
        rank.SetActive(false);
        preloader.Activate();
        Model model = GameManager.Instance.model;
        yield return model.getRanks();

        for (int i = 0; i < top.Length; i++)
        {
            if (i >= model.ranks.players.Count)
            { top[i].transform.parent.gameObject.SetActive(false); }
            else
            {
                top[i].transform.parent.gameObject.SetActive(true);
                top[i].text = model.ranks.players[i].name;
            }
        }

        Debug.Log("Rank: " + model.ranks.rank.ToString());
        list.SetActive(true);
        rank.SetActive(true);
        preloader.Deactivate();
        myRank.Start();
        myRank.SetValue(model.ranks.rank);
        updated = false;
    }

    public void GetArenaReward()
    { StartCoroutine(GetArenaRewardProcess()); }

    IEnumerator GetArenaRewardProcess()
    {
        Model model = GameManager.Instance.model;
        preloader.Activate();
        yield return model.getArenaReward();
        preloader.Deactivate();
    }
}