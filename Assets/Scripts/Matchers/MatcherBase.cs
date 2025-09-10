using System.Collections.Generic;
using UnityEngine;

public abstract class MatcherBase : IMatcher
{
    protected GridObject[,] grid;
    protected readonly int W, H;
    protected readonly float[] widthPos;
    protected readonly float[] heightPos;

    protected readonly System.Func<ObjectType> rngColor;
    protected readonly System.Func<ObjectType, GameObject> getCubeFromPool;
    protected readonly System.Func<Vector2Int> getGapPos;

    protected MatcherBase(
        GridObject[,] grid,
        int gridWidth, int gridHeight,
        float[] widthPositions, float[] heightPositions,
        System.Func<ObjectType> randomColor,
        System.Func<ObjectType, GameObject> cubePoolGetter,
        System.Func<Vector2Int> gapPosGetter)
    {
        this.grid = grid;
        W = gridWidth;
        H = gridHeight;
        widthPos = widthPositions;
        heightPos = heightPositions;
        rngColor = randomColor;
        getCubeFromPool = cubePoolGetter;
        getGapPos = gapPosGetter;
    }

    public void ResolveCascade(int maxLoops = 20)
    {
        int guard = 0;
        while (guard++ < maxLoops)
        {
            if (!ResolveOnce()) break;
        }
    }

    // Her alt sýnýf kendi eþleþmesini bulup temizlemeyi çaðýracak
    protected abstract bool ResolveOnce();

    // --- Ortak yardýmcýlar ---
    protected void ClearCells(HashSet<Vector2Int> cells)
    {
        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                c.SetFalse();
                grid[p.x, p.y] = null;
                ApplyDamageToNeighbours(p.x, p.y);
            }
        }
    }

    protected void CollapseColumns()
    {
        var gap = getGapPos();
        for (int x = 0; x < W; x++)
        {
            int writeY = 0;
            for (int y = 0; y < H; y++)
            {
                if (grid[x, y] is Cube c)
                {
                    int targetY = writeY;
                    if (x == gap.x && targetY == gap.y) targetY++;

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
                    // Engel gördüysen yazmayý onun ALTINA al
                    writeY = y + 1;
                }
            }
            if (x == gap.x) grid[gap.x, gap.y] = null; // güvence
        }
    }

    protected void RefillExceptGap()
    {
        var gap = getGapPos();
        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                if (x == gap.x && y == gap.y) continue;
                if (grid[x, y] == null)
                {
                    var color = rngColor();
                    var go = getCubeFromPool(color);
                    go.SetActive(true);
                    var cube = go.GetComponent<Cube>();
                    var start = new Vector3(widthPos[x], 700f, -H - 2);
                    cube.SetProperties(start, color, x, y);
                    grid[x, y] = cube;
                    cube.GetFall().StartFallingToHeight(heightPos[y]);
                }
            }
        }
    }

    protected void ApplyDamageToNeighbours(int x, int y)
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
                    GameManager.instance.DecreaseObstacleCount(obs.GetType1());
                    grid[nx, ny] = null;
                    obs.gameObject.SetActive(false);
                }
            }
        }
    }
}
