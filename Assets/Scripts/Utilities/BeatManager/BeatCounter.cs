using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class BeatCounter : MonoBehaviour
{
    private int _beatCount = 0;

    public UnityEvent OnLiftOff;

    public void OnTrigger()
    {
        _beatCount++;
        Debug.Log($"{GetType().Name}: Beat {_beatCount}");

        if (_beatCount == 32)
        {
            Debug.Log($"{GetType().Name}: lift off!");

            OnLiftOff?.Invoke();
        }
    }
}
