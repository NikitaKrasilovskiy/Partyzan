using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;
using TMPro;

public class ChatData : MonoBehaviour
{
    public delegate void ChatDataListener(ChatMessage[] chatMessages);
    [SerializeField]
    PreloaderAmination preloader;

    [SerializeField]
    TMP_InputField messageText;

    Model model;

    List<ChatDataListener> updateListeners = new List<ChatDataListener>();

    bool updateChat = false;

    public void AddListener(ChatDataListener chatDataListener)
    { updateListeners.Add(chatDataListener); }

    public void RemoveListener(ChatDataListener chatDataListener)
    { updateListeners.Remove(chatDataListener); }

    

    private void OnEnable()
    {
        StartUpdateChat();
        model = GameManager.Instance.model;
    }

    private void OnDisable()
    { StopAllCoroutines(); }

    public void StopUpdateChat()
    { updateChat = false; }

    public void StartUpdateChat()
    {
        updateChat = true;
        StopAllCoroutines();
        StartCoroutine(Updater());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            updateChat = false;
    }

    public void SendChatMessage(string message)
    { StartCoroutine(Send(message)); }

    IEnumerator Send(string s)
    {
        yield return GameManager.Instance.model.sendMessage(0, s);
        yield return UpdateChat();
    }

    IEnumerator Updater()
    {
        
        while (updateChat)
        {
            yield return UpdateChat();
            yield return new WaitForSeconds(GameManager.Instance.model.rules.commonParams.chatUpdateTime);
        }
    }

    IEnumerator UpdateChat()
    {
        Model model = GameManager.Instance.model;
        yield return model.getChat(0);
        if(model.chat!=null)
        foreach (ChatDataListener chatDatalistener in updateListeners)
        { chatDatalistener.Invoke(model.chat.messages); }
    }

    public void sendMessage()
    {
        string s = messageText.text;
        if (s == "") return;

        if (s.Length > model.rules.commonParams.maxMessageLength) s.Remove(model.rules.commonParams.maxMessageLength);
        StartCoroutine(SendMessageProcess(s));
    }

    IEnumerator SendMessageProcess(string s)
    {
        preloader.Activate();
        yield return model.sendMessage(0, s);

        messageText.text = "";
        preloader.Deactivate();
    }
}