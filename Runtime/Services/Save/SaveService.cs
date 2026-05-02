using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Save
{
    public class SaveService : ISaveService
    {
        private const string DefaultSlotId = "default";

        private readonly IDataStreamService _dataStreamService;
        private readonly ISaveSerializer _serializer;
        private readonly List<ISaveProvider> _providers;
        private readonly string _profileId;

        public SaveService(
            IDataStreamService dataStreamService,
            ISaveSerializer serializer,
            List<ISaveProvider> providers,
            string profileId = "default")
        {
            _dataStreamService = dataStreamService;
            _serializer = serializer;
            _providers = providers ?? new List<ISaveProvider>();
            _profileId = string.IsNullOrWhiteSpace(profileId) ? "default" : profileId;
        }

        public void Initialize()
        {
        }

        public void Save(Action<bool> onComplete = null, bool forceFullSnapshot = false)
        {
            Save(DefaultSlotId, onComplete, forceFullSnapshot);
        }

        public void Save(string slotId, Action<bool> onComplete = null, bool forceFullSnapshot = false)
        {
            var context = new SaveContext(slotId, _profileId);

            IReadOnlyCollection<IDataStream> sourceStreams = forceFullSnapshot
                ? _dataStreamService.GetAllStreams()
                : _dataStreamService.GetDirtyStreams();
            var streams = new List<IDataStream>(sourceStreams);

            var envelope = new SaveEnvelope
            {
                envelopeVersion = context.EnvelopeVersion,
                profileId = context.ProfileId,
                slotId = context.SlotId,
                savedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            foreach (IDataStream stream in streams)
            {
                envelope.streams.Add(stream.CreateSnapshot(_serializer));
            }

            if (envelope.streams.Count == 0 && !forceFullSnapshot)
            {
                onComplete?.Invoke(true);
                return;
            }

            string payload = _serializer.SerializeEnvelope(envelope);
            SaveWithProviderFallback(0, context, payload, success =>
            {
                if (success)
                {
                    foreach (IDataStream stream in streams)
                    {
                        stream.ClearDirty();
                    }
                }

                onComplete?.Invoke(success);
            });
        }

        public void Load(Action<bool> onComplete = null)
        {
            Load(DefaultSlotId, onComplete);
        }

        public void Load(string slotId, Action<bool> onComplete = null)
        {
            var context = new SaveContext(slotId, _profileId);

            LoadWithProviderFallback(0, context, (success, payload) =>
            {
                if (!success || string.IsNullOrWhiteSpace(payload))
                {
                    onComplete?.Invoke(false);
                    return;
                }

                if (!_serializer.TryDeserializeEnvelope(payload, out SaveEnvelope envelope) || envelope.streams == null)
                {
                    Debug.LogWarning("[SaveService] Failed to deserialize save envelope.");
                    onComplete?.Invoke(false);
                    return;
                }

                for (int i = 0; i < envelope.streams.Count; i++)
                {
                    SaveStreamSnapshot snapshot = envelope.streams[i];
                    if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.key))
                    {
                        continue;
                    }

                    if (!_dataStreamService.TryGetStream(snapshot.key, out IDataStream stream))
                    {
                        continue;
                    }

                    if (stream.TryApplySnapshot(snapshot, _serializer))
                    {
                        stream.ClearDirty();
                    }
                }

                onComplete?.Invoke(true);
            });
        }

        private void SaveWithProviderFallback(int index, SaveContext context, string payload, Action<bool> onComplete)
        {
            if (index >= _providers.Count)
            {
                onComplete?.Invoke(false);
                return;
            }

            ISaveProvider provider = _providers[index];
            if (provider == null || !provider.IsAvailable)
            {
                SaveWithProviderFallback(index + 1, context, payload, onComplete);
                return;
            }

            provider.Save(context, payload, success =>
            {
                if (success)
                {
                    onComplete?.Invoke(true);
                    return;
                }

                SaveWithProviderFallback(index + 1, context, payload, onComplete);
            });
        }

        private void LoadWithProviderFallback(int index, SaveContext context, Action<bool, string> onComplete)
        {
            if (index >= _providers.Count)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            ISaveProvider provider = _providers[index];
            if (provider == null || !provider.IsAvailable)
            {
                LoadWithProviderFallback(index + 1, context, onComplete);
                return;
            }

            provider.Load(context, (success, payload) =>
            {
                if (success && !string.IsNullOrWhiteSpace(payload))
                {
                    onComplete?.Invoke(true, payload);
                    return;
                }

                LoadWithProviderFallback(index + 1, context, onComplete);
            });
        }
    }
}
