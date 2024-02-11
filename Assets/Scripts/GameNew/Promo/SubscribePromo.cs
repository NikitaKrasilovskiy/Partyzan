using System.Collections;
using TMPro;
using UnityEngine;

public class SubscribePromo : MonoBehaviour
{
    [SerializeField]
    GameObject alredySubscribed;

    [SerializeField]
    GameObject donePanel;

    [SerializeField]
    GameObject errorPanel;

    [SerializeField]
    TextMeshProUGUI errorDetail;


    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    MenuButton subscribeBtn;

    void OnEnable ()
    {
        if (PlayerPrefs.HasKey("PromoSubscribe"))
        { alredySubscribed.SetActive(true); }
	}

    [System.Obsolete]
    public void Subscribe()
    {
        subscribeBtn.interactable = false;

        if (PlayerPrefs.HasKey("PromoSubscribe"))
        { alredySubscribed.SetActive(true); }
        else
        { StartCoroutine(Subscribe(inputField.text)); }
    }

    [System.Obsolete]
    IEnumerator Subscribe(string email)
    {
        WWW www = new WWW("https://herwam.net/subscribe/?mail="+email);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            PlayerPrefs.SetInt("PromoSubscribe", 0);
            donePanel.SetActive(true);
        }
        else
        {
            errorPanel.SetActive(true);
            errorDetail.text = www.error;
            subscribeBtn.interactable = true;
        }
    }
}