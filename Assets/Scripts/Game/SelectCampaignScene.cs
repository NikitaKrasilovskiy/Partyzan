using CCGKit;
using UnityEngine;

public class SelectCampaignScene : MonoBehaviour
{
    public void ToBerlin()
    {
        GameManager.Instance.model.selectedCampaign = 0;
        GameManager.Instance.model.selectedCity = 0;
        GameManager.LoadScene("Campaign");
        //GameManager.LoadScene("Campaign-PC");
    }

    public void ToMoscow()
    {
        GameManager.Instance.model.selectedCampaign = 1;
        GameManager.Instance.model.selectedCity = 0;
        GameManager.LoadScene("Campaign");
        //GameManager.LoadScene("Campaign-PC");
    }

    public void Return() {
        GameManager.LoadScene("Home");
        //GameManager.LoadScene("Home-PC");
    }
}