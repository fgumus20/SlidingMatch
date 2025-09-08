using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool instance;
    public List<GameObject> effectPool;

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
   
    public GameObject GetNextInactiveEffect()
    {
        foreach (var effect in effectPool)
        {
            if (!effect.activeInHierarchy)
            {
                return effect;
            }
        }
        return null;
    }

    public void ResetPool()
    {
        foreach (var effect in effectPool)
        {
            effect.SetActive(false);
        }
    }
}
