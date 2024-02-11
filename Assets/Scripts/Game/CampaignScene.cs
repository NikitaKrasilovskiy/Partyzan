using CCGKit;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CampaignScene : MonoBehaviour
{
    public GameObject moscow;
    public GameObject berlin;

    public TextMeshProUGUI cityTitleDE;
    public TextMeshProUGUI cityTitleSU;

    public List<CityButton> moscowButtons;
    public List<CityButton> berlinButtons;

    private List<CityButton> buttons;

    public PreloaderAmination preloader;

    public Transform backgroundPC;

    [System.Serializable]
    public class Area
    {
        public GameObject area;
        public List<CityButton> info;
    }

    public List<Area> moscowArea;
    public List<Area> berlinArea;

    private List<Area> area;

    private Model model;

    public TutorialScript tutorial;

    public Transform flag1;

    public static int stars(List<int> list)
    {
        if (list == null) return 0;
        if (list.Count == 0) return 0;
        int sum = 0;
        for (int i=0;i<list.Count;i++) {
            sum += list[i];
        }
        return sum / list.Count;
    }

    public static int sum(List<int> list)
    {
        if (list == null) return 0;
        int sum = 0;

        for (int i = 0; i < list.Count; i++) {
            sum += list[i];
        }
        return sum;
    }

    public void Start ()
    {
        state = this;
        model = GameManager.Instance.model;

        if (model.selectedCampaign == 0)
        {
            //moscow.SetActive(false);
            //berlin.SetActive(true);
            buttons = berlinButtons;
            area = berlinArea;
            //cityTitleDE.text = model.rules.campaigns[model.selectedCampaign][model.selectedCity].title[model.user.lang];
        }
        else if (model.selectedCampaign == 1)
        {
            //moscow.SetActive(true);
            //berlin.SetActive(false);
            buttons = moscowButtons;
            area = moscowArea;
            //cityTitleSU.text = model.rules.campaigns[model.selectedCampaign][model.selectedCity].title[model.user.lang];
        }
        for (int i=0;i<buttons.Count;i++)
        {
            if (area[i].area!=null) area[i].area.SetActive(i==model.selectedCity);
            try
            {
                buttons[i].SetList(model.user.campaigns[model.selectedCampaign][i]);

                if (!model.canClickCityButton(i)) buttons[i].block();
            }
            catch (System.Exception) { }
        }
        for (int j = 0; j < model.user.campaigns[model.selectedCampaign][model.selectedCity].Count; j++)
        {
            try
            {
                if (!model.canClickAreaButton(j)) area[model.selectedCity].info[j].block();
                if (model.user.campaigns[model.selectedCampaign][model.selectedCity][j]==3) area[model.selectedCity].info[j].unlock();
                area[model.selectedCity].info[j].SetArea(model.user.campaigns[model.selectedCampaign][model.selectedCity][j]);
            }
            catch (System.Exception) { }
        }
        if (tutorial.getScene()==25)
        {
            tutorial.SwitchToNextTutorialScene();
            flag1.SetParent(tutorial.transform, true);
            flag1.SetAsLastSibling();
            
#if UNITY_ANDROID
            flag1.position = new Vector3(-9, -4.5f, 90);
#else
            flag1.position = new Vector3(-10.5f, -2f, 90);
#endif
        }
        else
        { tutorial.showState(); }

        if(backgroundPC!=null)
        fixBackgroundPC(Vector2.zero);
    }
    public float X = 0;
    public float Y = 0;
    public float Z = 0;

    void Update ()
    {
        // if (tutorial.getScene()>23) {
        //     flag1.SetParent(tutorial.transform, true);
        //     flag1.SetAsLastSibling();
        // }
        //flag1.position = new Vector3(X, Y, Z);
    }

    public void playLevel(int id)
    {
        GameManager.Instance.model.currentLevel = id - 1;
        GameManager.LoadScene("Battle");
        //GameManager.LoadScene("Battle-PC");
    }

    public void OnBackButtonPressed()
    {
        GameManager.LoadScene("Home");
        //GameManager.LoadScene("Home-PC");
    }

    internal void updateMoney() {
    }

    public void ReturnToSelection() {
        GameManager.LoadScene("Home");
        //GameManager.LoadScene("Home-PC");
    }

    public void SelectMap(int id)
    { Debug.Log(id); }

    public static CampaignScene state;

    public void Battle(int level)
    {
        GameManager.Instance.model.selectedLevel = level - 1;
        GameManager.Instance.model.battle_test = false;
        GameManager.LoadScene("Battle");
        //GameManager.LoadScene("Battle-PC");
    }

    public void NextCity()
    {
        if (model.selectedCity < model.user.campaigns[model.selectedCampaign].Count-1)
        {
            if (sum(model.user.campaigns[model.selectedCampaign][model.selectedCity+1])>0)
            {
                model.selectedCity++;
                Start();
            }
        }
    }

    public void PrevCity()
    {
        if (model.selectedCity > 0)
        {
            model.selectedCity--;
            Start();
        }
    }

    public void BackToMenu()
    {
        preloader.Activate();
        GameManager.LoadSceneAsync("Home");
        //GameManager.LoadSceneAsync("Home-PC");
    }

    public void fixBackgroundPC(Vector2 v)
    {
        RectTransform rt = backgroundPC.GetComponent<RectTransform>();

        if (rt != null)
        { rt.anchoredPosition = new Vector3(-v.x*1800,0,0); }
    }
}