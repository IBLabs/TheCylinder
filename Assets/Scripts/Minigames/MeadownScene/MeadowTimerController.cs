using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MeadowTimerController : MonoBehaviour
{
    private TextMeshProUGUI _timerText;

    void Start()
    {
        _timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (NetworkMeadowGameManager.Instance == null) return;

        float timeLeft = NetworkMeadowGameManager.Instance.GameTimeLeft.Value;

        _timerText.text = $"{(int)timeLeft / 60:00}:{(int)timeLeft % 60:00}";
    }
}
