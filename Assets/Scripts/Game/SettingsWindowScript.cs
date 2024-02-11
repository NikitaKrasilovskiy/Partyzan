using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsWindowScript : MonoBehaviour
{
    private Model model;

    public List<GameObject> selector;
    public GameObject loginButton;
    public GameObject registerButton;
    public GameObject email;
    public TextMeshProUGUI myLogin;
    public PreloaderAmination preloader;
    public TutorialScript tutorialWindow;
    public HomeScene homeScene;

    [SerializeField]
    bool starte = false;

    void Start ()
    {
        model = GameManager.Instance.model;
        ShowLang();
    }

    public void OpenWindow()
    {
        model = GameManager.Instance.model;
        gameObject.SetActive(true);
        updateData();

        #if !STEAM
        if (model.user.login == "")
        {
            //loginButton.SetActive(true);
            //registerButton.SetActive(true);
            myLogin.gameObject.SetActive(false);
            email.gameObject.SetActive(false);
        }
        else
        {
            loginButton.SetActive(false);
            registerButton.SetActive(false);
            myLogin.gameObject.SetActive(true);
            email.gameObject.SetActive(true);
            myLogin.text = model.user.login;
        }
        #endif
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    void updateData()
    { ShowLang(); }
    
    public void SelectLang(int lang)
    { StartCoroutine(SwitchLangProcess(lang)); }

    private IEnumerator SwitchLangProcess(int lang)
    {
        if (!starte)
        { preloader.Activate(); }

        yield return model.updateLangAndLoadData(lang);

        if (!starte)
        {
            preloader.Deactivate();
        }

        ShowLang();

        if (starte)
        {
            PlayerPrefs.SetInt("lang", 0);
            PlayerPrefs.Save();
            gameObject.SetActive(false);
            homeScene.GetComponent<HomeScene>().StartTutorial();
        }
    }

    void ShowLang()
    {
        for (int i = 0; i < selector.Count; i++)
        { selector[i].SetActive(i == model.user.lang); }
    }
}