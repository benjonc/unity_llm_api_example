using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// OPENAI Whisper API�ӿ�
/// </summary>
public static class WhisperAPI
{
    /// <summary>
    /// ����ת����API
    /// </summary>
    /// <param name="audioData"> �������� </param>
    /// <param name="url"> ���ӵ�ַ </param>
    /// <param name="apiKey"> key </param>
    /// <param name="callback"> �ص� </param>
    public static void GetAudioText(byte[] audioData, string url, string apiKey, Action<string> callback)
    {
        apiKey = string.IsNullOrEmpty(apiKey) ? APIConst.OpenAIKey : apiKey;

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("model", "whisper-1"));
        formData.Add(new MultipartFormFileSection("file", audioData, "audio.mp3", "application/octet-stream"));

        UniTask.Void(async () =>
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.SetRequestHeader("Accept", "*/*");
                www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                var boundary = UnityWebRequest.GenerateBoundary();
                var formSections = UnityWebRequest.SerializeFormSections(formData, boundary);
                var contentType = $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}";
                www.uploadHandler = new UploadHandlerRaw(formSections) { contentType = contentType };
                www.downloadHandler = new DownloadHandlerBuffer();

                Debug.Log("Start Send Whisper!");

                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    return;
                }

                var text = www.downloadHandler.text;
                callback?.Invoke(text);
            }
        });
    }
}
