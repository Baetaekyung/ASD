using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tween�� ���� ��ü
/// </summary>
public class TweenUpdater : IPoolable
{
    #region Events
    private event Action<float> OnTweenEventHandler;
    private event Action OnStarted;
    private event Action OnCompleted;
    private event Action OnBetweenTime;
    #endregion

    #region Times
    public float timer;
    public float duration;
    public float timeScale = 1f;
    private float _currentBetweenEventElapsedTime = 0f;
    private float _betweenInvokeTime;
    #endregion

    #region Tween Config
    public TweenEnum tweenEnum;
    public AnimationCurve tweenCurve;
    #endregion

    // check for delete
    private bool _isFinish = false;
    public bool IsFinish => _isFinish;

    public TweenUpdater()
    {
        Clear();
    }

    public void Initialize(float duration, Action<float> onUpdate)
    {
        this.duration = duration;
        OnTweenEventHandler = onUpdate;
    }

    /// <summary>
    /// Tween ������Ʈ �ڵ鷯
    /// </summary>
    /// <param name="deltaTime">Second per frame</param>
    public void Update(float deltaTime)
    {
        OnStarted?.Invoke();

        timer += deltaTime * timeScale;
        float percent = timer / duration;

        float eval = tweenCurve.Evaluate(percent);
        OnTweenEventHandler?.Invoke(eval);

        if (timer >= duration)
        {
            _isFinish = true;
            OnCompleted?.Invoke();
            return;
        }
    }

    /// <summary>
    /// Tween�� ����� ����
    /// </summary>
    /// <param name="tween">Curve��� ���� Tween Config���� Ȯ��</param>
    public TweenUpdater SetCurve(TweenEnum tween)
    {
        tweenCurve = TweenManager.Instance.GetCurve(tween);

        return this;
    }

    /// <summary>
    /// Ʈ���� ������Ʈ Ÿ�� ����
    /// </summary>
    /// <param name="timeScale">TimeScale</param>
    public TweenUpdater SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;

        return this;
    }

    /// <summary>
    /// Tween�� Unity�� TimeScale�� �����ϵ��� ����
    /// </summary>
    /// <param name="ignoreTimeScale">default == true, if it true make it ignore UNITY timeScale</param>
    public TweenUpdater SetUpdate(bool ignoreTimeScale = true)
    {
        if (ignoreTimeScale)
            TweenManager.Instance.MakeTweenIgnoreTimeScale(this);

        return this;
    }

    /// <summary>
    /// Ʈ�� �� �׼� ����
    /// </summary>
    /// <param name="onComplete">Tween�� �������� ����� �Լ�, Loop == true�� ������ �ٽ� �ݺ��� ������ ����</param>
    public TweenUpdater OnComplete(Action onComplete)
    {
        OnCompleted += onComplete;

        return this;
    }

    /// <summary>
    /// Ʈ�� ���� ����� �׼� ����
    /// </summary>
    /// <param name="between">���� ����</param>
    /// <param name="onBetween">����� �׼�</param>
    public TweenUpdater OnBetween(float between, Action onBetween)
    {
        OnBetweenTime += onBetween;
        _betweenInvokeTime = between;

        if (_currentBetweenEventElapsedTime >= _betweenInvokeTime)
        {
            _currentBetweenEventElapsedTime = 0f;
            OnBetweenTime?.Invoke();
        }

        return this;
    }

    /// <summary>
    /// Ʈ�� ���� �׼� ����
    /// </summary>
    /// <param name="onStart">Tween�� �������� ����� �Լ�, Loop == true�� ������ �ٽ� �ݺ��� ������ ����</param>
    public TweenUpdater OnStart(Action onStart)
    {
        OnStarted += onStart;

        return this;
    }

    #region Pool
    public void OnGet() { }

    public void OnReturn()
    {
        Clear();
    }
    #endregion

    private void Clear()
    {
        timer = 0f;
        duration = 0f;

        tweenEnum = TweenEnum.Linear;
        timeScale = 1f;

        tweenCurve = TweenManager.Instance.GetCurve(TweenEnum.Linear);

        OnTweenEventHandler = null;
        OnStarted = null;
        OnCompleted = null;

        _isFinish = false;
    }

}

[Singleton(SingletonFlag.DontDestroy)]
public class TweenManager : MonoSingleton<TweenManager>
{
    private readonly WaitForSecondsRealtime UnscaledDelta = new(0f);
    private readonly Pool<TweenUpdater> pool = new Pool<TweenUpdater>();

    // Config�� Tween�� ��� ����
    private TweenConfig tweenConfig;

