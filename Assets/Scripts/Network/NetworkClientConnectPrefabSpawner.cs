using DG.Tweening;

using Unity.Netcode;

using UnityEngine;

public class NetworkClientConnectPrefabSpawner : MonoBehaviour
{
    [SerializeField] private CanvasGroup startButtonCanvasGroup;

    private bool hasActivated = false;

    void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += HandleConnectionEvent;
    }

    void OnDestroy()
    {
        NetworkManager.Singleton.OnConnectionEvent -= HandleConnectionEvent;
    }

    private void HandleConnectionEvent(NetworkManager manager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientConnected && eventData.ClientId != manager.LocalClientId)
        {
            if (manager.IsServer)
            {
                if (!hasActivated)
                {
                    hasActivated = true;

                    startButtonCanvasGroup.interactable = true;
                    startButtonCanvasGroup.blocksRaycasts = true;

                    startButtonCanvasGroup.DOFade(1f, 0.5f);
                }
            }
        }
    }
}
