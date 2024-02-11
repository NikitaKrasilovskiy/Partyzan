using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatViewer : MonoBehaviour
{
    [SerializeField]
    GameObject incomeMessagePref;

    [SerializeField]
    GameObject outcomeMessagePref;

    [SerializeField]
    Transform messageView;

    [SerializeField]
    ChatData chatData;

    [SerializeField]
    ScrollRect scrollRect;

    const string BASE_MASSEGE = "<b>{0}</b>:{1}";

    int count = 0;

    private void Awake()
    { chatData.AddListener(AddMassages); }

    private void OnDestroy()
    { chatData.RemoveListener(AddMassages); }

    public void AddMassages(ChatMessage[] chatMessages)
    {
        foreach(Transform child in messageView)
        { Destroy(child.gameObject); }

        for(int i=0; i<chatMessages.Length; i++)
        { AddMassege(chatMessages[i]); }

        if (count < chatMessages.Length)
        { StartCoroutine(AlignChat()); }

        count = chatMessages.Length;
    }

    public IEnumerator AlignChat()
    {
        yield return true;
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void AddMassege(ChatMessage chatMessage)
    {
        GameObject messageObject;

        if (GameManager.Instance.model.user.userID.Equals(chatMessage.userID))
        { messageObject = Instantiate(outcomeMessagePref, messageView); }
        else
        { messageObject = Instantiate(incomeMessagePref, messageView); }
        
        PreferTextSize preferTextSize = messageObject.GetComponent<PreferTextSize>();
        string messageText = string.Format(BASE_MASSEGE, chatMessage.author, chatMessage.text);
        preferTextSize.SetText(messageText);
    }
}