    #region Tween collections
    private List<TweenUpdater> _existTweens;
    private List<TweenUpdater> _existTweensIgnoreTimeScale;

    private bool _listChanged = false;
    private bool _listChangedIgnoreTimeScale = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (tweenConfig == null)
            tweenConfig = Resources.Load<TweenConfig>("TweenSetting/TweenConfig");

        Initialize(tweenConfig.maxTweenCount == 0 ? 40 : tweenConfig.maxTweenCount);
    }

    private void Initialize(int maxTweenCount)
    {
        pool.Initialize((maxTweenCount / 2), (maxTweenCount / 4), maxTweenCount);
        _existTweens = new List<TweenUpdater>(maxTweenCount);
        DebugEx.CLog($"TweenManager Initialize, Tween max count: {maxTweenCount}", "green");

        StartCoroutine(nameof(CustomLoopRoutine));
    }

    private void Update()
    {
        if (_listChanged)
        {
            _listChanged = false;
            return;
        }

        TweenLifeCycle(_existTweens, Time.deltaTime, "normal");
        _listChanged = false;
    }

    #region MakeTweens

    /// <summary>
    /// Tween�� �����Ѵ�
    /// </summary>
    /// <param name="duration">Tween�� ���ӵǴ� �ð�</param>
    /// <param name="onUpdate">Tween�� ������Ʈ �� ���� �Ͼ �׼�</param>
    public TweenUpdater MakeTween(float duration, Action<float> onUpdate)
    {
        TweenUpdater tween = pool.Get();
        tween.Initialize(duration, onUpdate);

        _existTweens.Add(tween);
        _listChanged = true;

        DebugEx.CLog("Tween maked!!", "blue");
        return tween;
    }

    /// <summary>
    /// Tween�� �����Ѵ�
    /// </summary>
    /// <param name="duration">Tween�� ���ӵǴ� �ð�</param>
    /// <param name="onUpdate">Tween�� ������Ʈ �� ���� �Ͼ �׼�</param>
    public TweenUpdater MakeTween(Action<float> onUpdate, float duration)
    {
        TweenUpdater tween = pool.Get();
        tween.Initialize(duration, onUpdate);

        _existTweens.Add(tween);
        _listChanged = true;

        DebugEx.CLog("Tween maked!!", "blue");
        return tween;
    }

    #endregion

    #region Tween life cycle
    private void TweenLifeCycle(List<TweenUpdater> tweenUpdaters, float deltaTime, string nameOfCollection)
    {
        if (tweenUpdaters == null || tweenUpdaters.Count == 0) return;

        foreach (var tween in tweenUpdaters)
        {
            if (tween.IsFinish)
            {
                DeleteFinishTweens(tweenUpdaters, nameOfCollection);

                return;
            }
        }

        // Todo: ToArray �ذ� ��� ã��
        foreach (var tween in tweenUpdaters.ToArray())
            tween.Update(deltaTime);
    }

    private IEnumerator CustomLoopRoutine()
    {
        DebugEx.CLog("Tween custom loop started!!", "blue");

        while (isActiveAndEnabled)
        {
            if (_listChangedIgnoreTimeScale)
            {
                _listChangedIgnoreTimeScale = false;
                yield return null;
            }

            TweenLifeCycle(_existTweensIgnoreTimeScale, Time.unscaledDeltaTime, "custom");
            _listChangedIgnoreTimeScale = false;

            yield return UnscaledDelta;
        }
    }

    private void DeleteFinishTweens(List<TweenUpdater> tweens, string nameOfCollection)
    {
        if (nameOfCollection == "custom")
            _listChangedIgnoreTimeScale = true;
        else if (nameOfCollection == "normal")
            _listChanged = true;

            List<int> toRemoveAt = new List<int>();
        for (int i = 0; i < tweens.Count; i++)
        {
            if (tweens[i].IsFinish)
                toRemoveAt.Add(i);
        }

        for (int i = 0; i < toRemoveAt.Count; i++)
            _existTweens.RemoveAt(toRemoveAt[i]);

        DebugEx.CLog("Tween delete happend", "blue");
    }
    #endregion

    #region Utils for tween update
    public AnimationCurve GetCurve(TweenEnum tween) => tweenConfig.GetTween(tween);
    public void MakeTweenIgnoreTimeScale(TweenUpdater normal)
    {
        _listChanged = true;
        _listChangedIgnoreTimeScale = true;

        if (_existTweens.Contains(normal))
            _existTweens.Remove(normal);

        _existTweensIgnoreTimeScale.Add(normal);
    }
    #endregion
}
