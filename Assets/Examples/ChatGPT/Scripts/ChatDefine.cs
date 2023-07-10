using System.Collections.Generic;

/// <summary>
/// ������Ϣ
/// </summary>
[System.Serializable]
public class Message
{
    public string role;
    public string content;
}


/// <summary>
/// ���͵���Ϣ
/// </summary>
[System.Serializable]
public class RequestData
{
    public List<Message> messages = new List<Message>();
    public string model = "gpt-3.5-turbo";
    public float temperature = 0.2f;
}

/// <summary>
/// �ظ�����Ϣ�б�
/// </summary>
[System.Serializable]
public class ResponseData
{
    public List<Choice> choices = new List<Choice>();
}

/// <summary>
/// �ظ��ĵ�����Ϣ
/// </summary>
[System.Serializable]
public class Choice
{
    public Message message;
}


public static class RoleType
{
    public static string User => "user";
    public static string System => "system";
    public static string Assistant => "assistant";
}



