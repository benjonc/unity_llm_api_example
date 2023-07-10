using UnityEngine;

public class TestChatGPT : MonoBehaviour
{
    public bool IsSend;
    public string UserInputContent;
    public string SystemPrompt;
    public string OpenAI_APIKEY;


    void Update()
    {
        if (IsSend)
        {
            IsSend = false;

            if (string.IsNullOrEmpty(UserInputContent)) return;

            ChatGPTAPI.SendGPTHistory(APIConst.OpenAIChatGPTUrl, OpenAI_APIKEY, UserInputContent, SystemPrompt, (chatMsg) => 
            {
                Debug.Log(chatMsg);
            });
        }        
    }
}
