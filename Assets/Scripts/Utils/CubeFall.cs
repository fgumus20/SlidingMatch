using System.Collections;
using UnityEngine;

public class CubeFall : MonoBehaviour
{
    private float fallSpeed;
    public float height;

    public void StartFallingToHeight(float targetHeight)
    {
        height = targetHeight;
        StartCoroutine(FallingRoutine());
    }
    private IEnumerator FallingRoutine()
    {
        fallSpeed = 800f;

        while (transform.localPosition.y > height)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                Mathf.MoveTowards(transform.localPosition.y, height, fallSpeed * Time.deltaTime),
                transform.localPosition.z
            );
            yield return null;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
    }
}
