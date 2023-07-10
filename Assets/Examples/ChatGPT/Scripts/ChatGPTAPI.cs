using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public static class ChatGPTAPI
{
    /// <summary>
    /// No history api interace
    /// </summary>
    /// <param name="url"></param>
    /// <param name="apiKey"></param>
    /// <param name="prompt"></param>
    /// <param name="system"></param>
    /// <param name="callback"></param>
    public static void SendGPTNoHistory(string url, string apiKey, string prompt, string system, Action<string> callback)
    {
        var usrMsg = new Message()
        {
            role = RoleType.User,
            content = prompt
        };
        var sysMsg = new Message()
        {
            role = RoleType.System,
            content = system
        };

        var sendMsgObj = new RequestData();
        sendMsgObj.messages.Add(sysMsg);
        sendMsgObj.messages.Add(usrMsg);

        var sendMsgJson = JsonUtility.ToJson(sendMsgObj);
        byte[] sendMsgRaw = Encoding.UTF8.GetBytes(sendMsgJson);

        UniTask.Void(async () =>
        {
            using (var webRequest = new UnityWebRequest(url, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(sendMsgRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                var op = webRequest.SendWebRequest();

                while (!op.isDone)
                {
                    await UniTask.Yield();
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"ChatGPT request failed: {webRequest.error}, error = {webRequest.downloadHandler.text}");
                    return;
                }

                var responseData = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                var responseMsg = responseData.choices[0].message.content;
                Debug.Log($"ChatGPT response: {responseMsg}");
                callback?.Invoke(responseMsg);
            }
        });
    }

    /// <summary>
    /// history api interace
    /// </summary>
    /// <param name="url"></param>
    /// <param name="apiKey"></param>
    /// <param name="prompt"></param>
    /// <param name="system"></param>
    /// <param name="callback"></param>
    public static void SendGPTHistory(string url, string apiKey, string prompt, string system, Action<string> callback)
    {
        var usrMsg = new Message()
        {
            role = RoleType.User,
            content = prompt
        };
        var sysMsg = new Message()
        {
            role = RoleType.System,
            content = system
        };

        _historyMsg.Add(usrMsg);
        _historyMsg.Add(sysMsg);

        var sendMsgObj = new RequestData();
        sendMsgObj.messages.AddRange(_historyMsg);

        var sendMsgJson = JsonUtility.ToJson(sendMsgObj);
        byte[] sendMsgRaw = Encoding.UTF8.GetBytes(sendMsgJson);

        UniTask.Void(async () =>
        {
            using (var webRequest = new UnityWebRequest(url, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(sendMsgRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                var op = webRequest.SendWebRequest();

                while (!op.isDone)
                {
                    await UniTask.Yield();
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"ChatGPT request failed: {webRequest.error}, error = {webRequest.downloadHandler.text}");

                    _historyMsg.Remove(usrMsg);
                    _historyMsg.Remove(sysMsg);
                    return;
                }

                var responseData = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                var responseMsg = responseData.choices[0].message.content;

                var astMsg = new Message()
                {
                    role = RoleType.Assistant,
                    content = responseMsg
                };
                _historyMsg.Add(astMsg);

                Debug.Log($"ChatGPT response: {responseMsg}");
                callback?.Invoke(responseMsg);                
            }
        });
    }

    /// <summary>
    /// History Message List
    /// </summary>
    private static List<Message> _historyMsg = new List<Message>();
}
