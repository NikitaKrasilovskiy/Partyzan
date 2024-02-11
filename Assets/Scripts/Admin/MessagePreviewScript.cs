using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePreviewScript : MonoBehaviour
{
    Model model;

    public PreloaderAmination preloader;
    public TextMeshProUGUI text;

    ChatMessage chatMessage;

    public List<GameObject> disabledForAdmin;

    void Start ()
    { model = GameManager.Instance.model; }

    public void Show(ChatMessage chatMessage)
    {
        model = GameManager.Instance.model;
        this.chatMessage = chatMessage;
        text.text = chatMessage.getText();

        if (chatMessage.role == 1)
        {
            foreach (GameObject go in disabledForAdmin)
            { go.SetActive(false);}
        }
    }

    public void Remove()
    { StartCoroutine(RemoveProcess()); }

    IEnumerator RemoveProcess()
    {
        preloader.Activate();
        yield return model.removeMessage(0,chatMessage);
        gameObject.SetActive(false);
        preloader.Deactivate();
    }

    public void Ban10Min()
    { StartCoroutine(BanProcess(10*60)); }

    public void Ban1Hour()
    { StartCoroutine(BanProcess(60 * 60)); }

    public void Ban1Day()
    { StartCoroutine(BanProcess(24 * 60 * 60)); }

    IEnumerator BanProcess(int time)
    {
        preloader.Activate();
        yield return model.banInChats(chatMessage.userID, time);
        preloader.Deactivate();
    }
}