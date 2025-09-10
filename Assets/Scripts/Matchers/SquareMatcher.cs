using System.Collections.Generic;
using UnityEngine;

public class SquareMatcher : MatcherBase, IMatcher
{
    public SquareMatcher(
        GridObject[,] grid,
        int gridWidth, int gridHeight,
        float[] widthPositions, float[] heightPositions,
        System.Func<ObjectType> randomColor,
        System.Func<ObjectType, GameObject> cubePoolGetter,
        System.Func<Vector2Int> gapPosGetter)
        : base(grid, gridWidth, gridHeight, widthPositions, heightPositions, randomColor, cubePoolGetter, gapPosGetter)
    { }

    protected override bool ResolveOnce()
    {
        var toClear = FindSquares2x2();
        if (toClear.Count == 0) return false;

        ClearCells(toClear);
        CollapseColumns();
        RefillExceptGap();
        return true;
    }

    private HashSet<Vector2Int> FindSquares2x2()
    {
        var res = new HashSet<Vector2Int>();
        for (int x = 0; x < W - 1; x++)
        {
            for (int y = 0; y < H - 1; y++)
            {
                var a = grid[x, y] as Cube;
                var b = grid[x + 1, y] as Cube;
                var c = grid[x, y + 1] as Cube;
                var d = grid[x + 1, y + 1] as Cube;
                if (a == null || b == null || c == null || d == null) continue;

                var color = a.GetType1();
                if ((int)color > 3) continue; // engel deðil

                if (b.GetType1() == color && c.GetType1() == color && d.GetType1() == color)
                {
                    res.Add(new Vector2Int(x, y));
                    res.Add(new Vector2Int(x + 1, y));
                    res.Add(new Vector2Int(x, y + 1));
                    res.Add(new Vector2Int(x + 1, y + 1));
                }
            }
        }
        return res;
    }
}
