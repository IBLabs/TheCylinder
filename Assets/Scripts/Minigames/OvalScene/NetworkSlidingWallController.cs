using Unity.Netcode;

using UnityEngine.Splines;

public class NetworkSlidingWallController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        ConfigureSlidingDoors();

        base.OnNetworkSpawn();
    }

    private void ConfigureSlidingDoors()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                foreach (var splineAnimator in GetComponentsInChildren<SplineAnimate>())
                {
                    splineAnimator.enabled = false;
                }
            }
        }
    }
}
