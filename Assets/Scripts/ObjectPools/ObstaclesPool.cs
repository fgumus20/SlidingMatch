using System.Collections.Generic;
using UnityEngine;

public class ObstaclesPool : MonoBehaviour
{
    public static ObstaclesPool instance;
    public List<GameObject> obstaclesPool;

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

    public GameObject GetNextInactiveObstacle()
    {
        foreach (var obstacle in obstaclesPool)
        {
            if (!obstacle.activeInHierarchy)
            {
                return obstacle;
            }
        }
        return null;
    }

    public void ResetPool()
    {
        foreach (var cube in obstaclesPool)
        {
            cube.SetActive(false);
        }
    }
}
