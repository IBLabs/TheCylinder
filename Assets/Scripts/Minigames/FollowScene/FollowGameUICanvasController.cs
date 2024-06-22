using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class FollowGameUICanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTimeText;

    private NetworkFollowGameManager _gameManager;

    void Start()
    {
        _gameManager = FindAnyObjectByType<NetworkFollowGameManager>();
    }

    void Update()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        var timeLeft = _gameManager.GameTimeLeft;
        gameTimeText.text = $"{(int)timeLeft / 60:00}:{(int)timeLeft % 60:00}";
    }
}
