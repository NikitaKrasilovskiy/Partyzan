using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CCGKit;

public class RaycastBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    SelectCardWindow cardMover;

    public void OnPointerEnter(PointerEventData eventData)
    { cardMover.blockReycast = true; }

    public void OnPointerExit(PointerEventData eventData)
    { cardMover.blockReycast = false; }
}