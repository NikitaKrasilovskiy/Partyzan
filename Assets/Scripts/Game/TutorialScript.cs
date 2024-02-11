using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    Model model;
    
    public Transform deckButton;
    private int deckButtonDepth = -1;

    public Transform campaignButton;
    private int campaignButtonDepth = -1;

    public Transform ratingButton;
    private int ratingButtonDepth = -1;

    public Transform clanButton;
    private int clanButtonDepth = -1;

    public Transform tasksButton;
    private int tasksButtonDepth = -1;

    //public Transform bonusButton;
    //private int bonusButtonDepth = -1;

    public Transform upgradeLine;
    private int upgradeLineDepth = -1;

    private Transform par;

    public Transform DeckSelectorWindow;
    public Transform DeckEditorWindow;
    public SelectCardWindow selectCardWindow;
    public CardsUpgrade cardsUpgrade;
    public BuyStarterPackWindow buyStarterPack;
    public BackPuckController openBackpackWindow;
    public TasksWindow tasksWindow;

    public GameObject lang, tutorialExit;

    [System.Serializable]
    public class TutorialFrame
    {
        public int id;
        public GameObject visual;
        public AudioClip ruSound;
    }

    public List<TutorialFrame> tutorialFrames;

    public Transform dark;

    public AudioSource audioSource;

    IEnumerator Start()
    {
        if (model.user.level > 1 || GameManager.Instance.model.tutorialScene > 36 )
        Destroy(this.gameObject);

        while (GameManager.Instance.model.user==null)
            yield return true;
        
        model = GameManager.Instance.model;
    }

    //float t = 2;
    //void Update()
    //{
        //if (getScene() != 28)
        //{
        //    t -= Time.deltaTime;
        //    getFrameGameObject(28).SetActive(t < 0);
        //}
    //}
    
    public void showState()
    {
        if (model.user.level > 1)
            Destroy(this.gameObject);
        
        model = GameManager.Instance.model;
        RestoreUI();
        if (/* model.tutorialComplete()  || */ model.user.level > 1 || model.tutorialScene > 36)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            hideTutorialFrames();
            GameObject currentVisual = getFrameGameObject(model.getTutorialScene());

            if (currentVisual != null)
            {
                currentVisual.SetActive(true);
                ApplyCurrentUI();
            }

            if (audioSource != null)
            {
                AudioClip audioClip = getFrameSound(model.getTutorialScene());

                if (audioSource != null)
                {
                    //GameManager.Instance.PlaySound(audioSource, audioClip);
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }
        }
    }

    private void RestoreUI()
    {
        dark.SetParent(transform, true);
        dark.SetSiblingIndex(0);
        if (deckButtonDepth != -1)
        {
            deckButton.SetSiblingIndex(deckButtonDepth);
            deckButtonDepth = -1;
        }
        if (campaignButtonDepth != -1)
        {
            campaignButton.SetSiblingIndex(campaignButtonDepth);
            campaignButtonDepth = -1;
        }
        if (ratingButtonDepth != -1)
        {
            ratingButton.SetSiblingIndex(ratingButtonDepth);
            ratingButtonDepth = -1;
        }
        if (clanButtonDepth != -1)
        {
            clanButton.SetSiblingIndex(clanButtonDepth);
            clanButtonDepth = -1;
        }
        if (tasksButtonDepth != -1)
        {
            tasksButton.SetSiblingIndex(tasksButtonDepth);
            tasksButtonDepth = -1;
        }
        //if (bonusButtonDepth != -1)
        //{
        //    bonusButton.SetSiblingIndex(bonusButtonDepth);
        //    bonusButtonDepth = -1;
        //}
        if (upgradeLineDepth != -1)
        {
            //upgradeLine.SetSiblingIndex(bonusButtonDepth);
            upgradeLineDepth = -1;
        }
    }

    private void ApplyCurrentUI()
    {
        
        model = GameManager.Instance.model;
        switch (model.getTutorialScene())
        {
            case 1:
            case 2:
                if (tasksWindow != null) tasksWindow.gameObject.SetActive(false);
                break;

            case 5:
                deckButtonDepth = deckButton.GetSiblingIndex();
                deckButton.SetSiblingIndex(deckButton.parent.childCount - 1);
                break;

            case 6:
                dark.SetParent(DeckSelectorWindow, true);
                dark.SetSiblingIndex(dark.parent.childCount - 2);
                break;

            case 7: selectCardWindow.updateTutorialUI(); break;

            case 8: selectCardWindow.updateTutorialUI(); break;

            case 9: selectCardWindow.updateTutorialUI(); break;

            case 10: selectCardWindow.updateTutorialUI(); break;

            case 11: cardsUpgrade.updateTutorialUI(); break;

            case 12: cardsUpgrade.updateTutorialUI(); break;

            case 13:
                cardsUpgrade.updateTutorialUI();
                par = upgradeLine.parent;
                upgradeLine.SetParent(this.transform);
                upgradeLine.SetAsFirstSibling();
                break;

            case 14:
                upgradeLine.SetParent(par);
                cardsUpgrade.updateTutorialUI();
                selectCardWindow.updateTutorialUI();
                break;

            case 15: selectCardWindow.updateTutorialUI(); break;

            case 16: selectCardWindow.updateTutorialUI(); break;

            case 17: cardsUpgrade.updateTutorialUI(); break;

            case 18: selectCardWindow.updateTutorialUI(); break;

            case 19: selectCardWindow.updateTutorialUI(); break;

            case 20: selectCardWindow.updateTutorialUI(); break;

            case 21: selectCardWindow.updateTutorialUI(); break;

            case 22: selectCardWindow.updateTutorialUI(); break;

            case 23: selectCardWindow.updateTutorialUI(); break;

            case 25:
                campaignButtonDepth = campaignButton.GetSiblingIndex();
                campaignButton.SetSiblingIndex(campaignButton.parent.childCount - 1);
                break;

            case 29:
            case 30:
                ratingButtonDepth = ratingButton.GetSiblingIndex();
                ratingButton.SetSiblingIndex(ratingButton.parent.childCount - 1);
                break;

            case 31:
                clanButtonDepth = clanButton.GetSiblingIndex();
                clanButton.SetSiblingIndex(clanButton.parent.childCount - 1);
                break;

            case 32:
            case 33:
                tasksButtonDepth = tasksButton.GetSiblingIndex();
                tasksButton.SetSiblingIndex(tasksButton.parent.childCount - 1);
                break;

            case 34:
            case 35:
                //bonusButtonDepth = bonusButton.GetSiblingIndex();
                //bonusButton.SetSiblingIndex(bonusButton.parent.childCount - 1);
                break;

            //case 38:
            //    dark.SetParent(transform.parent, true);
            //    dark.SetSiblingIndex(10);
            //    break;
        }
    }

    public void Click()
    {
        Debug.Log("Click on " + model.getTutorialScene());

        if (model.user.level > 1)
            Destroy(this.gameObject);

        switch (model.getTutorialScene())
        {
            case 1: SwitchToNextTutorialScene(); break;
            case 2: SwitchToNextTutorialScene(); break;
            case 3: SwitchToNextTutorialScene(); break;
            case 4: SwitchToNextTutorialScene(); break;
            case 7: SwitchToNextTutorialScene(); break;
            case 8: SwitchToNextTutorialScene(); break;
            case 9: SwitchToNextTutorialScene(); break;
            case 11: SwitchToNextTutorialScene(); break;
            case 13: SwitchToNextTutorialScene(); break;
            case 14: SwitchToNextTutorialScene(); break;
            case 15: SwitchToNextTutorialScene(); break;
            //case 16: SwitchToNextTutorialScene(); break; // Закомментировал, чтоб не было выхода с туторила.
            case 17: SwitchToNextTutorialScene(); break;
            case 18: SwitchToNextTutorialScene(); break;
            case 19: SwitchToNextTutorialScene(); break;
            case 20: SwitchToNextTutorialScene(); break;
            case 22: SwitchToNextTutorialScene(); break;
            case 24: SwitchToNextTutorialScene(); break;
            case 27: gameObject.SetActive(false); break;
            case 28: gameObject.SetActive(false); break;
            case 29: SwitchToNextTutorialScene(); break;
            case 30: SwitchToNextTutorialScene(); break;
            case 31: SwitchToNextTutorialScene(); break;
            case 32: SwitchToNextTutorialScene(); break;
            case 33: SwitchToNextTutorialScene(); break;
            case 34: SwitchToNextTutorialScene(); break;
            case 35: SwitchToNextTutorialScene(); break;
            case 36: SwitchToNextTutorialScene(); break;
                //case 37: SwitchToNextTutorialScene(); break;
                //case 39: SwitchToNextTutorialScene(); break;
        }
    }

    public void SwitchToNextTutorialScene()
    {
        Debug.Log("Scene: " + getScene());
        bool needSave = false;
        switch (getScene())
        {
            case 36: needSave = true; break;
                //case 37: StartCoroutine(getStarterPackContent()); break;
                //case 39:
                //    {
                //        Invoke("openBuyStarterPack", 3.0f);
                //        needSave = true;
                //    }
                //    break;
        }
        
        model.nextTutorialScene();

        if (needSave)
        { Debug.Log("Сохранение"); GameManager.Instance.saveModel(); }
        
        Debug.Log("Next Scene: " + getScene());
        showState();
    }

    //IEnumerator getStarterPackContent()
    //{
    //    yield return model.getStarterPackContent();

    //    if (model.starterPackContent.result == 0)
    //        openBackpackWindow.OpenTutorialBackpack();
    //    else
    //        SwitchToNextTutorialScene();
    //}

    void openBuyStarterPack()
    { buyStarterPack.OpenWindow(); }

    void hideTutorialFrames()
    {
        foreach (TutorialFrame tf in tutorialFrames)
        { tf.visual.SetActive(false); }
    }

    GameObject getFrameGameObject(int id)
    {
        foreach (TutorialFrame tf in tutorialFrames)
        { if (tf.id == id) return tf.visual; }

        return null;
    }

    AudioClip getFrameSound(int id)
    {
        AudioClip cl = tutorialFrames.Find((tf) => tf.id == id).ruSound;
        //return cl;
        foreach (TutorialFrame tf in tutorialFrames)
        {
            if (tf.id == id) return tf.ruSound;
        }
        return null;
    }

    public void restart()
    {
        model.restartTutorial();
        showState();
    }

    public int getScene()
    {
	    //return -1;
        model = GameManager.Instance.model;
        return model.getTutorialScene();
    }
}