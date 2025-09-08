using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour, GridObject
{
    public Transform obsPosition;
    public SpriteRenderer spriteRenderer;
    ObjectType type;
    int health;
    int x, y;
    public void SetProperties(Vector3 vector, Sprite sprite, ObjectType type, int x , int y)
    {
        SetPosition(vector);
        SetSprite(sprite);
        SetType(type);
        SetXandY(x, y);
        SetHealth(); 
    }
    public void SetPosition(Vector3 vector)
    {
        obsPosition.localPosition = vector;
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
    public int GetHealth() { return health; }
    public void TakeDamage() 
    { 
        health -= 1;
        if(GetType1() == ObjectType.Vase &&  health == 1)
        {
            SetSprite(SpritesLists.instance.GetSprites()[8]);
        }
    }
    public void SetFalse()
    {
        GameObject effect = EffectPool.instance.GetNextInactiveEffect();
        effect.SetActive(true);
        effect.GetComponent<EffectController>().play(obsPosition, GetType1());
        this.gameObject.SetActive(false);
    }
    private void SetHealth()
    {
        if (this.type == ObjectType.Vase)
        {
            this.health = 2;
            this.gameObject.AddComponent<CubeFall>();
        }
        else { health = 1; }   
    }
    public CubeFall GetFall()
    {
        return this.GetComponent<CubeFall>();
    }
    public ObjectType GetType1() { return type; }
}
