using CCGKit;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SelectCardWindow : MonoBehaviour, ICardMover
{
    [SerializeField]
    public float cardScaleFactor = 1.0f;

    private Vector3 cardScale;

    [SerializeField]
    public GameObject creatureCardViewMovePrefab;

    private List<MoveCard> cards = new List<MoveCard>();
    private List<MoveCard> reserve = new List<MoveCard>();

    [SerializeField]
    public List<Transform> mixerPH;

    [SerializeField]
    public List<Transform> reservePH;

    [SerializeField]
    public List<BlockerCard> blockers;

    private int deckSize = 5;

    public CardsUpgrade cardsUpgrade;

    public GameObject selectCardsHelp;
    public Transform saveButton;

    public TutorialScript tutorial;

    public GameObject askWindow;
    public DeckSelectorWindow deckSelectorWindow;

    int delta = 0;

    public bool blockReycast;

    MoveCard genCardMovement(CardV3 cardV3, Transform t, int id)
    {
        GameObject go = Instantiate(creatureCardViewMovePrefab as GameObject);
        go.transform.SetParent(t.parent);
        go.transform.SetAsFirstSibling();
        CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();
        cardView.updateCard(GameManager.Instance.model.rules.cardTypes[cardV3.id], cardV3.level, cardV3.p);
        cardView.transform.localScale = cardScale;
        cardView.gameObject.GetComponent<RectTransform>().position = t.position;
        cardView.hpModifier.background.transform.SetParent(t.parent);
        cardView.hpModifier.transform.SetParent(t.parent);
        MoveCard moveCard = go.GetComponent<MoveCard>();
        moveCard.id = id;
        moveCard.dump = cardV3;
        return moveCard;
    }

    private string save;

    public void OpenWindow()
    {
        delta = 0;
        cardScale = new Vector3(cardScaleFactor * BattleUISizeFixer.factor(), cardScaleFactor * BattleUISizeFixer.factor(), cardScaleFactor * BattleUISizeFixer.factor());
        save = Model.toJSON(GameManager.Instance.model.user.decks[0]);
        Debug.Log("GameManager.Instance.model.user.decks[0].cards.Count:" + GameManager.Instance.model.user.decks[0].cards.Count.ToString());
        gameObject.SetActive(true);

        for (int i = 0; i < mixerPH.Count; i++)
        {
            if ((i + 1) < GameManager.Instance.model.user.decks[0].cards.Count)
            {
                CardV3 cardV3 = GameManager.Instance.model.user.decks[0].cards[i + 1];
                if (!cardV3.present()) continue;
                cards.Add(genCardMovement(cardV3, mixerPH[i], i));
            }
            mixerPH[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < Mathf.Max(reservePH.Count, GameManager.Instance.model.user.decks[0].reserve.Count); i++)
        {
            if (i < GameManager.Instance.model.user.decks[0].reserve.Count)
            {
                CardV3 cardV3 = GameManager.Instance.model.user.decks[0].reserve[i];
                if (i > 4)
                {
                    reserve.Add(genCardMovement(cardV3, reservePH[4], i));
                    reserve[i].gameObject.SetActive(false);
                }
                else
                { reserve.Add(genCardMovement(cardV3, reservePH[i], i)); }
            }
            if (i < reservePH.Count) reservePH[i].gameObject.SetActive(false);
        }
        deckSize = GameManager.Instance.model.deckSize();
        if (deckSize >= 5) blockers[2].Hide();
        if (deckSize >= 4) blockers[1].Hide();
        if (deckSize >= 3) blockers[0].Hide();
        /*
        if (!GameManager.Instance.model.cardClickHelpShowed) {
            selectCardsHelp.SetActive(true);
            GameManager.Instance.model.cardClickHelpShowed = true;
        }
        */
        if (GameManager.Instance.model.tutorialScene == 6)
        {
            tutorial.SwitchToNextTutorialScene();
            updateTutorialUI();
        }
    }

    public void updateTutorialUI()
    {
        Debug.Log("Tutorial Scene: " + GameManager.Instance.model.getTutorialScene());
        saveButton.gameObject.SetActive(false);
        switch (GameManager.Instance.model.getTutorialScene())
        {
            case 7:
            case 19:
            case 20:
            case 21:
            case 22:
            tutorial.dark.SetParent(mixerPH[0].parent, true);
            tutorial.dark.SetAsLastSibling();

            foreach (MoveCard t in cards)
                { t.transform.SetAsLastSibling(); }

            foreach (BlockerCard bc in blockers)
                { bc.transform.SetAsLastSibling(); }
            break;

            case 8:
            case 14:
            case 15:
                tutorial.dark.SetParent(mixerPH[0].parent, true);
                tutorial.dark.SetAsLastSibling();
                foreach (MoveCard t in reserve)
                { t.transform.SetAsLastSibling(); }
            break;

            case 9:
            case 10:
                tutorial.dark.SetParent(mixerPH[0].parent, true);
                tutorial.dark.SetAsLastSibling();
                cards[0].transform.SetAsLastSibling();
            break;

            case 16:
                tutorial.dark.SetParent(mixerPH[0].parent, true);
                tutorial.dark.SetAsLastSibling();
                reserve[0].transform.SetAsLastSibling();
            break;

            case 18:
                tutorial.dark.SetParent(mixerPH[0].parent, true);
                tutorial.dark.SetAsLastSibling();
            break;

            case 23:
                tutorial.dark.SetParent(mixerPH[0].parent, true);
                tutorial.dark.SetAsLastSibling();
                saveButton.SetAsLastSibling();
                saveButton.gameObject.SetActive(true);
            break;

            default:
                saveButton.gameObject.SetActive(true);
            break;
        }
    }

    public void CloseWindow()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;
            Destroy(cards[i].gameObject);
        }

        for (int i = 0; i < reserve.Count; i++)
        {
            if (reserve[i] == null) continue;
            Destroy(reserve[i].gameObject);
        }

        gameObject.SetActive(false);
        cards.Clear();
        reserve.Clear();
    }

    public void LeftClick()
    {
        if (delta > 0) delta--;
        animFix();
    }

    public void RightClick()
    {
        if (reserve.Count - delta > 5) delta++;
        animFix();
    }

    public void Cancel()
    {
        GameManager.Instance.model.user.decks[0] = Model.parseJSON<DeckV2>(save);
        CloseWindow();
    }

    public void Apply()
    { SaveData(); }

    private void updateDeckModel()
    {
        GameManager.Instance.model.user.decks[0].cards = GameManager.Instance.model.user.decks[0].cards.GetRange(0, 1);
        GameManager.Instance.model.user.decks[0].reserve.Clear();

        for (int i = 0; i < cards.Count; i++)
        { GameManager.Instance.model.user.decks[0].cards.Add(cards[i].dump); }

        for (int i = 0; i < reserve.Count; i++)
        { GameManager.Instance.model.user.decks[0].reserve.Add(reserve[i].dump); }
    }

    public void ClickSave()
    {
        if (tutorial.getScene() == 23)
        { SaveData(); }
        else
        { askWindow.SetActive(true); }
    }

    private void SaveData()
    { StartCoroutine(saveDeckAndClose()); }

    public IEnumerator saveDeckProcess()
    {
        updateDeckModel();
        yield return GameManager.Instance.model.saveDeck(Model.toJSON(GameManager.Instance.model.user.decks[0]));
    }

    private IEnumerator saveDeckAndClose()
    {
        yield return saveDeckProcess();
        CloseWindow();

        if (tutorial.getScene() == 23)
        {
            deckSelectorWindow.CloseWindow();
            tutorial.SwitchToNextTutorialScene();
        }
    }

    [Obsolete]
    private IEnumerator saveDeck(string data)
    {
        Debug.Log("savedeck id = " + GameManager.Instance.model.userId.ToString());
        string URL = Model.API_URL + "saveDeck/" + GameManager.Instance.model.userId.ToString().ToString() + "/0";
        UnityWebRequest www = new UnityWebRequest(URL);
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.method = UnityWebRequest.kHttpVerbPOST;
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            GameManager.Instance.model.user.decks[0] = Model.parseJSON<DeckV2>(save);
        }
        else
        {
            Debug.Log("save complete!");
            Debug.Log(www.downloadHandler.text);
        }

        CloseWindow();
    }

    public void movementFinish(GameObject card)
    {
        bool reserveMove = true;

        for (int i = 0; i < cards.Count; i++)
        { if (cards[i].gameObject == card) reserveMove = false; }

        Debug.Log("reserveMove:" + reserveMove.ToString());

        float yCards = mixerPH[0].transform.position.y;
        float yNew = card.gameObject.transform.position.y;
        float yReserve = reservePH[0].transform.position.y;

        bool moveToTop;
        bool moveToDown;
        bool simpleMove;

        //if (blockReycast)
        //{
            moveToTop = reserveMove && ((yNew - yCards) > (yReserve - yNew));
            moveToDown = !reserveMove && ((yNew - yCards) < (yReserve - yNew));
            simpleMove = !reserveMove && (!((yNew - yCards) < (yReserve - yNew)));
        //}
        //else
        //{
        //    simpleMove = false;
        //    moveToTop = true;
        //    moveToDown = false;
        //}

        float xNew = card.gameObject.transform.position.x;
        int newId = 0;

        for (int i = 1; i <= (deckSize - 1); i++)
        { if ((mixerPH[i - 1].transform.position.x <= xNew) && (mixerPH[i].transform.position.x >= xNew)) newId = i; }

        if (mixerPH[deckSize - 1].transform.position.x <= xNew) newId = deckSize;

        if (moveToTop)
        {
            if (cards.Count < deckSize)
            {
                int from = card.GetComponent<MoveCard>().id;
                Debug.Log("MoveToTop from:" + from.ToString() + " to:" + newId.ToString());

                if (newId >= cards.Count)
                { cards.Add(reserve[from]); }
                else
                { cards.Insert(newId, reserve[from]); }

                reserve.RemoveAt(from);
            }
        }

        if (moveToDown)
        {
            int from = card.GetComponent<MoveCard>().id;
            newId += delta;
            Debug.Log("MoveToDown from:" + from.ToString() + " to:" + newId.ToString());

            if (newId >= reserve.Count)
            { reserve.Add(cards[from]); }
            else
            { reserve.Insert(newId, cards[from]); }

            cards.RemoveAt(from);
        }

        if (simpleMove)
        {
            int from = card.GetComponent<MoveCard>().id;
            Debug.Log("Simple move:" + from.ToString() + " to:" + newId.ToString());
            MoveCard mc = cards[from];
            cards.RemoveAt(from);
            if (newId >= cards.Count)
            { cards.Add(mc); }
            else
            {
                if (newId > from)
                { cards.Insert(newId - 1, mc); }
                else
                { cards.Insert(newId, mc); }
            }
        }

        if (tutorial.getScene()==21) tutorial.SwitchToNextTutorialScene();

        animFix();
    }

    private void animFix()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < cards.Count)
            {
                cards[i].animTo(mixerPH[i].transform.position);
                cards[i].id = i;
            }
        }
        for (int i = 0; i < reserve.Count; i++)
        {
            reserve[i].gameObject.SetActive(false);
            reserve[i].id = i;
        }
        for (int i = 0; i < 5; i++)
        {
            if (i + delta < reserve.Count)
            {
                reserve[i + delta].gameObject.SetActive(true);
                reserve[i + delta].animTo(reservePH[i].transform.position);
            }
        }
    }

    private bool processTutorial()
    {
        if (tutorial == null) return false;
        switch (GameManager.Instance.model.getTutorialScene())
        {
            case 7:
            case 8:
            case 9:
            case 14:
            case 15:
            case 19:
            case 20:
            case 22:
            case 23:
            case 24:
            tutorial.SwitchToNextTutorialScene();
            return true;
            case 21:
            return true;
        }
        return false;
    }

    public void click(GameObject card)
    { if (!processTutorial()) StartCoroutine(clickEnumerator(card)); }
    GameObject clickedCard;

    private IEnumerator clickEnumerator(GameObject card)
    {
        updateDeckModel();
        clickedCard = card;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].gameObject == card)
            { cardsUpgrade.OpenWindow(DeckPart.cards, i + 1, this); }
        }

        for (int i = delta; i < delta + 5; i++)
        {
            if (i >= reserve.Count) break;
            if (reserve[i].gameObject == card)
            { cardsUpgrade.OpenWindow(DeckPart.reserve, i, this); }
        }
        yield break;
    }

    public void updateCardImage(CardV3 cardV3)
    {
        CreatureCardViewUI cardView = clickedCard.GetComponent<CreatureCardViewUI>();
        cardView.updateCard(GameManager.Instance.model.rules.cardTypes[cardV3.id], cardV3.level, cardV3.p);
        save = Model.toJSON(GameManager.Instance.model.user.decks[0]);
    }
}