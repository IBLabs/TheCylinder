using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeGenerator : MonoBehaviour
{
    [Header("Rope Settings")]
    public int segmentCount = 10;
    public float segmentLength = 0.5f;
    public float segmentMass = 0.2f;
    public float colliderRadius = 0.1f;

    [Header("Rope Joints Settings")]
    public float spring = 50f;
    public float damper = 5f;

    [Header("Rope End Fixation")]
    public bool fixStart = false;
    public bool fixEnd = false;

    [Header("Rope End Prefabs")]
    public GameObject startPrefab;
    public GameObject endPrefab;

    private LineRenderer lineRenderer;
    private Rigidbody[] ropeSegments;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GenerateRope();
    }

    void Update()
    {
        UpdateLineRenderer();
    }

    void GenerateRope()
    {
        ClearRopeSegments();

        ropeSegments = new Rigidbody[segmentCount];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment;
            if (i == 0 && startPrefab != null)
            {
                segment = Instantiate(startPrefab, startPosition, Quaternion.identity, transform);
                AddRigidbodyAndColliderIfNeeded(segment);
            }
            else if (i == segmentCount - 1 && endPrefab != null)
            {
                segment = Instantiate(endPrefab, startPosition + Vector3.down * segmentLength * i, Quaternion.identity, transform);
                AddRigidbodyAndColliderIfNeeded(segment);
            }
            else
            {
                segment = new GameObject("RopeSegment" + i);
                segment.transform.position = startPosition + Vector3.down * segmentLength * i;
                segment.transform.parent = transform;

                Rigidbody rb = segment.AddComponent<Rigidbody>();
                rb.mass = segmentMass;

                SphereCollider collider = segment.AddComponent<SphereCollider>();
                collider.radius = colliderRadius;

                ropeSegments[i] = rb;
            }

            if (i > 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = ropeSegments[i - 1];

                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;

                joint.angularXMotion = ConfigurableJointMotion.Limited;
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                joint.angularZMotion = ConfigurableJointMotion.Limited;

                SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
                limitSpring.spring = spring;
                limitSpring.damper = damper;
                joint.angularXLimitSpring = limitSpring;
                joint.angularYZLimitSpring = limitSpring;

                SoftJointLimit lowLimit = new SoftJointLimit();
                lowLimit.limit = -10f;
                joint.lowAngularXLimit = lowLimit;

                SoftJointLimit highLimit = new SoftJointLimit();
                highLimit.limit = 10f;
                joint.highAngularXLimit = highLimit;

                SoftJointLimit angularYLimit = new SoftJointLimit();
                angularYLimit.limit = 10f;
                joint.angularYLimit = angularYLimit;

                SoftJointLimit angularZLimit = new SoftJointLimit();
                angularZLimit.limit = 10f;
                joint.angularZLimit = angularZLimit;
            }

            ropeSegments[i] = segment.GetComponent<Rigidbody>();

            if (i == 0 && fixStart)
            {
                ropeSegments[i].isKinematic = true;
            }

            if (i == segmentCount - 1 && fixEnd)
            {
                ropeSegments[i].isKinematic = true;
            }
        }
    }

    void AddRigidbodyAndColliderIfNeeded(GameObject segment)
    {
        if (segment.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.mass = segmentMass;
        }

        if (segment.GetComponent<Collider>() == null)
        {
            SphereCollider collider = segment.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;
        }
    }

    void ClearRopeSegments()
    {
        // Remove all previous rope segments
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
        {
            lineRenderer.SetPosition(i, ropeSegments[i].position);
        }
    }
}
