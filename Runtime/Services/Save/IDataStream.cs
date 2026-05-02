using System;

namespace Speedup.Services.Save
{
    public interface IDataStream
    {
        string Key { get; }
        int SchemaVersion { get; }
        bool IsDirty { get; }
        long UpdatedAtUnixMs { get; }

        event Action OnRegister;
        event Action OnDirty;
        event Action OnBeforeSave;
        event Action OnAfterLoad;

        void NotifyRegistered();
        void MarkDirty();
        void ClearDirty();
        SaveStreamSnapshot CreateSnapshot(ISaveSerializer serializer);
        bool TryApplySnapshot(SaveStreamSnapshot snapshot, ISaveSerializer serializer);
    }

    public interface IDataStream<T> : IDataStream
    {
        T Value { get; }
        void SetValue(T value, bool markDirty = true);
    }
}
