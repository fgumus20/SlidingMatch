using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchFX : MonoBehaviour
{
    public static MatchFX I;

    [Header("Scene Refs")]
    public Transform boardRoot;   // Grid parent ya da GridManager objesi
    public Camera mainCam;        // (opsiyonel) küçük shake

    [Header("UI Refs (opsiyonel)")]
    public RectTransform movesText;
    public RectTransform targetText;
    public RectTransform winPanel;
    public RectTransform losePanel;

    void Awake()
    {
        I = this;
        if (boardRoot == null)
        {
            var gm = FindObjectOfType<GridManager>();
            if (gm) boardRoot = gm.transform;
        }
        Debug.Log("[MatchFX] Ready. boardRoot=" + (boardRoot ? boardRoot.name : "NULL"));
    }

    // --- TILE FX ---
    public void PulseTiles(IEnumerable<Transform> tiles)
    {
        foreach (var t in tiles)
        {
            if (!t) continue;
            t.DOKill();
            DOTween.Sequence()
                .Append(t.DOScale(1.12f, 0.08f))
                .Append(t.DOScale(1f, 0.12f))
                .Play();
        }
    }

    // --- BOARD / CAMERA ---
    public void NudgeBoard(float strength = 10f)
    {
        if (boardRoot)
            boardRoot.DOPunchPosition(Vector3.down * strength, 0.15f, 10, 0.85f);
        if (mainCam)
            mainCam.DOShakeRotation(0.15f, 1.5f, 10, 90f, true);
    }

    // --- UI ---
    public void BumpCounter(RectTransform rt)
    {
        if (!rt) return;
        rt.DOKill();
        rt.DOPunchScale(Vector3.one * 0.15f, 0.2f, 8, 0.9f);
    }

    public void PanelIn(RectTransform panel)
    {
        if (!panel) return;
        panel.gameObject.SetActive(true);
        panel.localScale = Vector3.one * 0.85f;
        panel.DOScale(1f, 0.22f).SetEase(Ease.OutBack);
    }

    public void PanelOut(RectTransform panel)
    {
        if (!panel) return;
        panel.DOScale(0.9f, 0.15f).OnComplete(() => panel.gameObject.SetActive(false));
    }
}
