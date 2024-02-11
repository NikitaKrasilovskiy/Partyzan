using CCGKit;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoginRegisterWindow : MonoBehaviour
{
    private bool isRegister = false;

    public TextMeshProUGUI Login;
    public TextMeshProUGUI Register;

    public TMP_InputField EMail;
    public TMP_InputField Password;

    public ErrorMessage errorMessage;
    public SettingsWindowScript settingsWindowScript;
    public HomeScene homeScene;

    private Model model;

    void Start ()
    { model = GameManager.Instance.model; }

    public void OpenRegisterWindow()
    {
        //isRegister = true;
        //gameObject.SetActive(true);
        Login.gameObject.SetActive(false);
        //Register.gameObject.SetActive(true);
        //errorMessage.CloseWindow();
    }

    public void OpenLoginWindow()
    {
        isRegister = false;
        gameObject.SetActive(true);
        Login.gameObject.SetActive(true);
        Register.gameObject.SetActive(false);
        errorMessage.CloseWindow();
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    public void OnClickLoginOrRegister()
    {
        if (EMail.text == null) return;

        if (Password.text == null) return;

        if (isRegister)
        { register(); }
        else
        { login(); }
    }

    public void login()
    { StartCoroutine(loginProcess()); }

    public void register()
    { StartCoroutine(registerProcess()); }

    private IEnumerator loginProcess()
    {
        yield return model.login(EMail.text, Password.text);
        PlayerPrefs.SetString("login", EMail.text);
        PlayerPrefs.SetString("password", Password.text);

        if (model.errorMessage != null)
        {
            errorMessage.ShowMessage(model.errorMessage.error);
            model.errorMessage = null;
        }
        else
        {
            settingsWindowScript.OpenWindow();
            homeScene.updateText();
            CloseWindow();
        }
    }

    private IEnumerator registerProcess()
    {
        yield return model.singup(EMail.text, Password.text);
        PlayerPrefs.SetString("login", EMail.text);
        PlayerPrefs.SetString("password", Password.text);

        if (model.errorMessage != null)
        {
            errorMessage.ShowMessage(model.errorMessage.error);
            model.errorMessage = null;
        }
        else
        {
            settingsWindowScript.OpenWindow();
            CloseWindow();
        }
    }
}