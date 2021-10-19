namespace Code.Level
{
    public class LevelLayoutContext
    {
        /// <summary>
        /// Level index for referencing in code
        /// </summary>
        public int LevelIndex { get; set; } = 0;
        
        /// <summary>
        /// Level number for showing to user
        /// </summary>
        public int LevelNumber => LevelIndex + 1;
        
        public bool IsFirstLevel => LevelIndex == 0;
        public bool IsTutorial => LevelIndex < 0;
    }
}