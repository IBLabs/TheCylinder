using System;
using TMPro;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveSpeedValueText;
    [SerializeField] private HandTransformer rightHandTransformer;
    [SerializeField] private HandTransformer leftHandTransformer;

    private void OnEnable()
    {
        moveSpeedValueText.text = rightHandTransformer.moveSpeed.ToString();
    }

    public void OnMoveSpeedValueChanged(float newValue)
    {
        rightHandTransformer.moveSpeed = newValue;
        leftHandTransformer.moveSpeed = newValue;

        moveSpeedValueText.text = $"{newValue}";
    }
}