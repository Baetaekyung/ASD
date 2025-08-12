using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TweenEnum
{
    Linear,
}

/// <summary>
/// Linear = "�� �� ������ ����"
/// YoYo = "�������� �ٽ� ó�������� ���ư��ٰ� �ٽ� ����"
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
