using CCGKit;
using UnityEngine;
using UnityEngine.UI;

public class CityButtonV2 : MonoBehaviour
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
    public int campaignID;
    public int cityID;
    public int areaID;

    public Image MainImage;
    public Image ButtonHL;

    public Sprite TexturePL;
    public Sprite TextureSU;
    public Sprite TextureDE;

    public Sprite TexturePLBorder;
    public Sprite TextureSUBorder;
    public Sprite TextureDEBorder;

    public Sprite TextureSUGray;
    public Sprite TextureDEGray;

    private Model model;
    private bool go = true;
    void Start()
    {
        model = GameManager.Instance.model;
        model.selectedCampaign = campaignID;
        model.selectedCity = cityID;

        Sprite[] gray = new Sprite[2] { TextureDEGray , TextureSUGray };
        Sprite[] active = new Sprite[2] { TextureDE, TextureSU };
        Sprite[] border = new Sprite[2] { TextureDEBorder, TextureSUBorder };

        int stars = model.user.campaigns[campaignID][cityID][areaID];

        switch (stars)
        {
            case 0:
                ButtonHL.gameObject.SetActive(false);
                MainImage.sprite = gray[campaignID];
            break;

            case 1:
                ButtonHL.gameObject.SetActive(true);
                ButtonHL.sprite = border[campaignID];
                MainImage.sprite = active[campaignID];
            break;

            case 2:
                ButtonHL.gameObject.SetActive(true);
                ButtonHL.sprite = border[campaignID];
                MainImage.sprite = active[campaignID];
            break;

            case 3:
                ButtonHL.gameObject.SetActive(true);
                ButtonHL.sprite = TexturePLBorder;
                MainImage.sprite = TexturePL;
            break;
        }

        if (model.canClickAreaButton(areaID))
        {
            ButtonHL.sprite = border[campaignID];
            MainImage.sprite = active[campaignID];
        }
        ShowStars(stars);

        if (stars >= 3)
        {
            this.gameObject.GetComponent<MenuButton>().interactable = false;
            Selection.GetComponent<MenuButton>().interactable = true;
            go = false;
        }
        else go = true;
    }

    private void ShowStars(int stars)
    {
        StarOff1.SetActive(stars <= 0);
        StarOff2.SetActive(stars <= 1);
        StarOff3.SetActive(stars <= 2);
        StarOn1.SetActive(stars > 0);
        StarOn2.SetActive(stars > 1);
        StarOn3.SetActive(stars > 2);
    }

    public void Battle()
    {
        model.selectedCampaign = campaignID;
        model.selectedCity = cityID;

        if (!model.canClickAreaButton(areaID)) return;

        model.selectedArea = areaID;
        model.selectLevel();
        model.battle_test = false;

        if (go)
        { GameManager.LoadScene("Battle"); }
    }
}