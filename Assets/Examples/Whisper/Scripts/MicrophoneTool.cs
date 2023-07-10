using System.IO;
using UnityEngine;


/// <summary>
/// Â¼Òô¹¤¾ß
/// </summary>
public class MicrophoneTool : MonoBehaviour
{
    public static MicrophoneTool Ins { get; private set; }


    public int RecordTime = 10;
    public int Frequency = 16000;

    public void StartRecord()
    {
        if (string.IsNullOrEmpty(_microDeviceName)) return;

        _recordClip = Microphone.Start(_microDeviceName, false, RecordTime, Frequency);
    }


    public byte[] EndRecord()
    {
        if (string.IsNullOrEmpty(_microDeviceName)) return null;

        var position = Microphone.GetPosition(_microDeviceName);
        Microphone.End(_microDeviceName);

        var samples = new float[position * _recordClip.channels];
        _recordClip.GetData(samples, 0);

        var audioData = EncodeAsWAV(samples, _recordClip.frequency, _recordClip.channels);

        return audioData;
    }



    private void Start()
    {
        Ins = this;

        var devs = Microphone.devices;
        if (devs == null || devs.Length == 0) return;

        _microDeviceName = devs[0];
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }


    private AudioClip _recordClip;
    private string _microDeviceName;
}