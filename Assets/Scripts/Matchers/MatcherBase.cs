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
        // 🔹 1) Önce transform'ları topla (SetFalse etmeden!)
        var justCleared = new List<Transform>();

        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                justCleared.Add(c.transform);   // FX için sakla
            }
        }

        // 🔹 2) Sonra gerçekten temizle
        foreach (var p in cells)
        {
            if (grid[p.x, p.y] is Cube c)
            {
                c.SetFalse();                    // burada inactive olursa transform’u hâlâ bizde
                grid[p.x, p.y] = null;
                ApplyDamageToNeighbours(p.x, p.y);
            }
        }

        // 🔹 3) FX — SetFalse’tan SONRA da güvenle çalışır, çünkü elimizde referans var
        MatchFX.I?.PulseTiles(justCleared);
        MatchFX.I?.NudgeBoard(justCleared.Count >= 6 ? 14f : 10f);
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
                if (grid[x, y] != null) continue;

                // 1) Renk ve obje
                var color = rngColor();
                var go = getCubeFromPool != null ? getCubeFromPool(color) : null;
                if (go == null)
                {
                    Debug.LogWarning("[Refill] Pool'dan obje alınamadı, hücre atlandı.");
                    continue;
                }

                // 2) Parent ve ölçek güvenliği
                // (cubesParent kullanıyorsan: go.transform.SetParent(cubesParent, false);)
                // Eğer özel parent'ın yoksa worldPositionStays=false ile mevcut parent'ı koru
                go.transform.SetParent(go.transform.parent, false);

                // Prefab'ın varsayılan ölçeği (Cube.DefaultScale tanımlıysa onu kullan)
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

                // 3) Düşüş (varsa) / Doğrudan yerleştirme
                var fall = cube.GetFall();
                if (fall != null)
                {
                    // Yukarıdan başlat → düşür
                    var start = new Vector3(widthPos[x], 700f, z);
                    cube.SetProperties(start, color, x, y);
                    grid[x, y] = cube;
                    

                    // Spawn pop (ölçeği önce biraz küçült, sonra eski haline)
                    go.transform.localScale = defaultScale * 0.9f;
                    go.transform.DOScale(defaultScale, 0.12f).SetEase(Ease.OutQuad).SetUpdate(true);
                    cube.ResetVisual();
                    onFallStart?.Invoke();
                    fall.StartFallingToHeight(heightPos[y], onFallDone);
                }
                else
                {
                    // Animasyon yoksa doğrudan final pozisyona
                    var pos = new Vector3(widthPos[x], heightPos[y], z);
                    cube.SetProperties(pos, color, x, y);
                    grid[x, y] = cube;
                    
                    // Spawn pop
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
