using System.Collections.Generic;
using UnityEngine;

public class LineMatcher
{
    private GridObject[,] grid;
    private readonly int W, H;
    private readonly float[] widthPos;
    private readonly float[] heightPos;

    private readonly System.Func<ObjectType> rngColor;
    private readonly System.Func<ObjectType, GameObject> getCubeFromPool;
    private readonly System.Func<Vector2Int> getGapPos;


    public LineMatcher(
        GridObject[,] grid,
        int gridWidth, int gridHeight,
        float[] widthPositions, float[] heightPositions,
        System.Func<ObjectType> randomColor,
        System.Func<ObjectType, GameObject> cubePoolGetter,
        System.Func<Vector2Int> gapPosGetter)
    {
        this.grid = grid;
        this.W = gridWidth;
        this.H = gridHeight;
        this.widthPos = widthPositions;
        this.heightPos = heightPositions;
        this.rngColor = randomColor;
        this.getCubeFromPool = cubePoolGetter;
        this.getGapPos = gapPosGetter;
    }

    public void ResolveCascade(int maxLoops = 20)
    {
        int guard = 0;
        while (guard++ < maxLoops)
        {
            if (!ResolveOnce()) break;
        }
    }

    // --- tek adýmlýk çözüm: bul -> temizle -> düþür -> doldur ---
    public bool ResolveOnce()
    {
        var toClear = FindLineMatches3Plus();
        if (toClear.Count == 0) return false;

        ClearCells(toClear);
        CollapseColumns();
        RefillExceptGap();
        return true;
    }

    // --- sadece yatay/dikey 3+ çizgileri bul ---
    private HashSet<Vector2Int> FindLineMatches3Plus()
    {
        var res = new HashSet<Vector2Int>();

        // YATAY
        for (int y = 0; y < H; y++)
        {
            int x = 0;
            while (x < W)
            {
                var a = grid[x, y] as Cube;
                if (a == null) { x++; continue; }
                var color = a.GetType1();
                if ((int)color > 3) { x++; continue; } // engel vs deðil

                int run = 1, k = x + 1;
                while (k < W && grid[k, y] is Cube b && b.GetType1() == color) { run++; k++; }
                if (run >= 3)
                    for (int m = 0; m < run; m++) res.Add(new Vector2Int(x + m, y));
                x = k;
            }
        }

        // DÝKEY
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
                if (run >= 3)
                    for (int m = 0; m < run; m++) res.Add(new Vector2Int(x, y + m));
                y = k;
            }
        }

        return res;
    }

    private void ClearCells(HashSet<Vector2Int> cells)
    {
        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                c.SetFalse();          // efekt + disable (mevcut fonksiyonunuz)
                grid[p.x, p.y] = null;
                ApplyDamageToNeighbours(p.x, p.y);
            }
        }
    }

    private void CollapseColumns()
    {
        var gap = getGapPos(); // GridManager.gapPos

        for (int x = 0; x < W; x++)
        {
            int writeY = 0;
            for (int y = 0; y < H; y++)
            {
                if (grid[x, y] is Cube c)
                {
                    // Yazma hedefi gap'e denk gelirse bir hücre aþaðý kaydýr
                    int targetY = writeY;
                    if (x == gap.x && targetY == gap.y)
                        targetY++;

                    if (targetY != y)
                    {
                        grid[x, targetY] = c;
                        grid[x, y] = null;
                        c.SetXandY(x, targetY);
                        c.GetFall().StartFallingToHeight(heightPos[targetY]);
                    }
                    writeY = targetY + 1;
                }
                else if (grid[x, y] != null)
                {
                    // Engel gördüysen yazma kafasýný onun ALTINA al
                    writeY = y + 1;
                }
            }

            // Güvence: o kolonda gap hücresini boþ býrak
            if (x == gap.x)
                grid[gap.x, gap.y] = null;
        }
    }

    private void RefillExceptGap()
    {
        var gap = getGapPos(); // GridManager.gapPos
        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                if (x == gap.x && y == gap.y) continue; // gap boþ kalsýn
                if (grid[x, y] == null)
                {
                    var color = rngColor(); // 0..3
                    var go = getCubeFromPool(color);
                    go.SetActive(true);
                    var cube = go.GetComponent<Cube>();
                    var start = new Vector3(widthPos[x], 700f, -H - 2); // CreateCubes mantýðýna paralel
                    cube.SetProperties(start, color, x, y);
                    grid[x, y] = cube;
                    cube.GetFall().StartFallingToHeight(heightPos[y]);
                }
            }
        }
    }

    private void ApplyDamageToNeighbours(int x, int y)
    {
        int[,] dirs = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];
            if (nx < 0 || nx >= W || ny < 0 || ny >= H) continue;

            if (grid[nx, ny] is ObstacleController obs)
            {
                obs.TakeDamage();
                if (obs.GetHealth() <= 0)
                {
                    grid[nx, ny] = null;
                    obs.gameObject.SetActive(false);
                }
            }
        }
    }

}
