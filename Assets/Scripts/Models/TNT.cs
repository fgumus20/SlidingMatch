using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TNT : MonoBehaviour
{
    private HashSet<GridObject> EffectArea(Cube TNT, HashSet<GridObject> visited, int area)
    {
        GridManager gridManager = GridManager.instance;
        GridObject[,] gridArray = gridManager.GetGridArray();
        HashSet<GridObject> effectArea = new HashSet<GridObject>();

        int x = TNT.GetX();
        int y = TNT.GetY();
        int gridWidth = gridManager.GetGridWidth();
        int gridHeight = gridManager.GetGridHeight();

        for (int i = -area; i <= area; i++)
        {
            for (int j = -area; j <= area; j++)
            {
                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    GridObject gridObject = gridArray[nx, ny];
                    if (gridObject != null && !visited.Contains(gridObject))
                    {
                        visited.Add(gridObject);
                        effectArea.Add(gridObject);
                        if (gridObject.GetType1() == ObjectType.TNT && (Object)gridObject != TNT)
                        {
                            Cube nextTNT = (Cube)gridObject;
                            effectArea.UnionWith(EffectArea(nextTNT, visited, 2));
                        }
                    }
                }
            }
        }

        return effectArea;
    }
    public void explodeTNTtest(Cube TNT)
    {
        GridManager gridManager = GridManager.instance;
        GameManager.instance.DecreaseMoveCount();
        bool isDouble = false;
        List<int> emptyColumns = new List<int>();
        GridObject[,] gridArray = gridManager.GetGridArray();
        HashSet<GridObject> effectArea = EffectArea(TNT, new HashSet<GridObject>(), 2);

        foreach (GridObject obj in effectArea)
        {
            int xdo = TNT.GetX();
            int ydo = TNT.GetY();

            int xdo2 = obj.GetX();
            int ydo2 = obj.GetY();

            if (((xdo == xdo2 && System.Math.Abs(ydo - ydo2) == 1) ||
                (ydo == ydo2 && System.Math.Abs(xdo - xdo2) == 1)) &&
                obj.GetType1() == ObjectType.TNT)
            {
                isDouble = true;
            }
        }

        if (!isDouble)
        {
            foreach (GridObject gridobject in effectArea)
            {
                if (gridobject.GetType1() == ObjectType.Vase)
                {
                    ObstacleController obstacle = (ObstacleController)gridobject;
                    obstacle.TakeDamage();
                    if (obstacle.GetHealth() > 0) { continue; }
                }
                if((int)gridobject.GetType1() < 7 && (int) gridobject.GetType1() > 3)
                {
                    GameManager.instance.DecreaseObstacleCount(gridobject.GetType1());
                }
                int x = gridobject.GetX();
                int y = gridobject.GetY();
                gridobject.SetFalse();
                gridArray[x, y] = null;
                if (!emptyColumns.Contains(x))
                {
                    emptyColumns.Add(x);
                }

            }

            foreach (int i in emptyColumns)
            {
                gridManager.AdjustGridColumn(i);
            }
            gridManager.FindAllNeighbours();
        }
        else
        {
            DoubleTNTExplode(TNT);
        }

    }
    private void DoubleTNTExplode(Cube TNT)
    {
        GridManager gridManager = GridManager.instance;
        List<int> emptyColumns = new List<int>();
        GridObject[,] gridArray = gridManager.GetGridArray();
        HashSet<GridObject> effectArea = EffectArea(TNT, new HashSet<GridObject>(), 3);

        foreach (GridObject gridobject in effectArea)
        {
            if (gridobject.GetType1() == ObjectType.Vase)
            {
                ObstacleController obstacle = (ObstacleController)gridobject;
                obstacle.TakeDamage();
                if (obstacle.GetHealth() > 0) { continue; }
            }
            if ((int)gridobject.GetType1() < 7 && (int)gridobject.GetType1() > 3)
            {
                GameManager.instance.DecreaseObstacleCount(gridobject.GetType1());
            }
            int x = gridobject.GetX();
            int y = gridobject.GetY();
            gridobject.SetFalse();
            gridArray[x, y] = null;
            if (!emptyColumns.Contains(x))
            {
                emptyColumns.Add(x);
            }

        }

        foreach (int i in emptyColumns)
        {
            gridManager.AdjustGridColumn(i);
        }
        gridManager.FindAllNeighbours();
    }
}
