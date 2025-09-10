using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatcherBase : IMatcher
{
    protected GridObject[,] grid;
    protected readonly int W, H;
    protected readonly float[] widthPos;
    protected readonly float[] heightPos;

    protected readonly Func<ObjectType> rngColor;
    protected readonly Func<ObjectType, GameObject> getCubeFromPool;
    protected readonly Func<Vector2Int> getGapPos;

    // 🔹 düşüş sayacı için GridManager’dan gelen kancalar
    protected readonly Action onFallStart;
    protected readonly Action onFallDone;

    protected MatcherBase(
        GridObject[,] grid,
        int gridWidth, int gridHeight,
        float[] widthPositions, float[] heightPositions,
        Func<ObjectType> randomColor,
        Func<ObjectType, GameObject> cubePoolGetter,
        Func<Vector2Int> gapPosGetter,
        Action onFallStart,
        Action onFallDone)
    {
        this.grid = grid;
        W = gridWidth; H = gridHeight;
        widthPos = widthPositions; heightPos = heightPositions;
        rngColor = randomColor; getCubeFromPool = cubePoolGetter; getGapPos = gapPosGetter;
        this.onFallStart = onFallStart; this.onFallDone = onFallDone;
    }

    public void ResolveCascade(int maxLoops = 20)
    {
        int guard = 0;
        while (guard++ < maxLoops)
        {
            if (!ResolveOnce()) break;
        }
    }

    // 🔹 alt sınıf sadece “hangi hücreler temizlenecek?”i verir
    public abstract bool ResolveOnce();

    // --------- ORTAK YARDIMCILAR ---------

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

                        var fall = c.GetFall();
                        if (fall != null)
                        {
                            onFallStart?.Invoke();
                            fall.StartFallingToHeight(heightPos[targetY], onFallDone);
                        }
                        else
                        {
                            var lp = c.transform.localPosition;
                            c.transform.localPosition = new Vector3(lp.x, heightPos[targetY], lp.z);
                        }
                    }
                    writeY = targetY + 1;
                }
                else if (grid[x, y] != null)
                {
                    // engel varsa onun altından devam
                    writeY = y + 1;
                }
            }
            if (x == gap.x) grid[gap.x, gap.y] = null;
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
                    if (!go) continue;

                    go.SetActive(true);
                    var cube = go.GetComponent<Cube>();
                    float z = -y - 2f;

                    var fall = cube.GetFall();
                    if (fall != null)
                    {
                        // Yukarıdan doğurup düşür
                        var start = new Vector3(widthPos[x], 700f, z);
                        cube.SetProperties(start, color, x, y);
                        grid[x, y] = cube;

                        onFallStart?.Invoke();
                        fall.StartFallingToHeight(heightPos[y], onFallDone);
                    }
                    else
                    {
                        // Animasyon yoksa direkt yerine koy
                        var pos = new Vector3(widthPos[x], heightPos[y], z);
                        cube.SetProperties(pos, color, x, y);
                        grid[x, y] = cube;
                    }
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
