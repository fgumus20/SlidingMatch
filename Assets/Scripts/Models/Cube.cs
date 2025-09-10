using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour, GridObject
{

    public Transform cubePosition;
    public SpriteRenderer spriteRenderer;
    public List<GridObject> neighbourList;
    public List<GridObject> obstacles;
    public const float BASE_SCALE = 52f;
    ObjectType type;
    int x, y;
    public ParticleSystem explosionEffect;
    public static Vector3 DefaultScale = Vector3.zero;

    public void SetProperties(Vector3 vector, ObjectType type, int x, int y)
    {
        neighbourList = new List<GridObject>();
        obstacles = new List<GridObject>();
        ResetVisual();
        SetType(type);
        SetPosition(vector);
        SetXandY(x, y);
    }

    public void ResetVisual()
    {
        // Eski tween/efekt kalýntýlarýný temizle
        transform.DOKill(true);
        if (cubePosition) cubePosition.DOKill(true);

        // KÖK: 52x52x52 (dünya/board ölçeði)
        transform.localScale = Vector3.one * BASE_SCALE;
        transform.localRotation = Quaternion.identity;


    }

    public void SetPosition(Vector3 vector)
    {
        transform.localPosition = vector;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetType(ObjectType newType)
    {
        type = newType;
    }

    public void SetXandY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int GetX() { return x; }
    public int GetY() { return y; }

    public void AddNeighbourList(Cube neighbour)
    {
        if (neighbour != null && !neighbourList.Contains(neighbour))
        {
            neighbourList.Add(neighbour);
        }
    }
    public void SetNeighbourList(List<GridObject> neighbourList)
    {
        this.neighbourList = neighbourList;
        if (neighbourList.Count > 4)
        {
            SetSprite(SpritesLists.instance.GetTntStates()[(int)type]);
        }
        else
        {
            SetSprite(SpritesLists.instance.GetSprites()[(int)type]);
        }
    }
    public List<GridObject> GetNeighbourList() { return this.neighbourList; }

    public void SetObstacleList(List<GridObject> obstacles) { this.obstacles = obstacles; }
    public List<GridObject> GetObstacleList() { return this.obstacles; }

    public void ResetObstacle() { obstacles.Clear(); }

    public ObjectType GetType1() { return type; }

    public void ResetNeighbour()
    {
        neighbourList.Clear();
    }

    public int lenNeighbour()
    {
        return neighbourList.Count;
    }

    public void SetFalse()
    {
        if (GetType1() != ObjectType.TNT)
        {
            GameObject effect = EffectPool.instance.GetNextInactiveEffect();
            effect.SetActive(true);
            effect.GetComponent<EffectController>().play(cubePosition, GetType1());
        }

        this.gameObject.SetActive(false);
    }
    public CubeFall GetFall()
    {
        return this.GetComponent<CubeFall>();
    }
    void OnMouseDown()
    {
        Vector2Int pos = new Vector2Int(GetX(), GetY());
        if (GridManager.instance != null && GridManager.instance.IsAdjacentToGap(pos))
        {
            GridManager.instance.SlideIntoGap(pos);
        }
    }


}
