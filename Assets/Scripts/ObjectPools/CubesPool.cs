using System.Collections.Generic;
using UnityEngine;

public class CubesPool : MonoBehaviour
{
    public static CubesPool instance;
    public List<GameObject> blueCubePool;
    public List<GameObject> redCubePool;
    public List<GameObject> greenCubePool;
    public List<GameObject> yellowCubePool;
    public GameObject[] cubePrefabs;
    public Transform[] cubeParents;

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

    public GameObject GetNextInactiveCube(ObjectType type)
    {
        List<GameObject> cubePool = SetPool(type);
        foreach (var cube in cubePool)
        {
            if (!cube.activeInHierarchy)
            {
                return cube;
            }
        }

        return Instantiate(cubePrefabs[(int)type], cubeParents[(int)type]); ;
    }

    public void ResetPool()
    {
        for (int i = 0; i < 4; i++)
        {
            List<GameObject> cubePool = SetPool((ObjectType)i);
            foreach (var cube in cubePool)
            {
                cube.SetActive(false);
            }
        }

    }

    public List<GameObject> SetPool(ObjectType type)
    {
        if ((int)type == 0)
        {
            return blueCubePool;
        }
        else if ((int)type == 1)
        {
            return greenCubePool;
        }
        else if ((int)type == 2)
        {
            return redCubePool;
        }
        else if ((int)type == 3)
        {
            return yellowCubePool;
        }

        return null;
    }
}