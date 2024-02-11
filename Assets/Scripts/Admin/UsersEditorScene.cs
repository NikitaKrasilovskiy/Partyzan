using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsersEditorScene : MonoBehaviour
{
    public Transform list;

    public InputField searchInput;
    public UserPreviewScript userPreview;
    public PreloaderAmination preloader;


    Model model;

    void Start ()
    { model = GameManager.Instance.model; }

    public void Search()
    {
        if (searchInput.text == "") return;
        StartCoroutine(SearchProcess());
    }

    IEnumerator SearchProcess()
    {
        preloader.Activate();
        yield return model.findUsersByText(searchInput.text);
        preloader.Deactivate();
        ShowFindResult();
    }

    public void EloList()
    { StartCoroutine(EloListProcess()); }

    IEnumerator EloListProcess()
    {
        preloader.Activate();
        yield return model.topEloUsers();
        preloader.Deactivate();
        ShowFindResult();
    }

    List<UserPreviewScript> userPreviewList = new List<UserPreviewScript>();

    public void ShowFindResult()
    {
        if (model.searchResult == null) return;
        foreach ( UserPreviewScript ups in userPreviewList )
        { Destroy(ups.gameObject); }

        userPreviewList.Clear();

        for (int i=0; i< model.searchResult.Count; i++)
        {
            UserPreviewScript up = Instantiate(userPreview);
            up.preloader = preloader;
            up.Show(model.searchResult[i]);
            up.transform.SetParent(list,false);
            up.transform.localScale = Vector3.one;
            up.transform.position = new Vector3(0,0.3f-i*0.6f,0);
            userPreviewList.Add(up);
        }
    }

    public void UpdateElo()
    {
        if (userPreviewList == null) return;
        StartCoroutine(UpdateEloProcess());
    }

    IEnumerator UpdateEloProcess()
    {
        preloader.Activate();
        List<EloSet> eloSet = new List<EloSet>();

        foreach (UserPreviewScript es in userPreviewList)
        { eloSet.Add(es.getEloSet()); }

        yield return model.updateEloRating(eloSet);
        preloader.Deactivate();
    }
}