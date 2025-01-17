using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 补间函数
/// </summary>
public static class EasingFunctions
{
    /// <summary>
    /// 线性补间：匀速过渡
    /// </summary>
    public static float Linear(float t)
    {
        return t;  // 匀速过渡
    }

    /// <summary>
    /// 缓入：二次加速，开始时慢，逐渐加速
    /// </summary>
    public static float EaseIn(float t)
    {
        return t * t;
    }

    /// <summary>
    /// 缓出：二次减速，开始时快，逐渐减速
    /// </summary>
    public static float EaseOut(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    /// <summary>
    /// 缓入缓出：先加速再减速
    /// </summary>
    public static float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    /// <summary>
    /// 缓入：二次加速
    /// </summary>
    public static float QuadIn(float t)
    {
        return t * t;
    }

    /// <summary>
    /// 缓出：二次减速
    /// </summary>
    public static float QuadOut(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    /// <summary>
    /// 缓入缓出：二次加速减速
    /// </summary>
    public static float QuadInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    /// <summary>
    /// 缓入：三次加速
    /// </summary>
    public static float CubicIn(float t)
    {
        return t * t * t;
    }

    /// <summary>
    /// 缓出：三次减速
    /// </summary>
    public static float CubicOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    /// <summary>
    /// 缓入缓出：三次加速减速
    /// </summary>
    public static float CubicInOut(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    /// <summary>
    /// 缓入：四次加速
    /// </summary>
    public static float QuartIn(float t)
    {
        return t * t * t * t;
    }

    /// <summary>
    /// 缓出：四次减速
    /// </summary>
    public static float QuartOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 4f);
    }

    /// <summary>
    /// 缓入缓出：四次加速减速
    /// </summary>
    public static float QuartInOut(float t)
    {
        return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
    }

    /// <summary>
    /// 缓入：五次加速
    /// </summary>
    public static float QuintIn(float t)
    {
        return t * t * t * t * t;
    }

    /// <summary>
    /// 缓出：五次减速
    /// </summary>
    public static float QuintOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 5f);
    }

    /// <summary>
    /// 缓入缓出：五次加速减速
    /// </summary>
    public static float QuintInOut(float t)
    {
        return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
    }

    /// <summary>
    /// 弹跳入：弹跳效果的反向
    /// </summary>
    public static float BounceIn(float t)
    {
        return 1f - BounceOut(1f - t);
    }

    /// <summary>
    /// 弹跳出：弹跳效果
    /// </summary>
    public static float BounceOut(float t)
    {
        if (t < 1f / 2.75f)
        {
            return 7.5625f * t * t;
        }
        else if (t < 2f / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }

    /// <summary>
    /// 弹跳入出：弹跳效果的前后反向
    /// </summary>
    public static float BounceInOut(float t)
    {
        return t < 0.5f ? BounceIn(t * 2f) * 0.5f : BounceOut(t * 2f - 1f) * 0.5f + 0.5f;
    }

    /// <summary>
    /// 弹性入：弹性加速
    /// </summary>
    public static float ElasticIn(float t)
    {
        return Mathf.Sin(13f * Mathf.PI / 2f * t) * Mathf.Pow(2f, 10f * (t - 1f));
    }

    /// <summary>
    /// 弹性出：弹性减速
    /// </summary>
    public static float ElasticOut(float t)
    {
        return Mathf.Sin(-13f * Mathf.PI / 2f * (t + 1f)) * Mathf.Pow(2f, -10f * t) + 1f;
    }

    /// <summary>
    /// 弹性入出：弹性加速减速
    /// </summary>
    public static float ElasticInOut(float t)
    {
        return t < 0.5f ? ElasticIn(t * 2f) * 0.5f : ElasticOut(t * 2f - 1f) * 0.5f + 0.5f;
    }

    /// <summary>
    /// 反向弹性入：反向弹性加速
    /// </summary>
    public static float BackIn(float t)
    {
        const float s = 1.70158f;
        return t * t * ((s + 1f) * t - s);
    }

    /// <summary>
    /// 反向弹性出：反向弹性减速
    /// </summary>
    public static float BackOut(float t)
    {
        const float s = 1.70158f;
        return 1f - (1f - t) * (1f - t) * ((s + 1f) * (1f - t) + s);
    }

    /// <summary>
    /// 反向弹性入出：反向弹性加速减速
    /// </summary>
    public static float BackInOut(float t)
    {
        const float s = 1.70158f;
        return t < 0.5f
            ? 2f * t * t * ((s * 1.525f + 1f) * t - s * 1.525f)
            : 1f - Mathf.Pow(1f - (t * 2f - 1f), 2f) * ((s * 1.525f + 1f) * (t * 2f - 1f) + s * 1.525f);
    }
}

