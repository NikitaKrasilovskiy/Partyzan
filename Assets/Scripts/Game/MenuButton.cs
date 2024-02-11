using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using CCGKit;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointer
{
    [Range(0.0f, 1.0f)]
    public float glowIntensity = 0.3f;

    [Range(0.0f, 1.0f)]
    public float clickIntensity = 0.4f;

    [SerializeField]
    protected Image onHoverGlow;

    [SerializeField]
    protected Image onHoverOverlay;

    [SerializeField]
    protected AudioSource audioSource;

    [SerializeField]
    protected AudioClip clip;

    [SerializeField]
    public bool interactable = true;

    public UnityEvent onClickEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverGlow!=null) onHoverGlow.DOKill();
        if (onHoverOverlay != null) onHoverOverlay.DOKill();
        if (onHoverGlow != null) onHoverGlow.DOFade(glowIntensity, 0.5f);
        if (onHoverOverlay != null) onHoverOverlay.DOFade(1.0f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onHoverGlow != null) onHoverGlow.DOKill();
        if (onHoverOverlay != null) onHoverOverlay.DOKill();
        if (onHoverGlow != null) onHoverGlow.DOFade(0.0f, 0.25f);
        if (onHoverOverlay != null) onHoverOverlay.DOFade(0.0f, 0.25f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickEvent != null && interactable) onClickEvent.Invoke();
        if (clip!=null) GameManager.Instance.PlaySound(audioSource, clip);
        if (onHoverGlow != null) onHoverGlow.DOFade(0, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onHoverGlow != null) onHoverGlow.DOKill();
        if (onClickEvent != null) onHoverOverlay.DOKill();
        if (onHoverGlow != null) onHoverGlow.DOFade(clickIntensity, 0);
        if (onHoverOverlay != null) onHoverOverlay.DOFade(1.0f, 0.5f);
    }
}