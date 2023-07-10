using UnityEngine;

public class TestWhisper : MonoBehaviour
{
    public string OpenAI_APIKEY;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MicrophoneTool.Ins.StartRecord();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var recordData = MicrophoneTool.Ins.EndRecord();
            WhisperAPI.GetAudioText(recordData, APIConst.OpenAIWhisperUrl, OpenAI_APIKEY, (audioText) =>
            {
                Debug.Log($"You voice text: {audioText}");
            });
        }

    }
}
