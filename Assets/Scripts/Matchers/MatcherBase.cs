using DG.Tweening;
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

    public abstract bool ResolveOnce();



    protected void ClearCells(HashSet<Vector2Int> cells)
    {

        var justCleared = new List<Transform>();

        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                justCleared.Add(c.transform);
            }
        }

        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                c.SetFalse();
                grid[p.x, p.y] = null;
                ApplyDamageToNeighbours(p.x, p.y);
            }
        }

        MatchFX.I?.PulseTiles(justCleared);
        //MatchFX.I?.NudgeBoard(justCleared.Count >= 6 ? 14f : 10f);
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
                if (grid[x, y] != null) continue;

                var color = rngColor();
                var go = getCubeFromPool != null ? getCubeFromPool(color) : null;
                if (go == null)
                {
                    Debug.LogWarning("[Refill] Pool'dan obje alınamadı, hücre atlandı.");
                    continue;
                }

                go.transform.SetParent(go.transform.parent, false);

                Vector3 defaultScale = (Cube.DefaultScale == Vector3.zero) ? Vector3.one : Cube.DefaultScale;
                go.transform.localScale = defaultScale;

                go.SetActive(true);

                var cube = go.GetComponent<Cube>();
                if (cube == null)
                {
                    Debug.LogError("[Refill] GameObject'te Cube component yok!");
                    continue;
                }

                float z = -y - 2f;

                var fall = cube.GetFall();
                if (fall != null)
                {
                    var start = new Vector3(widthPos[x], 700f, z);
                    cube.SetProperties(start, color, x, y);
                    grid[x, y] = cube;
                    
                    go.transform.localScale = defaultScale * 0.9f;
                    go.transform.DOScale(defaultScale, 0.12f).SetEase(Ease.OutQuad).SetUpdate(true);
                    cube.ResetVisual();
                    onFallStart?.Invoke();
                    fall.StartFallingToHeight(heightPos[y], onFallDone);
                }
                else
                {
                    var pos = new Vector3(widthPos[x], heightPos[y], z);
                    cube.SetProperties(pos, color, x, y);
                    grid[x, y] = cube;
                    
                    go.transform.localScale = defaultScale * 0.9f;
                    go.transform.DOScale(defaultScale, 0.12f).SetEase(Ease.OutQuad).SetUpdate(true);
                    cube.ResetVisual();

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
