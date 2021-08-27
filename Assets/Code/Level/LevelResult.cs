namespace Code.Level
{
    public class LevelResult
    {
        public bool Success { get; }
        public bool NoDamage { get; }
        public LevelRecordingData LevelRecordingData { get; }

        public LevelResult(bool success, bool noDamage, LevelRecordingData levelRecordingData)
        {
            Success = success;
            NoDamage = noDamage;
            LevelRecordingData = levelRecordingData;
        }
    }
}