using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArenaEditorScript : MonoBehaviour
{
    public InputField EloForAllInput;
    public TextMeshProUGUI message;
    public PreloaderAmination preloader;
    Model model;

    void Start ()
    { model = GameManager.Instance.model; }

    public void SetEloForAll()
    {
        if (EloForAllInput.text == "")
        {
            message.text = "Пустое окно ввода";
            return;
        }

        float elo = float.Parse(EloForAllInput.text);

        if (elo <= 0)
        {
            message.text = "Рейтинг <= 0";
            return;
        }

        StartCoroutine(SetEloForAllProcess(elo));
    }

    IEnumerator SetEloForAllProcess(float elo)
    {
        preloader.Activate();
        message.text = "Запрос";
        yield return model.setEloForAll(elo);
        message.text = model.setEloForAllResult;
        preloader.Deactivate();
    }
}