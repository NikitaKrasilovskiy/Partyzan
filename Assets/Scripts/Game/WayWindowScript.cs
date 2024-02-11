using CCGKit;
using UnityEngine;

public class WayWindowScript : MonoBehaviour
{
    public HomeScene homeScene;
    public TutorialScript tutorial;

    [System.Obsolete]
    public void OpenWindow()
    {
        if (tutorial.getScene()==25)
        { ToBerlin(); }
        else
        {
            gameObject.SetActive(true);

            updateData();
        }
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    void updateData() { }

    [System.Obsolete]
    public void ToBerlin()
    {
        GameManager.Instance.model.selectedCampaign = 0;
        GameManager.Instance.model.selectedCity = 0;

        ToCampaignScene();
    }

    [System.Obsolete]
    public void ToMoscow()
    {
        GameManager.Instance.model.selectedCampaign = 1;
        GameManager.Instance.model.selectedCity = 0;

        ToCampaignScene();
    }

    [System.Obsolete]
    void ToCampaignScene()
    {
        homeScene.StopInvokes();
        DestroyObject (tutorial);
        System.GC.Collect();
        homeScene.preloader.Activate();
        GameManager.LoadSceneAsync("Campaign");
    }
}