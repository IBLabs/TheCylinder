
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeadowGameEndController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject canvasGameObject;
    [SerializeField] private MeshRenderer overlaySphereMeshRenderer;
    [SerializeField] private MeshRenderer uiPanelMeshRenderer;
    [SerializeField] private RectTransform firstWordRectTransform;
    [SerializeField] private RectTransform secondWordRectTransform;
    [SerializeField] private RectTransform alternateWordRectTransform;
    [SerializeField] private CanvasGroup continueButtonCanvasGroup;

    [SerializeField] private GameObject rayLeftController;
    [SerializeField] private GameObject rayRightController;

    [SerializeField] private XRDirectInteractor leftDirectInteractor;
    [SerializeField] private XRDirectInteractor rightDirectInteractor;

    [Header("Configuration")]
    [SerializeField] private float canvasRotationSpeed = 1.0f;
    [SerializeField] private float canvasRotationAmount = .1f;

    [ColorUsage(true, true)]
    [SerializeField] private Color loseColor;
    [SerializeField] private Color sphereLoseColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color winColor;
    [SerializeField] private Color sphereWinColor;

    void Start()
    {
        SetInitialState();
    }

    public void ShowGameEndScreen(WinnerType winner)
    {
        if (winner == WinnerType.VR)
        {
            uiPanelMeshRenderer.material.SetColor("_MainColor", winColor);
            overlaySphereMeshRenderer.material.SetColor("_Color_2", sphereWinColor);
        }
        else
        {
            uiPanelMeshRenderer.material.SetColor("_MainColor", loseColor);
            overlaySphereMeshRenderer.material.SetColor("_Color_2", sphereLoseColor);
        }


        leftDirectInteractor.enabled = false;
        rightDirectInteractor.enabled = false;

        rayLeftController.SetActive(true);
        rayRightController.SetActive(true);

        GetComponent<BoxCollider>().enabled = true;
        canvasGameObject.SetActive(true);

        StartCoroutine(ShowGameEndScreenCoroutine(winner));
    }

    private void SetInitialState()
    {
        GetComponent<BoxCollider>().enabled = false;
        canvasGameObject.SetActive(false);

        overlaySphereMeshRenderer.material.SetFloat("_Alpha", 0.0f);
        uiPanelMeshRenderer.material.SetFloat("_Alpha", 0.0f);

        var firstImage = firstWordRectTransform.GetComponent<Image>();
        firstImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        var secondImage = secondWordRectTransform.GetComponent<Image>();
        secondImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        var alternateImage = alternateWordRectTransform.GetComponent<Image>();
        alternateImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        continueButtonCanvasGroup.alpha = 0.0f;
    }

    private IEnumerator ShowGameEndScreenCoroutine(WinnerType winner)
    {
        overlaySphereMeshRenderer.material.DOFloat(1.0f, "_Alpha", .4f);

        uiPanelMeshRenderer.material.DOFloat(1.0f, "_Alpha", .4f);
        uiPanelMeshRenderer.transform.DOLocalMoveY(0f, .4f).SetEase(Ease.OutQuint).WaitForCompletion();

        yield return new WaitForSeconds(.2f);

        firstWordRectTransform.DOLocalMoveX(0.0f, .4f).From(new Vector3(-1.0f, 0.0f, 0.0f)).SetEase(Ease.OutQuint);
        firstWordRectTransform.GetComponent<Image>().DOFade(1.0f, .4f).WaitForCompletion();

        yield return new WaitForSeconds(.2f);

        if (winner == WinnerType.VR)
        {
            alternateWordRectTransform.DOAnchorPosX(0.0f, .4f).From(new Vector2(1.0f, 0.0f)).SetEase(Ease.OutQuint);
            alternateWordRectTransform.GetComponent<Image>().DOFade(1.0f, 1.0f);
        }
        else
        {
            secondWordRectTransform.DOAnchorPosX(0.0f, .4f).From(new Vector2(1.0f, 0.0f)).SetEase(Ease.OutQuint);
            secondWordRectTransform.GetComponent<Image>().DOFade(1.0f, 1.0f);
        }

        yield return new WaitForSeconds(.2f);

        continueButtonCanvasGroup.DOFade(1.0f, .4f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeadowGameEndController))]
public class MeadowGameEndControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeadowGameEndController myScript = (MeadowGameEndController)target;
        if (GUILayout.Button("Show Game End Screen"))
        {
            myScript.ShowGameEndScreen(WinnerType.VR);
        }
    }
}
#endif