using System;
using System.Collections;
using UnityEngine;

public class CubeFall : MonoBehaviour
{
    public float height;

    // >>> EKLER <<<
    public bool IsFalling { get; private set; }                 // durum bayraðý
    public event Action OnFallCompletedEvent;                   // event (istersen)
    Coroutine running;                                          // ayný anda tek rutin

    // Overload: onComplete callback ve hýz parametresi
    public void StartFallingToHeight(float targetHeight, Action onComplete = null, float speed = 800f)
    {
        height = targetHeight;

        // varsa önceki animasyonu sonlandýr
        if (running != null) StopCoroutine(running);

        running = StartCoroutine(FallingRoutine(onComplete, speed));
    }

    // Eski imza hâlâ çalýþsýn (geriye uyum)
    public void StartFallingToHeight(float targetHeight)
    {
        StartFallingToHeight(targetHeight, null, 800f);
    }

    private IEnumerator FallingRoutine(Action onComplete, float fallSpeed)
    {
        IsFalling = true;

        // düþüþ
        while (transform.localPosition.y > height)
        {
            var lp = transform.localPosition;
            float ny = Mathf.MoveTowards(lp.y, height, fallSpeed * Time.deltaTime);
            transform.localPosition = new Vector3(lp.x, ny, lp.z);
            yield return null;
        }
        // tam hizala
        var lp2 = transform.localPosition;
        transform.localPosition = new Vector3(lp2.x, height, lp2.z);

        IsFalling = false;
        running = null;

        // callback’ler
        onComplete?.Invoke();
        OnFallCompletedEvent?.Invoke();
    }
}
