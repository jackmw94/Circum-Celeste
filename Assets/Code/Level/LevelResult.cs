namespace Code.Level
{
    public class LevelResult
    {
        public bool WasReplay { get; }
        public bool Success { get; }
        public LevelRecordingData LevelRecordingData { get; }

        public LevelResult(bool success, LevelRecordingData levelRecordingData, bool wasReplay = false)
        {
            Success = success;
            LevelRecordingData = levelRecordingData;
            WasReplay = wasReplay;
        }
    }
}