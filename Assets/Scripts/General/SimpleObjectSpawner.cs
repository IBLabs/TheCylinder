using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Vector3 = UnityEngine.Vector3;

public class SimpleObjectSpawner : ObjectSpawner
{
    public void SimpleSpawn()
    {
        TrySpawnObject(transform.position, Vector3.up);
    }
}