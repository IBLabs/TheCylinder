using TMPro;
using UnityEngine;

public class HostUIController : MonoBehaviour
{
    public TextMeshProUGUI localIPAddressText;

    private void Start()
    {
        string localIPAddress = Utilities.ItaNetworkUtilities.GetLocalIPAddress();
        localIPAddressText.text = localIPAddress;
    }
}