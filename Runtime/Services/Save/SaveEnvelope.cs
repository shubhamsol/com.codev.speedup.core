using System;
using System.Collections.Generic;

namespace Speedup.Services.Save
{
    [Serializable]
    public class SaveEnvelope
    {
        public int envelopeVersion;
        public string slotId;
        public string profileId;
        public long savedAtUnixMs;
        public List<SaveStreamSnapshot> streams = new List<SaveStreamSnapshot>();
    }

    [Serializable]
    public class SaveStreamSnapshot
    {
        public string key;
        public int schemaVersion;
        public long updatedAtUnixMs;
        public string payload;
    }
}
