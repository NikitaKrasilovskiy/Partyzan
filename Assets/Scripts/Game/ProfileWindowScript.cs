using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileWindowScript : MonoBehaviour
{
    public TMP_InputField Name;
    public Slider Level;
    public Slider Pro;
    public TextMeshProUGUI LevelNumber;
    public TextMeshProUGUI ProNumber;

    public TranslatorScript Wins;
    public TranslatorScript Fails;
    public TranslatorScript Time;

    public TextMeshProUGUI[] names;

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        updateData();
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    public void Save()
    { StartCoroutine(SaveProcess()); }

    public void SaveAndClose()
    { StartCoroutine(SaveAndCloseProcess()); }

    void updateData()
    {
        Model model = GameManager.Instance.model;
        User user = model.user;
        Name.text = user.name;
        Level.maxValue = user.needExp;
        Level.value = user.exp;
        Pro.maxValue = 30 * 24 * 60 * 60;
        Pro.value = model.remainProTime();
        LevelNumber.text = user.level.ToString();
        ProNumber.text = model.remainProDays().ToString();
        Wins.SetValue(user.wins);
        Fails.SetValue(user.fails);
        Time.SetValue(user.totalTime / 60 / 60);
    }

    private IEnumerator SaveProcess()
    {
        string name = Name.text;
        if (name == "") yield break;
        Model model = GameManager.Instance.model;
        yield return model.updateName(name);
        Name.text = model.user.name;

        foreach (TextMeshProUGUI t in names)
        { t.text = model.user.name; }
    }

    private IEnumerator SaveAndCloseProcess()
    {
        yield return SaveProcess();
        CloseWindow();
    }
}