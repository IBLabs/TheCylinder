using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class MeadowScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI desktopScoreText;
    [SerializeField] private TextMeshProUGUI vrScoreText;

    void Update()
    {
        desktopScoreText.text = NetworkMeadowGameManager.Instance.DesktopPointCount.Value.ToString();
        vrScoreText.text = NetworkMeadowGameManager.Instance.VRPointCount.Value.ToString();
    }
}
