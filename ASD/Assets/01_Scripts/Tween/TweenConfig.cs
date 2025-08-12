using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TweenEnum
{
    Linear,
}

/// <summary>
/// Linear = "쭉 한 쪽으로 진행"
/// YoYo = "끝났으면 다시 처음값으로 돌아갔다가 다시 진행"
/// </summary>
public enum TweenLoopEnum
{
    Linear,
    YoYo,
}

[Serializable]
public struct TweenCurve
{
    public TweenEnum tween;
    public AnimationCurve curve;
}

[CreateAssetMenu(menuName = "TweenConfig")]
public class TweenConfig : ScriptableObject
{
    public int maxTweenCount = 40;

    public List<TweenCurve> tweens = new();

    public AnimationCurve GetTween(TweenEnum tween)
    {
        return tweens.FirstOrDefault(x => x.tween == tween).curve;
    }
}
