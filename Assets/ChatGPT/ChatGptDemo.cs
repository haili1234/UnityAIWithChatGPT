using System;
using OpenAi;
using UnityEngine;
using UnityEngine.UI;


public class ChatGptDemo : MonoBehaviour
{
    public Chat chat;
    public Text text;

    private void Start()
    {
        SingleAskStream("写一首诗吧");
    }

    /// <summary>
    ///  聊天
    /// </summary>
    /// <param name="message"></param>
    private async void Chat(string message)
    {
        chat.messages.Add(new Message
        {
            role = "user",
            content = message
        });
        await ChatGpt.Chat(chat);
        Debug.Log(chat.Now);
    }

    /// <summary>
    /// 单次问答
    /// </summary>
    /// <param name="message"></param>
    private async void SingleAsk(string message)
    {
        var result = await ChatGpt.SingleAsk(message);
        Debug.Log(result);
    }

    /// <summary>
    ///  流式聊天
    /// </summary>
    /// <param name="message"></param>
    private async void ChatStream(string message)
    {
        chat.messages.Add(new Message
        {
            role = "user",
            content = message
        });
        await ChatGpt.ChatStream(chat, Debug.Log);
        Debug.Log($"结束了--------{chat.Now}");
    }

    async void SingleAskStream(string message)
    {
        var result = await ChatGpt.SingleAskStream(message, Debug.Log);
        if (text != null)
        {
            text.text = result;
        }
        Debug.Log($"结束了--------{result}");
    }
}