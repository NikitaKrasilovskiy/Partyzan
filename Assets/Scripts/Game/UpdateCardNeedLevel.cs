using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCardNeedLevel : MonoBehaviour
{
    public TranslatorScript text;

    public void Show()
    {
        gameObject.SetActive(true);
        text.SetValue(GameManager.Instance.model.openCellLevel());
    }
}