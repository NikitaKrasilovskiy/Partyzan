using CCGKit;
using System.Collections.Generic;
using UnityEngine;

public class CityButton : MonoBehaviour
{
    public GameObject StarOff1;
    public GameObject StarOff2;
    public GameObject StarOff3;
    public GameObject StarOn1;
    public GameObject StarOn2;
    public GameObject StarOn3;
    public GameObject Closed;
    public GameObject Enemy;
    public GameObject PL;
    public GameObject Selection;
    public int id;
    public HomeScene homeScene;

    public void Select()
    {
        if (Closed.activeInHierarchy) return;
        GameManager.Instance.model.selectedCity = id;

        foreach (CityButton cityButton in gameObject.transform.parent.gameObject.GetComponentsInChildren<CityButton>())
        { cityButton.UnSelect(); }

        Selection.SetActive(true);
        CampaignScene.state.Start();        
    }

    public void UnSelect()
    { Selection.SetActive(false); }

    public void block()
    {
        SetStars(0);
        Closed.SetActive(true);
        Enemy.SetActive(true);
        PL.SetActive(false);
        Selection.SetActive(false);
    }

    public void unlock()
    { Closed.SetActive(false); }

    public void SetStars(int n)
    {
        switch (n)
        {
            case 0:
                StarOff1.SetActive(true);
                StarOff2.SetActive(true);
                StarOff3.SetActive(true);
                StarOn1.SetActive(false);
                StarOn2.SetActive(false);
                StarOn3.SetActive(false);
                break;

            case 1:
                StarOff1.SetActive(false);
                StarOff2.SetActive(true);
                StarOff3.SetActive(true);
                StarOn1.SetActive(true);
                StarOn2.SetActive(false);
                StarOn3.SetActive(false);
                break;

            case 2:
                StarOff1.SetActive(false);
                StarOff2.SetActive(false);
                StarOff3.SetActive(true);
                StarOn1.SetActive(true);
                StarOn2.SetActive(true);
                StarOn3.SetActive(false);
                break;

            case 3:
                StarOff1.SetActive(false);
                StarOff2.SetActive(false);
                StarOff3.SetActive(false);
                StarOn1.SetActive(true);
                StarOn2.SetActive(true);
                StarOn3.SetActive(true);
                break;
        }
    }

    public void SetList(List<int> list)
    {
        int n = CampaignScene.stars(list);
        SetStars(n);
        switch (n)
        {
            case 0:
                Closed.SetActive(false);
                Enemy.SetActive(true);
                PL.SetActive(false);
                break;

            case 1:
                Closed.SetActive(false);
                Enemy.SetActive(true);
                PL.SetActive(false);
                break;

            case 2:
                Closed.SetActive(false);
                Enemy.SetActive(true);
                PL.SetActive(false);
                break;

            case 3:
                Closed.SetActive(false);
                Enemy.SetActive(false);
                PL.SetActive(true);
                break;
        }
        Selection.SetActive(id == GameManager.Instance.model.selectedCity);
    }

    public void SetArea(int n)
    {
        SetStars(n);
        //Closed.SetActive(false);
        Enemy.SetActive(n<3);
        PL.SetActive(n>=3);
        Selection.SetActive(false);
    }

    public void Battle()
    {
        if (!GameManager.Instance.model.canClickAreaButton(id)) return;
        GameManager.Instance.model.selectedArea = id;
        GameManager.Instance.model.selectLevel();
        GameManager.Instance.model.battle_test = false;
        GameManager.LoadScene("Battle");
        //GameManager.LoadScene("Battle-PC");
    }
}