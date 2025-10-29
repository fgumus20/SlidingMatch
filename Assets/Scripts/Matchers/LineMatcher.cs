using System.Collections.Generic;
using UnityEngine;

public class LineMatcher : MatcherBase, IMatcher
{
    public LineMatcher(
        GridObject[,] grid, int gridWidth, int gridHeight,
        float[] widthPositions, float[] heightPositions,
        System.Func<ObjectType> randomColor,
        System.Func<ObjectType, GameObject> cubePoolGetter,
        System.Func<Vector2Int> gapPosGetter,
        System.Action onFallStart,
        System.Action onFallDone)
        : base(grid, gridWidth, gridHeight, widthPositions, heightPositions,
               randomColor, cubePoolGetter, gapPosGetter, onFallStart, onFallDone)
    { }

    public override bool ResolveOnce()
    {
        if (!GameManager.isGameActive)
            return false;
        var toClear = FindLineMatches3Plus();
        if (toClear.Count == 0) return false;

        ClearCells(toClear);
        CollapseColumns();
        RefillExceptGap();
        return true;
    }

    private HashSet<Vector2Int> FindLineMatches3Plus()
    {
        var res = new HashSet<Vector2Int>();
        // yatay
        for (int y = 0; y < H; y++)
        {
            int x = 0;
            while (x < W)
            {
                var a = grid[x, y] as Cube;
                if (a == null) { x++; continue; }
                var color = a.GetType1();
                if ((int)color > 3) { x++; continue; }

                int run = 1, k = x + 1;
                while (k < W && grid[k, y] is Cube b && b.GetType1() == color) { run++; k++; }
                if (run >= 3) for (int m = 0; m < run; m++) res.Add(new Vector2Int(x + m, y));
                x = k;
            }
        }
        // dikey
        for (int x = 0; x < W; x++)
        {
            int y = 0;
            while (y < H)
            {
                var a = grid[x, y] as Cube;
                if (a == null) { y++; continue; }
                var color = a.GetType1();
                if ((int)color > 3) { y++; continue; }

                int run = 1, k = y + 1;
                while (k < H && grid[x, k] is Cube b && b.GetType1() == color) { run++; k++; }
                if (run >= 3) for (int m = 0; m < run; m++) res.Add(new Vector2Int(x, y + m));
                y = k;
            }
        }
        return res;
    }
}
