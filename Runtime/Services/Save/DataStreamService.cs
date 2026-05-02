using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Save
{
    public class DataStreamService : IDataStreamService
    {
        private readonly Dictionary<string, IDataStream> _streamsByKey = new Dictionary<string, IDataStream>(StringComparer.Ordinal);
        private readonly List<IDataStream> _allStreams = new List<IDataStream>();
        private readonly List<IDataStream> _dirtyStreamsBuffer = new List<IDataStream>();

        public void Initialize()
        {
        }

        public void RegisterStream(IDataStream stream)
        {
            if (stream == null)
            {
                Debug.LogWarning("[DataStreamService] Tried to register a null stream.");
                return;
            }

            if (string.IsNullOrWhiteSpace(stream.Key))
            {
                Debug.LogWarning("[DataStreamService] Stream key is null or empty.");
                return;
            }

            if (_streamsByKey.ContainsKey(stream.Key))
            {
                Debug.LogWarning($"[DataStreamService] Stream key '{stream.Key}' already exists. Registration skipped.");
                return;
            }

            _streamsByKey.Add(stream.Key, stream);
            _allStreams.Add(stream);
            stream.NotifyRegistered();
        }

        public bool TryGetStream(string key, out IDataStream stream)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                stream = null;
                return false;
            }

            return _streamsByKey.TryGetValue(key, out stream);
        }

        public bool TryGetStream<T>(string key, out IDataStream<T> stream)
        {
            stream = null;
            if (!TryGetStream(key, out IDataStream rawStream))
            {
                return false;
            }

            stream = rawStream as IDataStream<T>;
            return stream != null;
        }

        public IReadOnlyCollection<IDataStream> GetAllStreams()
        {
            return _allStreams;
        }

        public IReadOnlyCollection<IDataStream> GetDirtyStreams()
        {
            _dirtyStreamsBuffer.Clear();
            for (int i = 0; i < _allStreams.Count; i++)
            {
                if (_allStreams[i].IsDirty)
                {
                    _dirtyStreamsBuffer.Add(_allStreams[i]);
                }
            }

            return _dirtyStreamsBuffer;
        }
    }
}
