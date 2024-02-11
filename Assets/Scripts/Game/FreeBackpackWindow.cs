using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FreeBackpackWindow : MonoBehaviour
{
    public List<TextMeshProUGUI> Count;
    public PreloaderAmination preloader;

    public void OpenWindow()
    {
        if (GameManager.Instance.model.canGetFreeBackpack())
        { GetBackpack(); }
        ShowState();
    }

    public void CloseWindow()
    {
        ShowState();
        gameObject.SetActive(false);
    }

    private void ShowState()
    { UpdateText(Count, GameManager.Instance.model.user.backpacks); }

    public void GetBackpack()
    {
        gameObject.SetActive(true);
        StartCoroutine(GetBackpackProcess());
    }

    private IEnumerator GetBackpackProcess()
    {
        preloader.Activate();
        yield return GameManager.Instance.model.getFreeBackpack();
        preloader.Deactivate();
        ShowState();
    }

    private void UpdateText(List<TextMeshProUGUI> gui, int value)
    {
        if (gui != null)
        {
            foreach (TextMeshProUGUI t in gui)
                t.text = value.ToString();
        }
    }
}