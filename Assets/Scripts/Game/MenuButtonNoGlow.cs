using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using CCGKit;

public class MenuButtonNoGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    protected Image onHoverOverlay;

    [SerializeField]
    protected AudioSource audioSource;

    [SerializeField]
    protected AudioClip clip;

    public UnityEvent onClickEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverOverlay != null) onHoverOverlay.DOKill();
        if (onHoverOverlay != null) onHoverOverlay.DOFade(1.0f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onHoverOverlay != null) onHoverOverlay.DOKill();
        if (onHoverOverlay != null) onHoverOverlay.DOFade(0.0f, 0.25f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((audioSource != null) && (clip != null)) GameManager.Instance.PlaySound(audioSource, clip);
        if (onClickEvent != null) onClickEvent.Invoke();
    }
}