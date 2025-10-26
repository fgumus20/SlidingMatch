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
    private List<GridObject> FindNeighbour(GridObject cube, HashSet<GridObject> visited)
    {
        List<GridObject> connected = new List<GridObject>();

        int x = cube.GetX();
        int y = cube.GetY();

        visited.Add(cube);
        connected.Add(cube);

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
            {
                GridObject neighbour = gridArray[nx, ny];
                if ((int)neighbour.GetType1() <4 && !visited.Contains(neighbour))
                {
                    if (cube.GetType1() == neighbour.GetType1())
                    {
                        visited.Add(neighbour);
                        connected.AddRange(FindNeighbour(neighbour, visited));
                    }
                }
            }
        }
        return connected;
    }
    private List<GridObject> FindObstacles(GridObject cube)
    {
        List<GridObject> obstacles = new List<GridObject>();
        int x = cube.GetX();
        int y = cube.GetY();
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
            {
                GridObject obstacle = gridArray[nx, ny];
                if ((int)obstacle.GetType1() > 3 && !obstacles.Contains(obstacle) && (int)obstacle.GetType1() != 7)
                {
                    obstacles.Add(obstacle);
                }
            }
        }
        return obstacles;
    }
    public void FindAllNeighbours()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if ((int)gridArray[x, y].GetType1() < 4)
                {
                    Cube cube = (Cube)gridArray[x, y];
                    cube.ResetNeighbour();
                    cube.ResetObstacle();
                    HashSet<GridObject> visited = new HashSet<GridObject>();
                    cube.SetNeighbourList(FindNeighbour(cube, visited));
                    cube.SetObstacleList(FindObstacles(cube));
                }
            }
        }
    }
    public void ExplodeCubes(Cube cube, List<GridObject> neighbourList, List<GridObject> obstacles)
    {
        List<int> emptyColumns = new List<int>();
        bool isTnt = false;
        GameManager.instance.DecreaseMoveCount();
        foreach (Cube neighbour in neighbourList)   
        {
            if(neighbourList.Count > 4)
            {
                if (cube == neighbour)
                {
                    foreach (GridObject item in neighbour.GetObstacleList())
                    {
                        if (!obstacles.Contains(item))
                        {
                            obstacles.Add(item);
                        }
                    }
                    isTnt = true;
                    continue;
                }
            }
            int x = neighbour.GetX();
            int y = neighbour.GetY();
            foreach (GridObject item in neighbour.GetObstacleList())
            {
                if (!obstacles.Contains(item))
                {
                    obstacles.Add(item);
                }
            }
            neighbour.SetFalse();
            gridArray[x, y] = null;
            if (!emptyColumns.Contains(x))
            {
                emptyColumns.Add(x);
            }
        }

        foreach (ObstacleController obstacle in obstacles)
        {
            if (obstacle.GetType1() == ObjectType.Vase || obstacle.GetType1() == ObjectType.Box)
            {
                obstacle.TakeDamage();
                if(obstacle.GetHealth() == 0)
                {
                    GameManager.instance.DecreaseObstacleCount(obstacle.GetType1());
                    int x = obstacle.GetX();
                    int y = obstacle.GetY();
                    obstacle.SetFalse();
                    gridArray[x,y] = null;
                    if (!emptyColumns.Contains(x))
                    {
                        emptyColumns.Add(x);
                    }
                }
            }
        }
        if(isTnt == true)
        {
            List<GameObject> poolList = CubesPool.instance.SetPool(cube.GetType1());
            poolList.Remove(cube.gameObject);
            cube.SetType(ObjectType.TNT);
            cube.SetSprite(SpritesLists.instance.GetSprites()[(int)ObjectType.TNT]);
            cube.AddComponent<TNT>();
            cube.ResetNeighbour();
            cube.ResetObstacle();
        }
        foreach(int i in emptyColumns)
        {
            AdjustGridColumn(i);
        }
        FindAllNeighbours();
    }
    public void AdjustGridColumn(int x)
    {    
        for (int y = 0; y < gridHeight; y++)
        {
            if (gridArray[x, y] == null ) 
            {          
               for(int i = y+1; i < gridHeight; i++)
                {
                    if (gridArray[x, i] != null && gridArray[x,i].GetType1() != ObjectType.Box && gridArray[x,i].GetType1() != ObjectType.Stone)
                    {   
                        gridArray[x, y] = gridArray[x, i];
                        gridArray[x, y].SetXandY(x, y);
                        gridArray[x, y].GetFall().StartFallingToHeight(HeightPositions[y]);
                        gridArray[x, i] = null;
                        break;
                    }
                }
            }
        }
        CreateCubes(x);
    }
    public void CreateCubes(int x)
    {
        for (int i = 0; i < gridHeight; ++i)
        {
            if (gridArray[x,i] == null)
            {
                ObjectType type = (ObjectType)UnityEngine.Random.Range(0, 4);
                GameObject cubeObj = CubesPool.instance.GetNextInactiveCube(type);
                cubeObj.SetActive(true);
                Cube cube = cubeObj.GetComponent<Cube>();

                Vector3 position = new Vector3(WidthPositions[x], 700, -gridHeight - 2);
                
                cube.SetProperties(position, type, x, i);
                gridArray[x, i] = cube;
                gridArray[x, i].GetFall().StartFallingToHeight(HeightPositions[i]);

            }
        }
    }

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



