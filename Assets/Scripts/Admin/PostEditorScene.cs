using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostEditorScene : MonoBehaviour
{
    public InputField MessageRU;
    public InputField MessageEN;
    public InputField MessagePL;
    public Toggle Important;
    public TextMeshProUGUI state;
    public PreloaderAmination preloader;

    public void SendMessage()
    {
        if (MessageRU.text == "") { state.text = "Сообщение на русском языке пустое"; return; }
        if (MessageEN.text == "") { state.text = "Сообщение на английском языке пустое"; return; }
        if (MessagePL.text == "") { state.text = "Сообщение на польском языке пустое"; return; }

        StartCoroutine(SendMessageProcess());
    }

    IEnumerator SendMessageProcess()
    {
        Model model = GameManager.Instance.model;
        preloader.Activate();
        AdminMessage adminMessage = new AdminMessage();
        adminMessage.message = new string[3];
        adminMessage.message[0] = MessageRU.text;
        adminMessage.message[1] = MessageEN.text;
        adminMessage.message[2] = MessagePL.text;
        adminMessage.important = Important.isOn;
        yield return model.sendAdminMessage(adminMessage);
        state.text = model.sendAdminMessageComplete ? "Сообщение отправлено всем пользователям" : "Сообщение НЕ отправлено";
        preloader.Deactivate();
    }
}