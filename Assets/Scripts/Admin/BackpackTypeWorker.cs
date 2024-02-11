using CCGKit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackTypeWorker : MonoBehaviour
{
    public InputField cost;
    public InputField premium1Percent;
    public InputField premium1Value;
    public InputField premium2Percent;
    public InputField premium2Value;
    public InputField exp1Percent;
    public InputField exp1Value;
    public InputField exp2Percent;
    public InputField exp2Value;
    public InputField cards1Percent;
    public InputField cards1Value;
    public InputField cards2Percent;
    public InputField cards2Value;
    public InputField gold1Percent;
    public InputField gold1Value;
    public InputField gold2Percent;
    public InputField gold2Value;

    public void setContent(BackpackContent content)
    {
        cost.text=content.cost.ToString();

        if (content.premium.Count >= 1) premium1Percent.text = content.premium[0].p.ToString(); else premium1Percent.gameObject.SetActive(false);
        if (content.premium.Count >= 1) premium1Value.text = content.premium[0].value.ToString(); else premium1Value.gameObject.SetActive(false);
        if (content.premium.Count >= 2) premium2Percent.text = content.premium[1].p.ToString(); else premium2Percent.gameObject.SetActive(false);
        if (content.premium.Count >= 2) premium2Value.text = content.premium[1].value.ToString(); else premium2Value.gameObject.SetActive(false);

        if (content.exp.Count >= 1) exp1Percent.text = content.exp[0].p.ToString(); else exp1Percent.gameObject.SetActive(false);
        if (content.exp.Count >= 1) exp1Value.text = content.exp[0].value.ToString(); else exp1Value.gameObject.SetActive(false);
        if (content.exp.Count >= 2) exp2Percent.text = content.exp[1].p.ToString(); else exp2Percent.gameObject.SetActive(false);
        if (content.exp.Count >= 2) exp2Value.text = content.exp[1].value.ToString(); else exp2Value.gameObject.SetActive(false);

        if (content.cards.Count >= 1) cards1Percent.text = content.cards[0].p.ToString(); else cards1Percent.gameObject.SetActive(false);
        if (content.cards.Count >= 1) cards1Value.text = content.cards[0].value.ToString(); else cards1Value.gameObject.SetActive(false);
        if (content.cards.Count >= 2) cards2Percent.text = content.cards[1].p.ToString(); else cards2Percent.gameObject.SetActive(false);
        if (content.cards.Count >= 2) cards2Value.text = content.cards[1].value.ToString(); else cards2Value.gameObject.SetActive(false);

        if (content.gold.Count >= 1) gold1Percent.text = content.gold[0].p.ToString(); else gold1Percent.gameObject.SetActive(false);
        if (content.gold.Count >= 1) gold1Value.text = content.gold[0].value.ToString(); else gold1Value.gameObject.SetActive(false);
        if (content.gold.Count >= 2) gold2Percent.text = content.gold[1].p.ToString(); else gold2Percent.gameObject.SetActive(false);
        if (content.gold.Count >= 2) gold2Value.text = content.gold[1].value.ToString(); else gold2Value.gameObject.SetActive(false);
    }

    PValue pValue(InputField v, InputField p)
    { return new PValue(int.Parse(v.text), int.Parse(p.text)); }

    public BackpackContent getContent()
    {
        BackpackContent backpackContent = new BackpackContent();

        backpackContent.cost = Int32.Parse(cost.text);

        backpackContent.premium = new List<PValue>();
        backpackContent.exp = new List<PValue>();
        backpackContent.cards = new List<PValue>();
        backpackContent.gold = new List<PValue>();

        if (premium1Percent.gameObject.activeInHierarchy) backpackContent.premium.Add(pValue(premium1Value, premium1Percent));
        if (premium2Percent.gameObject.activeInHierarchy) backpackContent.premium.Add(pValue(premium2Value, premium2Percent));

        if (exp1Percent.gameObject.activeInHierarchy) backpackContent.exp.Add(pValue(exp1Value, exp1Percent));
        if (exp2Percent.gameObject.activeInHierarchy) backpackContent.exp.Add(pValue(exp2Value, exp2Percent));

        if (cards1Percent.gameObject.activeInHierarchy) backpackContent.cards.Add(pValue(cards1Value, cards1Percent));
        if (cards2Percent.gameObject.activeInHierarchy) backpackContent.cards.Add(pValue(cards2Value, cards2Percent));

        if (gold1Percent.gameObject.activeInHierarchy) backpackContent.gold.Add(pValue(gold1Value, gold1Percent));
        if (gold2Percent.gameObject.activeInHierarchy) backpackContent.gold.Add(pValue(gold2Value, gold2Percent));

        return backpackContent;
    }
}