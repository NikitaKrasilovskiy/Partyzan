﻿using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerManaBar : MonoBehaviour
{
    public TextMeshPro manaText;

    public List<GameObject> manaIcons;

    public void SetMana(int mana)
    {
        manaText.text = mana + "/10";

        for (var i = 0; i < manaIcons.Count; i++)
        {
            if (i < mana)
            { manaIcons[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().DOFade(1.0f, 0.5f); }
            else
            { manaIcons[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().DOFade(0.0f, 0.5f); }
        }
    }
}