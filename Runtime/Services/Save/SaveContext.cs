namespace Speedup.Services.Save
{
    public class SaveContext
    {
        public const int CurrentEnvelopeVersion = 1;

        public string SlotId { get; }
        public string ProfileId { get; }
        public int EnvelopeVersion { get; }

        public SaveContext(string slotId, string profileId = "default", int envelopeVersion = CurrentEnvelopeVersion)
        {
            SlotId = string.IsNullOrWhiteSpace(slotId) ? "default" : slotId;
            ProfileId = string.IsNullOrWhiteSpace(profileId) ? "default" : profileId;
            EnvelopeVersion = envelopeVersion < 1 ? CurrentEnvelopeVersion : envelopeVersion;
        }
    }
}
