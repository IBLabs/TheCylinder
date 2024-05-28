using UnityEngine;
using TMPro;
using Utilities;

public class IPPrinter : MonoBehaviour
{
    [SerializeField] private bool waitForJoinCode;

    public TextMeshProUGUI ipText;

    private void Start()
    {
        if (!waitForJoinCode)
        {
            string ipAddress = ItaNetworkUtilities.GetLocalIPAddress();
            ipText.text = ipAddress;
        }
    }

    public void OnRelayJoinCodeReceived(string joinCode)
    {
        ipText.text = joinCode;
    }
}