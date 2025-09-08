using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridInitalizer : MonoBehaviour
{
    public GridObject[,] gridArray;

    public Sprite[] sprites;
    public Sprite[] effectSprites;

    public RectTransform gridRect;
    float gridHeight;
    float gridWidth;
    public Vector2Int gapPos; 
    int gameWidth;
    int gameHeight;
    int moveCount;
    
    Level level;

    public float[] WidthPositionsEven;
    public float[] WidthPositionsOdd;

    public float[] HeightPositionsEven;
    public float[] HeightPositionsOdd;

    public float[] HeightPositions;
    public float[] WidthPositions;

    List<string> GridJSON = new List<string>();

    void Start()
    {   
        LevelInfo();
        CalcGridSize(gameWidth, gameHeight);
        gridArray = new GridObject[gameWidth, gameHeight];
        InitiliazeCubes();
        GridManager.instance.SetArrays(gridArray, WidthPositions, HeightPositions, moveCount);
    }

    void CalcGridSize(int gameWidth, int gameHeight)
    {
        gridWidth = gameWidth * 75;
        gridHeight = gameHeight * 78.9f;
        gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
    }

    void LevelInfo()
    {
        level = LevelManager.instance.LoadLevel(LevelManager.instance.GetCurrentLevel());

        gameWidth = level.GridWidth;
        gameHeight = level.GridHeight;
        moveCount = level.MoveCount;
        GridJSON = level.Grid;
        GameManager.instance.SetGameNum(moveCount,0,0,0);
        WidthPositions = new float[gameWidth];
        HeightPositions = new float[gameHeight];

        if (gameHeight % 2 == 0)
            Array.Copy(HeightPositionsEven, ((10 - gameHeight) / 2), HeightPositions, 0, gameHeight);
        else
            Array.Copy(HeightPositionsOdd, ((9 - gameHeight) / 2), HeightPositions, 0, gameHeight);

        if (gameWidth % 2 == 0)
            Array.Copy(WidthPositionsEven, ((10 - gameWidth) / 2), WidthPositions, 0, gameWidth);
        else
            Array.Copy(WidthPositionsOdd, ((9 - gameWidth) / 2), WidthPositions, 0, gameWidth);
    }

    void InitiliazeCubes()
    {

        for (int h = 0; h < gameHeight; h++)
        {
            for (int w = 0; w < gameWidth; w++)
            {


                string cell = GridJSON[w + h * gameWidth];


                if (string.Equals(cell, "em"))
                {
                    gridArray[w, h] = null;

                    GridManager.instance.gapPos = new Vector2Int(w, h);
                    continue;
                }
                ObjectType type = TypeCalc(GridJSON[w + h*gameWidth]);

                if ((int)type < 4)
                {
                    GameObject cubeObj = CubesPool.instance.GetNextInactiveCube(type);
                    cubeObj.SetActive(true);
                    Cube cube = cubeObj.GetComponent<Cube>();

                    Vector3 position = new Vector3(WidthPositions[w], HeightPositions[h], -h - 2);
                    cube.SetProperties(position, type, w, h);
                    gridArray[w, h] = cube;
                }
                else
                {
                    GameObject obstacleObj = ObstaclesPool.instance.GetNextInactiveObstacle();
                    obstacleObj.SetActive(true);
                    ObstacleController obstacle = obstacleObj.GetComponent<ObstacleController>();
                    Vector3 position = new Vector3(WidthPositions[w], HeightPositions[h], -h - 2);
                    obstacle.GetComponent<ObstacleController>().SetProperties(position, sprites[(int)type], type, w, h);
                    if(type == ObjectType.Box)
                    {
                        GameManager.instance.SetBoxCount(GameManager.instance.GetBoxCount() + 1);
                    }else if(type == ObjectType.Vase) 
                    {
                        GameManager.instance.SetVaseCount(GameManager.instance.GetVaseCount() + 1);
                    }else if(type == ObjectType.Stone)
                    {
                        GameManager.instance.SetStoneCount(GameManager.instance.GetStoneCount() + 1);
                    }
                    
                    gridArray[w, h] = obstacle;
                }                      
            }
        }
    }

    ObjectType TypeCalc(string type)
    {
        if (string.Equals(type, "b"))
        {
            return ObjectType.Blue;
        }
        else if (string.Equals(type, "g"))
        {
            return ObjectType.Green;
        }
        else if (string.Equals(type, "r"))
        {
            return ObjectType.Red;
        }
        else if (string.Equals(type, "y"))
        {
            return ObjectType.Yellow;
        }
        else if (string.Equals(type, "rand"))
        {
            return (ObjectType)UnityEngine.Random.Range(0, 4);
        }
        else if (string.Equals(type, "bo"))
        {
            return ObjectType.Box;
        }
        else if (string.Equals(type, "v"))
        {
            return ObjectType.Vase;
        }
        else if (string.Equals(type, "s"))
        {
            return ObjectType.Stone;
        }

        return ObjectType.Blue;

    }

}