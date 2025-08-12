using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Logo : MonoBehaviour
{
    [SerializeField] private Image logo;

    private void Start()
    {
        MakeSizeUpTween();
    }

    private void MakeSizeUpTween()
    {
        TweenManager.Instance.MakeTween(3f, HandleUpdate)
                    .OnComplete(HandleCallback);
    }

    private void HandleCallback()
    {
        TweenManager.Instance.MakeTween(2f, MakeSizeDownTween)
            .OnComplete(MakeSizeUpTween);
    }

    private void MakeSizeDownTween(float percent)
    {
        Vector3 from = logo.rectTransform.localScale;
        Vector3 to = Vector3.one;

        logo.rectTransform.localScale = Vector3.Lerp(from, to, percent);
    }

    private void HandleUpdate(float percent)
    {
        Vector3 from = logo.rectTransform.localScale;
        Vector3 to = Vector3.one * 1.1f;

        logo.rectTransform.localScale = Vector3.Lerp(from, to, percent);
    }
}
