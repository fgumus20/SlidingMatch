using UnityEngine;
using DG.Tweening;

public class TitleBounce : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        
        rectTransform.localScale = Vector3.one * 0.95f;

        rectTransform
            .DOAnchorPosY(rectTransform.anchoredPosition.y + 10f, 0.4f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
