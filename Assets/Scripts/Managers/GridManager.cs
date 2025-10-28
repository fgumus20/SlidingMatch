using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public float[] HeightPositions;
    public float[] WidthPositions;
    public GridObject[,] gridArray;
    int moveCount;
    string pattern;
    private IMatcher matcher;

    int gridWidth, gridHeight;
    [SerializeField] private float slideDuration = 0.18f;
    public Vector2Int gapPos;
    private bool isResolving;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetArrays(GridObject[,] gridArray,float[] widths, float[] heights, int moveCount,string patern)
    {
        this.gridArray = gridArray;
        this.WidthPositions = widths;
        this.HeightPositions = heights;
        this.moveCount = moveCount;
        this.gridWidth = widths.Length;
        this.gridHeight = heights.Length;
        this.pattern = patern;

    
        System.Func<ObjectType> rng = () => (ObjectType)UnityEngine.Random.Range(0, 4);
        System.Func<ObjectType, GameObject> poolGetter = (t) => CubesPool.instance.GetNextInactiveCube(t);
        System.Func<Vector2Int> gapGetter = () => gapPos;

        if (!string.IsNullOrEmpty(pattern) && pattern.ToLowerInvariant().Contains("square"))
        {
            matcher = new SquareMatcher(gridArray, gridWidth, gridHeight,
                WidthPositions, HeightPositions, rng, poolGetter, gapGetter,
                OnFallStart, OnFallDone);
        }
        else
        {
            matcher = new LineMatcher(gridArray, gridWidth, gridHeight,
                WidthPositions, HeightPositions, rng, poolGetter, gapGetter,
                OnFallStart, OnFallDone);
        }

        //FindAllNeighbours();
    }
    public GridObject[,] GetGridArray() {  return gridArray; }
    public int GetGridWidth() { return gridWidth; }
    public int GetGridHeight() {  return gridHeight; }


    public bool IsAdjacentToGap(Vector2Int pos)
    {
        return (Mathf.Abs(pos.x - gapPos.x) + Mathf.Abs(pos.y - gapPos.y)) == 1;
    }

    public Vector3 GridToLocal(Vector2Int p)
    {
        return new Vector3(WidthPositions[p.x], HeightPositions[p.y], -p.y - 2);
    }

    public void SlideIntoGap(Vector2Int from)
    {

        var cube = gridArray[from.x, from.y] as Cube;
        if (cube == null) return;
        if (!IsAdjacentToGap(from)) return;

        gridArray[gapPos.x, gapPos.y] = cube;
        cube.SetXandY(gapPos.x, gapPos.y);

        var targetLocal = GridToLocal(gapPos);

        cube.MoveToLocal(targetLocal, slideDuration);
        //cube.transform.localPosition = targetLocal;

        gridArray[from.x, from.y] = null;
        gapPos = from;

        GameManager.instance.DecreaseMoveCount();

        TryResolveStep();

    }
    private bool AnyFalling()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                if (gridArray[x, y] is Cube c && c.GetFall() != null && c.GetFall().IsFalling)
                    return true;
        return false;
    }

    private System.Collections.IEnumerator ResolveRoutine()
    {
        if (isResolving) yield break;
        isResolving = true;

        while (AnyFalling()) yield return null;

        while (matcher.ResolveOnce())
        {
            while (AnyFalling()) yield return null;
        }

        isResolving = false;
    }

    private int fallingCount = 0;
    private bool resolving = false;

    private void OnFallStart() { fallingCount++; }
    private void OnFallDone() { fallingCount--; if (fallingCount <= 0) TryResolveStep(); }

    private void TryResolveStep()
    {
        if (resolving) return;
        if (fallingCount > 0) return;
        StartCoroutine(ResolveStep());
    }

    private System.Collections.IEnumerator ResolveStep()
    {
        resolving = true;
        yield return null;

        bool didWork = matcher.ResolveOnce();
        resolving = false;

        if (didWork)
        {
            if (fallingCount <= 0) TryResolveStep();
        }
    }
}



