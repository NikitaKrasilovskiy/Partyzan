using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDef
{
    [SerializeField] public int attack;
    [SerializeField] public int hp;
    [SerializeField] public int sell;
}

[Serializable]
public class UpgradeCost
{
    [SerializeField]
    public int gold;
}

[Serializable]
public class Ability
{
    [SerializeField] public string[] text;
    [SerializeField] public int[] values;
    [SerializeField] public int icon;
}

public class CardType
{
    [SerializeField] public int id;
    [SerializeField] public string[] title;
    [SerializeField] public string description;
    [SerializeField] public string image;
    [SerializeField] public int country;

    [SerializeField] public CardDef[] cardDefs;
    [SerializeField] public UpgradeCost[] upgradeCost;
    [SerializeField] public Ability[] abilities;

    [SerializeField] public string attack_sound1;
    [SerializeField] public string attack_sound2;

    public CardType()
    {
        id = -1;
        title = new string[3];
        description = "";
        image = "";

        cardDefs = new CardDef[3];
        upgradeCost = new UpgradeCost[2];
    }

    internal bool isNewCard()
    { return id == -1; }

    internal bool isGeneral()
    { return ((id == 0) || (id == 6) || (id == 12)); }
}