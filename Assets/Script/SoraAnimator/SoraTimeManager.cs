using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoraTimeManager : MonoBehaviour
{


    private List<EasySoraAnimator> animations = new List<EasySoraAnimator>();

    // ��Ӷ���
    public void AddAnimation(EasySoraAnimator animation)
    {
        if (!animations.Contains(animation))
        {
            animations.Add(animation);
        }
    }

    // �������ж���
    public void Update()
    {
        foreach (var animation in animations)
        {
            animation.Update();
        }
    }


}
