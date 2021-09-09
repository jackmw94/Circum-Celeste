namespace Code.Level
{
    public class LevelResult
    {
        public bool WasReplay { get; }
        public bool Success { get; }
        public bool NoDamage { get; }
        public LevelRecordingData LevelRecordingData { get; }

        public LevelResult(bool success, bool noDamage, LevelRecordingData levelRecordingData, bool wasReplay = false)
        {
            Success = success;
            NoDamage = noDamage;
            LevelRecordingData = levelRecordingData;
            WasReplay = wasReplay;
        }
    }
}