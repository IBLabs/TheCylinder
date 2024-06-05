using System.Collections;

using UnityEngine;

using DG.Tweening;

public class PrisonWallObstacleController : MonoBehaviour
{
    [SerializeField] private float hiddenDuration = 2f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float moveDuration = 0.4f;

    void Start()
    {
        StartCoroutine(ToggleWallCoroutine());
    }

    private IEnumerator ToggleWallCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(hiddenDuration);

            transform.DOLocalMoveY(6.2f, .4f).From(-7.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(visibleDuration);

            transform.DOLocalMoveY(-7.5f, .4f).From(6.2f).SetEase(Ease.InOutSine);
        }
    }
}
