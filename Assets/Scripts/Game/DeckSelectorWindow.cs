using UnityEngine;

public class DeckSelectorWindow : MonoBehaviour
{
    public DeckSelectorFlag[] decks;

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        foreach(DeckSelectorFlag deckSelectorFlag in decks)
        { deckSelectorFlag.UpdateData(); }
    }
    public void CloseWindow()
    { gameObject.SetActive(false); }
}