/// <summary>
/// 负责执行动画的组件类，支持多种动画过渡效果
/// </summary>
public class SoraAnimator : MonoBehaviour
{
    private Action onComplete;
    private string propertyName;
    private object startValue;
    private object endValue;
    private float duration;
    private Func<float, float> easingFunction;
    private float delay;
    private SoraAnimator nextAnimator;

    private Component targetComponent;

    public SoraAnimator(Component targetComponent)
    {
        this.targetComponent = targetComponent;
    }

    /// <summary>
    /// 设置动画的参数
    /// </summary>
    /// <param name="propertyName">动画属性名</param>
    /// <param name="startValue">动画起始值</param>
    /// <param name="endValue">动画结束值</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="easingFunction">缓动函数</param>
    /// <param name="delay">延迟时间</param>
    /// <param name="onComplete">动画完成后的回调函数</param>
    public void SetAnimation<T>(T target, string propertyName, object startValue, object endValue, float duration, Func<float, float> easingFunction, float delay = 0f, Action onComplete = null)
    {
        this.propertyName = propertyName;
        this.startValue = startValue;
        this.endValue = endValue;
        this.duration = duration;
        this.easingFunction = easingFunction;
        this.delay = delay;
        this.onComplete = onComplete;
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void Play()
    {
        StartCoroutine(AnimateWithDelay());
    }

    // 带延迟的动画播放
    private IEnumerator AnimateWithDelay()
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }
        yield return Animate();
    }

    // 动画具体实现
    private IEnumerator Animate()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = easingFunction(t);
            object currentValue = LerpValue(startValue, endValue, easedT);

            SetPropertyValue(currentValue);
            yield return null;
        }

        SetPropertyValue(endValue);
        onComplete?.Invoke();

        if (nextAnimator != null)
        {
            nextAnimator?.Play();
        }
    }

    /// <summary>
    /// 设置动画完成后的回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetOnComplete(Action callback)
    {
        onComplete = callback;
    }

    // 根据不同类型进行插值计算
    private object LerpValue(object start, object end, float t)
    {
        if (start is float startFloat && end is float endFloat)
        {
            return Mathf.Lerp(startFloat, endFloat, t);
        }
        else if (start is Vector3 startVec && end is Vector3 endVec)
        {
            return Vector3.Lerp(startVec, endVec, t);
        }
        else if (start is Color startColor && end is Color endColor)
        {
            return Color.Lerp(startColor, endColor, t);
        }
        else if (start is Vector2 startVec2 && end is Vector2 endVec2)
        {
            return Vector2.Lerp(startVec2, endVec2, t);
        }
        else if (start is Quaternion startQuat && end is Quaternion endQuat)
        {
            return Quaternion.Slerp(startQuat, endQuat, t);
        }
        return start;
    }

    // 动态设置目标组件的属性值
    private void SetPropertyValue(object value)
    {
        if (targetComponent != null)
        {
            // 通过反射获取目标组件的属性信息
            var propertyInfo = targetComponent.GetType().GetProperty(propertyName);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                // 设置属性值
                propertyInfo.SetValue(targetComponent, value);
            }
            else
            {
                Debug.LogError($"找不到属性 {propertyName} 或该属性无法修改（在 {targetComponent.GetType().Name} 上）");
            }
        }
    }

    /// <summary>
    /// 与另一个动画一起执行
    /// </summary>
    /// <param name="otherAnimator"></param>
    public void While(SoraAnimator otherAnimator)
    {
        otherAnimator.onComplete += () => { Play(); };
    }

    /// <summary>
    /// 动画结束后执行下一个动画
    /// </summary>
    /// <param name="nextAnimator"></param>
    public void Then(SoraAnimator nextAnimator)
    {
        this.nextAnimator = nextAnimator;
    }
}