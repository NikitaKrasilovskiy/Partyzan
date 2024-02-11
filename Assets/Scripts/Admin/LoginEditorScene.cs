using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginEditorScene : MonoBehaviour
{
    public InputField EMail;
    public InputField Password;
    public TextMeshProUGUI log;
    public PreloaderAmination preloader;

    void Start ()
    {
        Model model = GameManager.Instance.model;
        EMail.text = model.user.login;
        Password.text = model.user.pass;
        getRole();
    }

    private IEnumerator loginProcess()
    {
        preloader.Activate();
        log.text = "Запрос...";
        Model model = GameManager.Instance.model;
        yield return model.login(EMail.text, Password.text);

        if (model.errorMessage != null)
        { log.text = model.errorMessage.error; }
        else
        { log.text = model.admin ? "Подключились как администратор" : "Подключились не как администратор!!!!"; }

        preloader.Deactivate();
        GameManager.Instance.saveModel();
    }

    private IEnumerator getRoleProcess()
    {
        preloader.Activate();
        log.text = "Проверка ключа администратора...";
        Model model = GameManager.Instance.model;
        yield return model.getRole();

        log.text = model.admin ? "Да, подключились как администратор" : "Нет, подключились не как администратор";
        preloader.Deactivate();
    }

    public void Login()
    { StartCoroutine(loginProcess()); }

    public void getRole()
    { StartCoroutine(getRoleProcess()); }
}