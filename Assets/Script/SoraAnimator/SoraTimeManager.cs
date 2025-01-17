using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoraTimeManager : MonoBehaviour
{


    private List<EasySoraAnimator> animations = new List<EasySoraAnimator>();

    // 添加动画
    public void AddAnimation(EasySoraAnimator animation)
    {
        if (!animations.Contains(animation))
        {
            animations.Add(animation);
        }
    }

    // 更新所有动画
    public void Update()
    {
        foreach (var animation in animations)
        {
            animation.Update();
        }
    }


}
