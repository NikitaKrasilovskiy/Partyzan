using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserPreviewScript : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI EMail;

    public InputField Elo;
    public TextMeshProUGUI Gold;

    public PreloaderAmination preloader;

    UserPreview userPreview;

    public void Show(UserPreview userPreview)
    {
        Name.text = userPreview.name;
        EMail.text = userPreview.email;
        Elo.text = userPreview.elo.ToString();
        Gold.text = userPreview.gold.ToString();
        this.userPreview = userPreview;
    }

    public void Click()
    { StartCoroutine(LoadUserProcess()); }

    public IEnumerator LoadUserProcess()
    {
        preloader.Activate();
        Model model = GameManager.Instance.model;
        yield return model.getUser(userPreview.id);

        preloader.Deactivate();
        SceneManager.LoadScene("UserEditor");
    }

    public EloSet getEloSet()
    {
        EloSet eloSet = new EloSet();
        eloSet.id = userPreview.id;
        eloSet.elo = float.Parse(Elo.text);
        return eloSet;
    }
}