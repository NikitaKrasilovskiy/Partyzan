using CCGKit;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditCardListScene : MonoBehaviour
{
    Model model;
    public GameObject creatureCardViewPrefab;
    public List<Transform> cardPositions;

    void Start ()
    {
        Assert.IsNotNull(cardPositions);
        model = GameManager.Instance.model;
        StartCoroutine(loadCards());
	}

    IEnumerator loadCards()
    {
        yield return model.loadRules();
        int i = 0;
        foreach (CardType ct in model.rules.cardTypes)
        {
            GameObject go = Instantiate(creatureCardViewPrefab as GameObject);
            CreatureCardViewUI cardView = go.GetComponent<CreatureCardViewUI>();

            cardView.updateCard(ct,1,0);
            cardView.transform.SetParent(creatureCardViewPrefab.transform.parent);
            cardView.transform.position = cardPositions[i].position;
            cardView.transform.localScale = creatureCardViewPrefab.transform.localScale;
            cardView.updateTexture(ct.image);
            cardView.setCountry(ct.country);

            cardPositions[i].gameObject.SetActive(false);
            i++;
        }
        creatureCardViewPrefab.SetActive(false);
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 5f;

            Vector2 v = Camera.main.ScreenToWorldPoint(mousePosition);
            Collider2D[] col = Physics2D.OverlapPointAll(v);

            if (col.Length > 0)
            {
                foreach (Collider2D c in col)
                {
                    CreatureCardViewUI ccv = c.gameObject.GetComponent<CreatureCardViewUI>();
                    if (ccv != null)
                    {
                        model.editCard(ccv.getCardID());
                        SceneManager.LoadScene("EditCard");
                    }
                }
            }
        }
    }

    public void newCard()
    {
        model.editCard(-1);
        SceneManager.LoadScene("EditCard");
    }

    public void edit(GameObject go)
    {
        CreatureCardViewUI ccv = go.GetComponent<CreatureCardViewUI>();
        Debug.Log(ccv.getCardID());
        model.editCard(ccv.getCardID());
        SceneManager.LoadScene("EditCard");
    }
}