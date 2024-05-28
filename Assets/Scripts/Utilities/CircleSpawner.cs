using UnityEngine;
using System.Collections.Generic;

public class CircleSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfObjects = 10;
    public float radius = 5f;

    private List<GameObject> instances = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            CreateInstance(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            UpdateInstance(i);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 newPos = GetPosition(i);
            Gizmos.DrawCube(newPos, new Vector3(.2f, .2f, .2f));
        }
    }

    private void CreateInstance(int index)
    {
        Vector3 newPos = GetPosition(index);
        GameObject instance = Instantiate(prefab, newPos, Quaternion.identity, transform);
        instance.transform.LookAt(transform.position);
        instance.transform.Rotate(0, 180, 0); // Add this line
        instances.Add(instance);
    }

    private void UpdateInstance(int index)
    {
        Vector3 newPos = GetPosition(index);
        instances[index].transform.position = newPos;
        instances[index].transform.LookAt(transform.position);
        instances[index].transform.Rotate(0, 180, 0); // Add this line
    }

    private Vector3 GetPosition(int index)
    {
        float angle = index * Mathf.PI * 2 / numberOfObjects;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + transform.position;
    }
}