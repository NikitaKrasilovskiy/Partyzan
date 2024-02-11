using CCGKit;
using UnityEngine.Assertions;
using System;
using System.Collections;
using uFileBrowser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditScene : MonoBehaviour
{
    Model model;

    public InputField titleRU;
    public InputField titleEN;
    public InputField titlePL;
    public InputField imageURL;
    public InputField Sound1URL;
    public InputField Sound2URL;

    public InputField description;

    public InputField attack1;
    public InputField hp1;

    public InputField upgrade1;

    public InputField attack2;
    public InputField hp2;

    public InputField level2DescriptionRU;
    public InputField level2DescriptionEN;
    public InputField level2DescriptionPL;
    public InputField level2Param1;
    public InputField level2Param2;
    public InputField level2Param3;

    public InputField upgrade2;

    public InputField attack3;
    public InputField hp3;

    public InputField level3DescriptionRU;
    public InputField level3DescriptionEN;
    public InputField level3DescriptionPL;
    public InputField level3Param1;
    public InputField level3Param2;
    public InputField level3Param3;

    public CreatureCardViewUI level1;
    public CreatureCardViewUI level2;
    public CreatureCardViewUI level3;

    public FileBrowser fileBrowser;

    public Dropdown country;

    private bool dataSet = false;

    void Start ()
    {
        Assert.IsNotNull(titleRU);
        Assert.IsNotNull(titleEN);
        Assert.IsNotNull(titlePL);

        Assert.IsNotNull(level1);
        Assert.IsNotNull(level2);
        Assert.IsNotNull(level3);

        Assert.IsNotNull(description);
        Assert.IsNotNull(attack1);
        Assert.IsNotNull(hp1);
        Assert.IsNotNull(upgrade1);
        Assert.IsNotNull(attack2);
        Assert.IsNotNull(hp2);
        Assert.IsNotNull(upgrade2);
        Assert.IsNotNull(attack3);
        Assert.IsNotNull(hp3);

        Assert.IsNotNull(fileBrowser);
        Assert.IsNotNull(country);

        model = GameManager.Instance.model;

        updateTitleAndImage();
        applyCardInfo();

        dataSet = true;
    }

    internal CardType card()
    { return model.getCurrentEditorCard(); }

    public void saveData()
    {
        collectDataFromScreen();
        StartCoroutine(model.saveCards());
    }

    public void returnToList()
    { SceneManager.LoadScene("EditorCardList"); }

    [Obsolete]
    public void upload()
    { upload(fileBrowser.AddressPath + fileBrowser.FileName); }

    [Obsolete]
    public void upload(string file)
    { StartCoroutine(uploadFile(file)); }

    [Obsolete]
    IEnumerator uploadFile(string file)
    {
        yield return model.uploadFile(file);
        card().image = model.uploadedResult;
        updateTitleAndImage();
    }

    void updateTitleAndImage()
    {
        Debug.Log(card());
        Debug.Log(card().image);
        level1.updateTexture(card().image);
        level2.updateTexture(card().image);
        level3.updateTexture(card().image);
        imageURL.text = card().image;
        Sound1URL.text = card().attack_sound1;
        Sound2URL.text = card().attack_sound2;
    }

    int getInt(string str)
    {
        if (str == null) return 1;
        if (str == "") return 1;
        try
        { return Convert.ToInt32(str); }
        catch(Exception)
        { return 1; }
    }

    void collectDataFromScreen()
    {
        card().title[0] = titleRU.text;
        card().title[1] = titleEN.text;
        card().title[2] = titlePL.text;

        card().image = imageURL.text;
        card().description = description.text;
        card().country = country.value;
        card().attack_sound1 = Sound1URL.text;
        card().attack_sound2 = Sound2URL.text;

        if (card().cardDefs[0] == null) card().cardDefs[0] = new CardDef();
        card().cardDefs[0].attack = getInt(attack1.text);
        card().cardDefs[0].hp = getInt(hp1.text);

        if (card().upgradeCost[0] == null) card().upgradeCost[0] = new UpgradeCost();
        card().upgradeCost[0].gold = getInt(upgrade1.text);

        if (card().cardDefs[1] == null) card().cardDefs[1] = new CardDef();
        card().cardDefs[1].attack = getInt(attack2.text);
        card().cardDefs[1].hp = getInt(hp2.text);

        card().abilities[0].text[0] = level2DescriptionRU.text;
        card().abilities[0].text[1] = level2DescriptionEN.text;
        card().abilities[0].text[2] = level2DescriptionPL.text;
        if (level2Param1.gameObject.activeInHierarchy) card().abilities[0].values[0] = Int32.Parse(level2Param1.text);
        if (level2Param2.gameObject.activeInHierarchy) card().abilities[0].values[1] = Int32.Parse(level2Param2.text);
        if (level2Param3.gameObject.activeInHierarchy) card().abilities[0].values[2] = Int32.Parse(level2Param3.text);

        if (card().upgradeCost[1] == null) card().upgradeCost[1] = new UpgradeCost();
        card().upgradeCost[1].gold = getInt(upgrade2.text);

        if (card().cardDefs[2] == null) card().cardDefs[2] = new CardDef();
        card().cardDefs[2].attack = getInt(attack3.text);
        card().cardDefs[2].hp = getInt(hp3.text);

        card().abilities[1].text[0] = level3DescriptionRU.text;
        card().abilities[1].text[1] = level3DescriptionEN.text;
        card().abilities[1].text[2] = level3DescriptionPL.text;
        if (level3Param1.gameObject.activeInHierarchy) card().abilities[1].values[0] = Int32.Parse(level3Param1.text);
        if (level3Param2.gameObject.activeInHierarchy) card().abilities[1].values[1] = Int32.Parse(level3Param2.text);
        if (level3Param3.gameObject.activeInHierarchy) card().abilities[1].values[2] = Int32.Parse(level3Param3.text);
    }

    void applyCardInfo()
    {
        Debug.Log(Model.toJSON(card()));

        titleRU.text = card().title[0];
        titleEN.text = card().title[1];
        titlePL.text = card().title[2];

        imageURL.text = card().image;
        description.text = card().description;
        country.value = card().country;

        if (card().cardDefs[0] != null)
        {
            level1.updateCard(card(),1,0);
            attack1.text = card().cardDefs[0].attack.ToString();
            hp1.text = card().cardDefs[0].hp.ToString();
        }

        if (card().upgradeCost[0] != null)
        { upgrade1.text = card().upgradeCost[0].gold.ToString(); }

        if (card().cardDefs[1] != null)
        {
            level2.updateCard(card(), 2, 0);
            attack2.text = card().cardDefs[1].attack.ToString();
            hp2.text = card().cardDefs[1].hp.ToString();
            level2DescriptionRU.text = card().abilities[0].text[0];
            level2DescriptionEN.text = card().abilities[0].text[1];
            level2DescriptionPL.text = card().abilities[0].text[2];

            if (card().abilities[0].values.Length < 1)
                level2Param1.gameObject.SetActive(false);
            else
            {
                level2Param1.gameObject.SetActive(true);
                level2Param1.text = card().abilities[0].values[0].ToString();
            }

            if (card().abilities[0].values.Length < 2)
                level2Param2.gameObject.SetActive(false);
            else
            {
                level2Param2.gameObject.SetActive(true);
                level2Param2.text = card().abilities[0].values[1].ToString();
            }

            if (card().abilities[0].values.Length < 3)
            { level2Param3.gameObject.SetActive(false); }
            else
            {
                level2Param3.gameObject.SetActive(true);
                level2Param3.text = card().abilities[0].values[2].ToString();
            }
        }

        if (card().upgradeCost[1] != null)
        { upgrade2.text = card().upgradeCost[1].gold.ToString(); }

        if (card().cardDefs[2] != null)
        {
            level3.updateCard(card(), 3, 0);
            attack3.text = card().cardDefs[2].attack.ToString();
            hp3.text = card().cardDefs[2].hp.ToString();
            level3DescriptionRU.text = card().abilities[1].text[0];
            level3DescriptionEN.text = card().abilities[1].text[1];
            level3DescriptionPL.text = card().abilities[1].text[2];

            if (card().abilities[1].values.Length < 1)
                level3Param1.gameObject.SetActive(false);
            else
            {
                level3Param1.gameObject.SetActive(true);
                level3Param1.text = card().abilities[1].values[0].ToString();
            }

            if (card().abilities[1].values.Length < 2)
                level3Param2.gameObject.SetActive(false);
            else
            {
                level3Param2.gameObject.SetActive(true);
                level3Param2.text = card().abilities[1].values[1].ToString();
            }

            if (card().abilities[1].values.Length < 3)
                level3Param3.gameObject.SetActive(false);
            else
            {
                level3Param3.gameObject.SetActive(true);
                level3Param3.text = card().abilities[1].values[2].ToString();
            }
        }
    }

    public void updateCards()
    {
        if (dataSet)
        {
            collectDataFromScreen();
            applyCardInfo();
        }
    }
}