using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public ParticleSystem effect;
    public Transform effectPosition;
    private List<Sprite> addedSprites = new List<Sprite>();
    public void play(Transform position,ObjectType type)
    {
        SetPosition(position);
        if((int) type < 4)
        {
            
            effect.textureSheetAnimation.AddSprite(SpritesLists.instance.GetEffectsSprites()[(int)type]);
        }
        else if ( (int)type < 7)
        {
            effect.textureSheetAnimation.AddSprite(SpritesLists.instance.GetObstacleParticles()[(int)type % 4 * 3]);
            effect.textureSheetAnimation.AddSprite(SpritesLists.instance.GetObstacleParticles()[(int)type % 4 * 3 + 1]);
            effect.textureSheetAnimation.AddSprite(SpritesLists.instance.GetObstacleParticles()[(int)type % 4 * 3 + 2]);
        }  

        effect.Play();
        StartCoroutine(DeactivateAfterDuration(type));
    }
    public void SetPosition(Transform position)
    {
        effectPosition.position = position.position;
        Vector3 updatedPosition = effectPosition.localPosition;
        updatedPosition.z = -15;
        effectPosition.localPosition = updatedPosition;
    }
    private IEnumerator DeactivateAfterDuration(ObjectType type)
    {
        float duration = 1; 
        yield return new WaitForSeconds(duration);
        if((int) type < 4)
        {
            effect.textureSheetAnimation.RemoveSprite(0);
        }
        else
        {
            effect.textureSheetAnimation.RemoveSprite(0);
            effect.textureSheetAnimation.RemoveSprite(0);
            effect.textureSheetAnimation.RemoveSprite(0);
        }
        gameObject.SetActive(false);
    }
}
