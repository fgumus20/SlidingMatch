using System;
using System.Collections;
using UnityEngine;

public class CubeFall : MonoBehaviour
{
    public float height;

    public bool IsFalling { get; private set; }
    public event Action OnFallCompletedEvent;
    Coroutine running;

    public void StartFallingToHeight(float targetHeight, Action onComplete = null, float speed = 800f)
    {
        height = targetHeight;

        if (running != null) StopCoroutine(running);

        running = StartCoroutine(FallingRoutine(onComplete, speed));
    }

    public void StartFallingToHeight(float targetHeight)
    {
        StartFallingToHeight(targetHeight, null, 800f);
    }

    private IEnumerator FallingRoutine(Action onComplete, float fallSpeed)
    {
        IsFalling = true;

        while (transform.localPosition.y > height)
        {
            var lp = transform.localPosition;
            float ny = Mathf.MoveTowards(lp.y, height, fallSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(lp.x, ny, lp.z);
            yield return null;
        }
        var lp2 = transform.localPosition;
        transform.localPosition = new Vector3(lp2.x, height, lp2.z);

        IsFalling = false;
        running = null;

        onComplete?.Invoke();
        OnFallCompletedEvent?.Invoke();
    }
}
