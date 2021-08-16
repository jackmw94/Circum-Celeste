namespace Code.Level
{
    public enum CellType
    {
        None,
        Wall,
        Pickup,
        Enemy,
        Escape,
        PlayerStart,
    }

    public enum EscapeCriteria
    {
        Timed,
        DestroyedAll,
        PickedUpAll
    }
}