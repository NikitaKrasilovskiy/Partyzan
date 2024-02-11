using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserEditorScene : MonoBehaviour
{
    public PreloaderAmination preloader;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Lang;

    public TextMeshProUGUI state;

    public InputField message;
    public Toggle Important;

    private string[] langs = new string[]{"Русский","Английский","Польский"};

    Model model;

    void Start ()
    {
        model = GameManager.Instance.model;
        Name.text = "Имя: "+model.userEdit.name;
        Lang.text = "Язык: " + langs[model.userEdit.lang];
    }

    public void SendAdminMessage()
    {
        if (message.text == "") { state.text = "Пустое сообщение"; return; }
        StartCoroutine(SendAdminMessageProcess());
    }

    IEnumerator SendAdminMessageProcess()
    {
        preloader.Activate();
        yield return model.sendAdminPersonelMessage(model.userEditID, message.text, Important.isOn);

        state.text = "Сообщение отправлено";
        preloader.Deactivate();
    }
}