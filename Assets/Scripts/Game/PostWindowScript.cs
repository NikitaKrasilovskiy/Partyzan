using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostWindowScript : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    public Image Poster;
    public Image Light;

    public Sprite PosterNorm;
    public Sprite PosterImp;

    public Sprite LightNorm;
    public Sprite LightImp;

    public void ShowMessage(string s, bool important)
    {
        if (s == "") return;
        gameObject.SetActive(true);

        messageText.text = s;
        Poster.sprite = important ? PosterImp : PosterNorm;
        Light.sprite = important ? LightImp : LightNorm;
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }
}