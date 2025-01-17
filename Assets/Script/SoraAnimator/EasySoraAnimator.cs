using System;
using UnityEngine;

public class EasySoraAnimator
{
    public string PropertyName { get; private set; }
    private object startValue;
    private object endValue;
    private float duration;
    private Func<float, float> easingFunction;
    private float elapsedTime;
    private bool isPlaying;
    private Action onComplete;
    private object targetComponent;

    public EasySoraAnimator(object targetComponent)
    {
        this.targetComponent = targetComponent;
    }

    public void SetAnimation(string propertyName, object startValue, object endValue, float duration, Func<float, float> easingFunction, Action onComplete = null)
    {
        this.PropertyName = propertyName;
        this.startValue = startValue;
        this.endValue = endValue;
        this.duration = duration;
        this.easingFunction = easingFunction;
        this.onComplete = onComplete;
    }

    public void Play()
    {
        isPlaying = true;
        elapsedTime = 0f;
    }

    public void Update()
    {
        if (!isPlaying) return;

        elapsedTime += UnityEngine.Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        float easedT = easingFunction(t);
        object currentValue = LerpValue(startValue, endValue, easedT);

        SetPropertyValue(currentValue);

        if (elapsedTime >= duration)
        {
            SetPropertyValue(endValue);
            onComplete?.Invoke();
            isPlaying = false;
        }
    }

    private object LerpValue(object start, object end, float t)
    {
        if (start is float startFloat && end is float endFloat)
        {
            return Mathf.Lerp(startFloat, endFloat, t);
        }
        else if (start is UnityEngine.Vector3 startVec && end is UnityEngine.Vector3 endVec)
        {
            return UnityEngine.Vector3.Lerp(startVec, endVec, t);
        }
        else if (start is UnityEngine.Color startColor && end is UnityEngine.Color endColor)
        {
            return UnityEngine.Color.Lerp(startColor, endColor, t);
        }
        else if (start is UnityEngine.Quaternion startQuat && end is UnityEngine.Quaternion endQuat)
        {
            return UnityEngine.Quaternion.Slerp(startQuat, endQuat, t);
        }
        return start;
    }

    private void SetPropertyValue(object value)
    {
        if (targetComponent != null)
        {
            var propertyInfo = targetComponent.GetType().GetProperty(PropertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(targetComponent, value);
            }
            else
            {
                UnityEngine.Debug.LogError($"无法设置属性 {PropertyName}，请确保该属性是可写的。");
            }
        }
    }
}
