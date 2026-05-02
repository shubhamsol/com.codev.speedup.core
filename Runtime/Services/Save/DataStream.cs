using System;

namespace Speedup.Services.Save
{
    public sealed class DataStream<T> : IDataStream<T>
    {
        public string Key { get; }
        public int SchemaVersion { get; }
        public bool IsDirty { get; private set; }
        public long UpdatedAtUnixMs { get; private set; }
        public T Value => _value;

        public event Action OnRegister;
        public event Action OnDirty;
        public event Action OnBeforeSave;
        public event Action OnAfterLoad;

        private T _value;

        public DataStream(string key, T initialValue, int schemaVersion = 1)
        {
            Key = key;
            SchemaVersion = schemaVersion < 1 ? 1 : schemaVersion;
            _value = initialValue;
            UpdatedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            IsDirty = false;
        }

        public void SetValue(T value, bool markDirty = true)
        {
            _value = value;
            UpdatedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (markDirty)
            {
                MarkDirty();
            }
        }

        public void MarkDirty()
        {
            IsDirty = true;
            UpdatedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            OnDirty?.Invoke();
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }

        public SaveStreamSnapshot CreateSnapshot(ISaveSerializer serializer)
        {
            OnBeforeSave?.Invoke();
            return new SaveStreamSnapshot
            {
                key = Key,
                schemaVersion = SchemaVersion,
                updatedAtUnixMs = UpdatedAtUnixMs,
                payload = serializer.SerializeStreamPayload(_value)
            };
        }

        public bool TryApplySnapshot(SaveStreamSnapshot snapshot, ISaveSerializer serializer)
        {
            if (snapshot == null || !string.Equals(snapshot.key, Key, StringComparison.Ordinal))
            {
                return false;
            }

            if (!serializer.TryDeserializeStreamPayload(snapshot.payload, out T deserialized))
            {
                return false;
            }

            _value = deserialized;
            UpdatedAtUnixMs = snapshot.updatedAtUnixMs > 0
                ? snapshot.updatedAtUnixMs
                : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            IsDirty = false;
            OnAfterLoad?.Invoke();
            return true;
        }

        public void NotifyRegistered()
        {
            OnRegister?.Invoke();
        }
    }
}
