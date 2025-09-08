using System.Collections.Generic;

public class Level
{
    public int LevelNumber { get; set; }
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
    public int MoveCount { get; set; }
    public List<string> Grid { get; set; }

    public Level(int levelNumber, int gridWidth, int gridHeight, int moveCount, List<string> grid)
    {
        LevelNumber = levelNumber;
        GridWidth = gridWidth;
        GridHeight = gridHeight;
        MoveCount = moveCount;
        Grid = grid;
    }
}
