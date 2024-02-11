using CCGKit;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public bool dragOnSurfaces = true;

    private bool drag = false;

    public int id;

    [SerializeField]
    public MonoBehaviour cardMover;

    [NonSerialized]
    public CardV3 dump;

    private Transform m_DraggingPlane;

    void Start ()
    { m_DraggingPlane = gameObject.transform; }

    private static int CLICK_COUNTER_DEFAULT = 3;
    private int clickCounter = CLICK_COUNTER_DEFAULT;

    void Update ()
    {
		if (anim)
        {
            time += Time.deltaTime;
            var rt = gameObject.GetComponent<Transform>();
            if (time > T)
            {
                rt.position = to;
                anim = false;
            }
            else
            { rt.position = Vector3.Lerp(from, to, time/T); }
        }
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        TutorialScript ts = FindObjectOfType<TutorialScript>();
        if (ts != null)
        {
            if (ts.gameObject.activeSelf)
                return;
        }

        if (anim) return;
        drag = true;
        gameObject.transform.SetAsLastSibling();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        { delta = globalMousePos - m_DraggingPlane.position; }

        clickCounter = CLICK_COUNTER_DEFAULT;
    }

    public void OnDrag(PointerEventData data)
    {
        if (anim) return;
        if (!drag) return;

        var rt = gameObject.GetComponent<Transform>();
        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane as RectTransform, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos-delta;
            rt.rotation = m_DraggingPlane.rotation;
        }

        clickCounter--;
    }
    Vector3 delta;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (anim) return;
        drag = false;
        (cardMover.GetComponent<ICardMover>()).movementFinish(gameObject);
    }

    private float time = 0;
    private float T = 0.2f;
    private bool anim = false;
    private Vector3 from;
    private Vector3 to;
    internal int ind;

    public void animTo(Vector3 position)
    {
        anim = true;
        from = gameObject.GetComponent<Transform>().position;
        to = position;
        time = 0;
    }

    internal int x()
    { return (int)(gameObject.GetComponent<Transform>().position.x*1000); }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickCounter <= 0)
        {
            clickCounter = CLICK_COUNTER_DEFAULT;
            return;
        }
        (cardMover.GetComponent<ICardMover>()).click(gameObject);
    }
}