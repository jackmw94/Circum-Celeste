namespace Code.Level
{
    public class RunTracker
    {
        public bool HasSkipped { get; set; } = false;
        public bool IsPerfect { get; set; } = true;
        public int Deaths { get; set; } = 0;

        public void ResetRun()
        {
            HasSkipped = false;
            IsPerfect = true;
            Deaths = 0;
        }

        public override string ToString()
        {
            return $"HasSkipped={HasSkipped}, IsPerfect={IsPerfect}, Deaths={Deaths}";
        }
    }
}