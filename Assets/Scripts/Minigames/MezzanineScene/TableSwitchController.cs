using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableSwitchController : MonoBehaviour
{
    public bool IsActivated { get; private set; }

    [SerializeField] private Transform internalPartTransform;

    [Header("Configuration")]
    [SerializeField] private float yMoveOffset = -0.012f;

    [ColorUsage(true, true)]
    [SerializeField] private Color activatedColor = Color.green;

    [Header("Events")]
    public UnityEvent<TableSwitchController> OnSwitchActivated;

    private MeshRenderer _meshRenderer;

    private bool _isSwitchEnabled = true;

    void Start()
    {
        _meshRenderer = internalPartTransform.GetComponent<MeshRenderer>();

        SetInitialState();
    }

    public void SetInitialState()
    {
        IsActivated = false;
        _meshRenderer.material.SetColor("_EmissionColor", Color.black);
        internalPartTransform.localPosition = Vector3.zero;
    }

    public void SetSwitchEnabled(bool isEnabled)
    {
        _isSwitchEnabled = isEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isSwitchEnabled) return;

        if (other.CompareTag("Player"))
        {
            IsActivated = true;

            internalPartTransform.DOLocalMoveY(yMoveOffset, 0.5f);
            _meshRenderer.material.DOColor(activatedColor, "_EmissionColor", 0.2f);

            OnSwitchActivated?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isSwitchEnabled) return;

        if (other.CompareTag("Player"))
        {
            IsActivated = false;

            internalPartTransform.DOLocalMoveY(0f, 0.5f);
            _meshRenderer.material.DOColor(Color.black, "_EmissionColor", 0.2f);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TableSwitchController))]
public class TableSwitchControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TableSwitchController tableSwitchController = (TableSwitchController)target;

        if (GUILayout.Button("Trigger"))
        {
            tableSwitchController.OnSwitchActivated?.Invoke(tableSwitchController);
        }
    }
}
#endif