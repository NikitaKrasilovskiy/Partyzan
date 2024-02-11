using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectorFlag : MonoBehaviour
{
    public GameObject Active;
    public GameObject Passive;
    public GameObject Selector;
    public GameObject ActiveSelector;
    public GameObject SelectorBack;
    public GameObject Blocked;
    public PreloaderAmination preloader;
    public int value;
    public SelectCardWindow selectCardWindow;
    public TutorialScript tutorialWindow;
    private bool active;
    private Model model;

    void Start ()
    { UpdateData(); }

	void Update ()
    {
       // ActiveSelector.SetActive(model.user.activeDeck == value);
    }

    public void UpdateData()
    {
        model = GameManager.Instance.model;
        active = model.user.decks.Count > value;
        Active.SetActive(active);
        Selector.SetActive(active);
        SelectorBack.SetActive(active);
        Passive.SetActive(!active);
        Blocked.SetActive(!active);
        GetComponent<MenuButton>().enabled = active;
        Update();
    }

    public void ClickSelector()
    { StartCoroutine(activeDeck()); }

    private IEnumerator activeDeck()
    {
        preloader.Activate();
        yield return model.activeDeck(value);

        preloader.Deactivate();
    }

    public void Click()
    { selectCardWindow.OpenWindow(); }
}