using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenAi
{
    public class ChatGpt
    {
        public const string ApiKey = "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";
       
        static async Task GetAwaiter(AsyncOperation asyncOperation)
        {
            var task = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => { task.SetResult(true); };
            await task.Task;
        }

        static async Task<string> GetResponse(string body, List<(string, string)> headers = null)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");

            headers ??= new List<(string, string)>
            {
                ("Authorization", $"Bearer {ApiKey}"),
                ("Content-Type", "application/json")
            };
            for (int i = 0; i < headers.Count; i++)
            {
                webRequest.SetRequestHeader(headers[i].Item1, headers[i].Item2);
            }

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            await GetAwaiter(webRequest.SendWebRequest());

            string result = "";
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    if (url.EndsWith("/completions"))
                    {
                        Debug.LogError(webRequest.downloadHandler.text);
                        if (webRequest.error == "HTTP/1.1 429 Too Many Requests")
                        {
                            Debug.Log("请求太快,请求失败,正在重试...");
                            await Task.Delay(2000);
                            result = await GetResponse(body, headers);
                        }
                    }

                    break;
                case UnityWebRequest.Result.Success:
                    result = webRequest.downloadHandler.text;
                    break;
            }

            webRequest.Dispose();
            return result;
        }

        static string OperateMessage(string str)
        {
            string[] split ={"data:"};
            var list = str.Split(split,StringSplitOptions.None);
            string result = "";
            foreach (var s in list)
            {
                try
                {
                    if (s.Contains("[DONE]")) continue;
                    var line = JsonUtility.FromJson<ChatCompletionChunk>(s);
                    if (line == null) continue;
                    result += line.choices.Count > 0 ? line.choices[0].delta.content : "";
                    
                }
                catch (Exception e)
                {
                   // Debug.LogError(e);
                }
            }

            return result;
        }

        /// <summary>
        ///  这个是Stream时返回的数据
        /// </summary>
        [Serializable]
        public class ChatCompletionChunk
        {
            public string id;
            public string @object;
            public long created;
            public string model;
            public List<Choice> choices;

            [Serializable]
            public class Choice
            {
                public Delta delta;
                public int index;
                public object finish_reason;
            }

            [Serializable]
            public class Delta
            {
                public string content;
            }
        }

        public static async Task<string> ChatStream(Chat chat, Action<string> onResultChange,
            int messageDelay = 100)
        {
            if (chat == null)
                chat = new Chat();
            chat.stream = true;
            string body = JsonUtility.ToJson(chat);
            var url = "https://api.openai.com/v1/chat/completions";
            var headers = new List<(string name, string header)>
            {
                ("Authorization", $"Bearer {ApiKey}"),
                ("Content-Type", "application/json")
            };
            UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

            for (int i = 0; i < headers.Count; i++)
            {
                webRequest.SetRequestHeader(headers[i].name, headers[i].header);
            }

            byte[] bodyToSend = new UTF8Encoding().GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            var asyncOption = webRequest.SendWebRequest();

            int safeCount = 1000;
            string result = "";
            while (!asyncOption.isDone)
            {
                await Task.Delay(messageDelay);
                result = OperateMessage(webRequest.downloadHandler.text);
                result = RemoveStartSpace(result);
                onResultChange?.Invoke(result);
                if (safeCount-- < 0)
                {
                    Debug.LogError("请求超时");
                    break;
                }
            }

            webRequest.Dispose();
            chat.AddAssistantMessage(result);
            return result;
        }

        public static async Task<string> SingleAskStream(string line, Action<string> onResultChange,
            string initSetting = "",
            int messageDelay = 100)
        {
            Chat chat = new Chat();
            if (initSetting != "")
                chat.AddSystemMessage(initSetting);

            chat.AddUserMessage(line);

            return await ChatStream(chat, onResultChange, messageDelay);
        }

        public static async Task<string> SingleAsk(string line, string initSetting = "")
        {
            var chat = new Chat();
            chat.stream = false;
            if (initSetting != "")
                chat.AddSystemMessage(initSetting);

            chat.AddUserMessage(line);

            var body = JsonUtility.ToJson(chat);
            var response = JsonUtility.FromJson<ChatCompletion>(await GetResponse(body));
            var result = response.choices.Count > 0 ? response.choices[0].message.content : "";
            result = RemoveStartSpace(result);
            return result;
        }

        public static async Task<ChatCompletion> Chat(Chat chat)
        {
            chat.stream = false;
            var body = JsonUtility.ToJson(chat);
            var response = JsonUtility.FromJson<ChatCompletion>(await GetResponse(body));
            var result = response.choices.Count > 0 ? response.choices[0].message.content : "";
            result = RemoveStartSpace(result);
            response.choices[0].message.content = result;
            chat.messages.Add(response.choices[0].message);
            return response;
        }

        //移除字符串开头的空格和换行
        public static string RemoveStartSpace(string str)
        {
            int i = 0;
            for (; i < str.Length; i++)
            {
                if (str[i] != ' ' && str[i] != '\n')
                    break;
            }

            return str.Substring(i);
        }
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [Serializable]
    public class Choice
    {
        public Message message;
        public string finish_reason;
        public int index;
    }

    [Serializable]
    public class ChatCompletion
    {
        public string id;
        public string @object;
        public int created;
        public string model;
        public Usage usage;
        public List<Choice> choices = new List<Choice>();
        public string Now => choices.Count > 0 ? choices[1].message.content : "null";
    }


    [Serializable]
    public class Chat
    {
        public Chat()
        {
            messages = new List<Message>();
            model = "gpt-3.5-turbo";
        }

        public string model;
        public List<Message> messages;
        public string Now => messages.Count > 0 ? messages[1].content : "null";
        public bool stream = false;

        public int max_tokens = 1024;

        public void AddSystemMessage(string message)
        {
            messages.Add(new Message
            {
                role = "system",
                content = message
            });
        }

        public void AddUserMessage(string message)
        {
            messages.Add(new Message
            {
                role = "user",
                content = message
            });
        }


        public void AddAssistantMessage(string message)
        {
            messages.Add(new Message
            {
                role = "assistant",
                content = message
            });
        }
    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}