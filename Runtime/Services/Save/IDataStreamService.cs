using System.Collections.Generic;
using Speedup.Core;

namespace Speedup.Services.Save
{
    public interface IDataStreamService : IGameService
    {
        void RegisterStream(IDataStream stream);
        bool TryGetStream(string key, out IDataStream stream);
        bool TryGetStream<T>(string key, out IDataStream<T> stream);
        IReadOnlyCollection<IDataStream> GetAllStreams();
        IReadOnlyCollection<IDataStream> GetDirtyStreams();
    }
}
