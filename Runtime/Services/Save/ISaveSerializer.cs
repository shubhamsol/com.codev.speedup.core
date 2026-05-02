namespace Speedup.Services.Save
{
    public interface ISaveSerializer
    {
        string SerializeEnvelope(SaveEnvelope envelope);
        bool TryDeserializeEnvelope(string payload, out SaveEnvelope envelope);
        string SerializeStreamPayload<T>(T payload);
        bool TryDeserializeStreamPayload<T>(string payload, out T value);
    }
}
