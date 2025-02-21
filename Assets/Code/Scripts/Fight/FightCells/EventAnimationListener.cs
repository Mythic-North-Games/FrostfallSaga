using UnityEngine;
using System;

public class AnimationEventListener : MonoBehaviour
{
    public event Action OnAnimationEnd;

    public void OnEndOfAnimation()
    {
        OnAnimationEnd?.Invoke();
    }
}
