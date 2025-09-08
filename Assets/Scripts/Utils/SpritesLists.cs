using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesLists : MonoBehaviour
{
    public static SpritesLists instance;

    public Sprite[] sprites;
    public Sprite[] effectSprites;
    public Sprite[] obstacleParticles;
    public Sprite[] tntStates;
    public void Awake()
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
    public Sprite[] GetSprites() { return sprites;}
    public Sprite[] GetEffectsSprites() { return effectSprites;} 
    public Sprite[] GetObstacleParticles() { return obstacleParticles;}
    public Sprite[] GetTntStates() {  return tntStates;}
}
