using System;
using UnityEngine;

namespace Speedup.Services.Save
{
    public class JsonSaveSerializer : ISaveSerializer
    {
        public string SerializeEnvelope(SaveEnvelope envelope)
        {
            return JsonUtility.ToJson(envelope);
        }

        public bool TryDeserializeEnvelope(string payload, out SaveEnvelope envelope)
        {
            envelope = null;
            if (string.IsNullOrWhiteSpace(payload))
            {
                return false;
            }

            try
            {
                envelope = JsonUtility.FromJson<SaveEnvelope>(payload);
                return envelope != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string SerializeStreamPayload<T>(T payload)
        {
            return JsonUtility.ToJson(payload);
        }

        public bool TryDeserializeStreamPayload<T>(string payload, out T value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(payload))
            {
                return false;
            }

            try
            {
                value = JsonUtility.FromJson<T>(payload);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
